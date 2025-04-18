using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroSaaS.Domain.Entities;
using MicroSaaS.IntegrationTests.Utils;
using MicroSaaS.Application.DTOs.Checklist;
using MicroSaaS.Shared.DTOs;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Linq;

namespace MicroSaaS.IntegrationTests.Controllers
{
    public class ContentChecklistControllerTests : IClassFixture<SimplifiedTestFactory>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        private string _authToken = string.Empty;

        // IDs fixos para testes convertidos para Guid
        private static readonly Guid CREATOR_ID_1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid CREATOR_ID_2 = Guid.Parse("22222222-2222-2222-2222-222222222222");

        public ContentChecklistControllerTests(SimplifiedTestFactory factory, ITestOutputHelper output)
        {
            _output = output;
            _output.WriteLine("Initializing ContentChecklistControllerTests");
            _client = factory.CreateClient();

            // Obtém token de autenticação para os testes
            GetAuthTokenAsync().GetAwaiter().GetResult();
        }

        private async Task GetAuthTokenAsync()
        {
            var loginRequest = new
            {
                Email = "test@example.com",
                Password = "Test@123"
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _output.WriteLine("Enviando solicitação de login para o controlador de teste");
            // Usar endpoint específico para evitar ambiguidade
            var response = await _client.PostAsync("/api/auth/login", content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (authResponse == null)
            {
                throw new InvalidOperationException("Falha ao desserializar resposta de autenticação");
            }

            _authToken = authResponse.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
        }

        private async Task<Guid> CreateChecklist_WithValidData_ShouldReturnCreatedChecklist()
        {
            // Arrange
            var request = new CreateChecklistRequestDto
            {
                CreatorId = CREATOR_ID_1,
                Title = "Teste de Checklist " + DateTime.UtcNow.Ticks,
                Description = "Descrição do checklist de teste"
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/ContentChecklist", content);

            // Log the response content regardless of status code
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Response status: {response.StatusCode}");
            _output.WriteLine($"Response body: {responseContent}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var result = JsonSerializer.Deserialize<ContentChecklistDto>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            result.Should().NotBeNull();
            result!.Id.Should().NotBe(Guid.Empty);
            result.Title.Should().Be(request.Title);

            return result.Id;
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnChecklist()
        {
            // Arrange
            var checklistId = await CreateChecklist_WithValidData_ShouldReturnCreatedChecklist();

            // Act
            var response = await _client.GetAsync($"/api/ContentChecklist/{checklistId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Response from GetById: {responseContent}");

            var result = JsonSerializer.Deserialize<ContentChecklistDto>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            result.Should().NotBeNull();
            result!.Id.Should().Be(checklistId);
        }

        [Fact]
        public async Task GetByCreatorId_WithValidId_ShouldReturnChecklists()
        {
            // Arrange - usar ID fixo
            var creatorId = CREATOR_ID_1;

            // Criar uma checklist para o criador
            var request = new CreateChecklistRequestDto
            {
                CreatorId = creatorId,
                Title = "Checklist do Criador " + DateTime.UtcNow.Ticks,
                Description = "Descrição do checklist do criador"
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Primeiro, precisamos criar um checklist para que possamos testá-lo
            var createResponse = await _client.PostAsync("/api/ContentChecklist", content);
            if (createResponse.StatusCode != HttpStatusCode.Created)
            {
                _output.WriteLine($"Falha ao criar checklist: {await createResponse.Content.ReadAsStringAsync()}");
            }

            // Act
            var response = await _client.GetAsync($"/api/ContentChecklist/creator/{creatorId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Response from GetByCreatorId: {responseContent}");

            var result = JsonSerializer.Deserialize<List<ContentChecklistDto>>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            result.Should().NotBeNull();
            result!.Count.Should().BeGreaterThanOrEqualTo(1);
            result.Should().Contain(c => c.CreatorId == creatorId);
        }

        [Fact]
        public async Task AddItem_WithValidData_ShouldReturnUpdatedChecklist()
        {
            // Arrange
            var checklistId = await CreateChecklist_WithValidData_ShouldReturnCreatedChecklist();

            var request = new AddChecklistItemRequestDto
            {
                Description = "Item de teste " + DateTime.UtcNow.Ticks,
                IsRequired = true
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/api/ContentChecklist/{checklistId}/items", content);

            // Assert
            _output.WriteLine($"AddItem status: {response.StatusCode}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"AddItem response: {responseContent}");

            var result = JsonSerializer.Deserialize<ContentChecklistDto>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            result.Should().NotBeNull();
            result!.Items.Should().NotBeNull();
            result.Items.Should().Contain(i => i.Description == request.Description);

            // Verificar se o checklist foi atualizado no servidor
            var getResponse = await _client.GetAsync($"/api/ContentChecklist/{checklistId}");
            getResponse.EnsureSuccessStatusCode();

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var checklist = JsonSerializer.Deserialize<ContentChecklistDto>(getContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            checklist.Should().NotBeNull();
            checklist!.Items.Should().Contain(i => i.Description == request.Description);
        }

        [Fact]
        public async Task UpdateItem_WithValidData_ShouldSucceed()
        {
            // Arrange
            var checklistId = await CreateChecklist_WithValidData_ShouldReturnCreatedChecklist();

            // Adicionar item
            var addRequest = new AddChecklistItemRequestDto
            {
                Description = "Item para atualizar " + DateTime.UtcNow.Ticks,
                IsRequired = true
            };

            var addJson = JsonSerializer.Serialize(addRequest);
            var addContent = new StringContent(addJson, Encoding.UTF8, "application/json");

            var addResponse = await _client.PostAsync($"/api/ContentChecklist/{checklistId}/items", addContent);

            // Verificar se conseguimos adicionar o item
            _output.WriteLine($"AddItem status: {addResponse.StatusCode}");
            addResponse.EnsureSuccessStatusCode();

            var addResponseContent = await addResponse.Content.ReadAsStringAsync();
            _output.WriteLine($"AddItem response: {addResponseContent}");

            var addResult = JsonSerializer.Deserialize<ContentChecklistDto>(addResponseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            addResult.Should().NotBeNull();
            addResult!.Items.Should().NotBeEmpty();
            var itemId = addResult.Items.First(i => i.Description == addRequest.Description).Id;

            // Criar request para atualizar
            var updateRequest = new UpdateItemRequestDto
            {
                IsCompleted = true
            };

            var updateJson = JsonSerializer.Serialize(updateRequest);
            var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/api/ContentChecklist/{checklistId}/items/{itemId}", updateContent);

            // Assert
            _output.WriteLine($"UpdateItem status: {response.StatusCode}");
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verificar se o item foi atualizado
            var getResponse = await _client.GetAsync($"/api/ContentChecklist/{checklistId}");
            getResponse.EnsureSuccessStatusCode();

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var checklist = JsonSerializer.Deserialize<ContentChecklistDto>(getContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            checklist.Should().NotBeNull();
            var updatedItem = checklist!.Items.FirstOrDefault(i => i.Id == itemId);
            updatedItem.Should().NotBeNull();
            updatedItem!.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task DeleteChecklist_WithValidId_ShouldSucceed()
        {
            // Arrange
            var checklistId = await CreateChecklist_WithValidData_ShouldReturnCreatedChecklist();

            // Act
            var response = await _client.DeleteAsync($"/api/ContentChecklist/{checklistId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Verificar que foi excluído
            var getResponse = await _client.GetAsync($"/api/ContentChecklist/{checklistId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

            // Log do resultado para depuração
            _output.WriteLine($"DeleteChecklist status: {response.StatusCode}, verificação após exclusão: {getResponse.StatusCode}");
        }

        // Classes para serialização e deserialização
        private class AuthResponse
        {
            public bool Success { get; set; }
            public string Token { get; set; } = string.Empty;
            public UserDto User { get; set; } = new UserDto();
        }

        private class UserDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }


    }
}