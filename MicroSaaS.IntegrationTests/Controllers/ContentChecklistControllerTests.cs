using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroSaaS.Domain.Entities;
using MicroSaaS.IntegrationTests.Utils;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

namespace MicroSaaS.IntegrationTests.Controllers
{
    public class ContentChecklistControllerTests : IClassFixture<SimplifiedTestFactory>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        private string _authToken = string.Empty;
        
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
            
            _authToken = authResponse.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
        }
        
        private async Task<Guid> CreateChecklist_WithValidData_ShouldReturnCreatedChecklist()
        {
            // Arrange
            var request = new CreateChecklistRequest
            {
                CreatorId = Guid.NewGuid(),
                Title = "Teste de Checklist " + DateTime.UtcNow.Ticks
            };
            
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // Act
            var response = await _client.PostAsync("/api/ContentChecklist", content);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Response: {responseContent}");
            
            var result = JsonSerializer.Deserialize<ContentChecklist>(responseContent, 
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
            var result = JsonSerializer.Deserialize<ContentChecklist>(responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            result.Should().NotBeNull();
            result!.Id.Should().Be(checklistId);
        }
        
        [Fact]
        public async Task GetByCreatorId_WithValidId_ShouldReturnChecklists()
        {
            // Arrange
            var creatorId = Guid.NewGuid();
            
            // Criar uma checklist para o criador
            var request = new CreateChecklistRequest
            {
                CreatorId = creatorId,
                Title = "Checklist do Criador " + DateTime.UtcNow.Ticks
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
            var result = JsonSerializer.Deserialize<List<ContentChecklist>>(responseContent, 
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
            
            var request = new AddChecklistItemRequest
            {
                Description = "Item de teste " + DateTime.UtcNow.Ticks,
                IsRequired = true
            };
            
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            // Act
            var response = await _client.PostAsync($"/api/ContentChecklist/{checklistId}/items", content);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ContentChecklist>(responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            result.Should().NotBeNull();
            result!.Items.Should().NotBeNull();
            result.Items.Should().Contain(i => i.Description == request.Description);
        }
        
        [Fact]
        public async Task UpdateItem_WithValidData_ShouldSucceed()
        {
            // Arrange
            var checklistId = await CreateChecklist_WithValidData_ShouldReturnCreatedChecklist();
            
            // Adicionar item
            var addRequest = new AddChecklistItemRequest
            {
                Description = "Item para atualizar " + DateTime.UtcNow.Ticks,
                IsRequired = true
            };
            
            var addJson = JsonSerializer.Serialize(addRequest);
            var addContent = new StringContent(addJson, Encoding.UTF8, "application/json");
            
            var addResponse = await _client.PostAsync($"/api/ContentChecklist/{checklistId}/items", addContent);
            addResponse.EnsureSuccessStatusCode();
            
            var addResponseContent = await addResponse.Content.ReadAsStringAsync();
            var addResult = JsonSerializer.Deserialize<ContentChecklist>(addResponseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            var itemId = addResult!.Items.First(i => i.Description == addRequest.Description).Id;
            
            // Criar request para atualizar
            var updateRequest = new UpdateItemRequest
            {
                IsCompleted = true
            };
            
            var updateJson = JsonSerializer.Serialize(updateRequest);
            var updateContent = new StringContent(updateJson, Encoding.UTF8, "application/json");
            
            // Act
            var response = await _client.PutAsync($"/api/ContentChecklist/{checklistId}/items/{itemId}", updateContent);
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
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
        }
        
        // Classes para serialização e deserialização
        private class AuthResponse
        {
            public bool Success { get; set; }
            public string Token { get; set; }
            public UserDto User { get; set; }
        }
        
        private class UserDto
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }
        
        private class CreateChecklistRequest
        {
            public Guid CreatorId { get; set; }
            public string Title { get; set; }
        }
        
        private class AddChecklistItemRequest
        {
            public string Description { get; set; }
            public bool IsRequired { get; set; }
        }
        
        private class UpdateItemRequest
        {
            public bool IsCompleted { get; set; }
        }
    }
} 