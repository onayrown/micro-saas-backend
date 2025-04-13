using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Models;
using MicroSaaS.IntegrationTests.Utils;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

namespace MicroSaaS.IntegrationTests.Controllers
{
    public class ContentCreatorControllerTests : IClassFixture<SimplifiedTestFactory>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        private string _authToken = string.Empty;

        private static readonly Guid CREATOR_ID_1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid CREATOR_ID_2 = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");

        public ContentCreatorControllerTests(SimplifiedTestFactory factory, ITestOutputHelper output)
        {
            _output = output;
            _output.WriteLine("Initializing ContentCreatorControllerTests");
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

        [Fact]
        public async Task GetCurrentCreator_WithValidToken_ShouldReturnCreator()
        {
            // Arrange
            _output.WriteLine("Starting test: GetCurrentCreator_WithValidToken_ShouldReturnCreator");

            // Act
            // Usamos a rota alternativa para evitar ambiguidade
            var response = await _client.GetAsync("/api/v1/creators/current");

            // Assert
            _output.WriteLine($"Response status: {response.StatusCode}");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Response: {responseContent}");
            
            var result = JsonSerializer.Deserialize<ApiResponse<ContentCreatorDto>>(responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            result.Should().NotBeNull();
            result!.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Name.Should().NotBeNullOrEmpty();
            result.Data.Email.Should().NotBeNullOrEmpty();
            result.Data.SocialMediaAccounts.Should().NotBeNull();
        }

        private async Task<Guid> CreateCreator_WithValidData_ShouldReturnCreatedCreator()
        {
            // Arrange
            var creatorDto = new ContentCreatorDto
            {
                Name = "Creator Test",
                Email = "creator_test@example.com",
                Username = "creator_test",
                Bio = "Test bio for creator",
                ProfileImageUrl = "https://example.com/avatar.jpg",
                WebsiteUrl = "https://example.com/creator"
            };

            var json = JsonSerializer.Serialize(creatorDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/creators", content);

            // Assert
            _output.WriteLine($"Response status: {response.StatusCode}");
            if (response.StatusCode != HttpStatusCode.Created)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _output.WriteLine($"Error creating creator: {errorContent}");
            }
            
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Response: {responseContent}");
            
            var result = JsonSerializer.Deserialize<ContentCreatorDto>(responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            result.Should().NotBeNull();
            result!.Id.Should().NotBe(Guid.Empty);
            result.Name.Should().Be(creatorDto.Name);
            result.Email.Should().Be(creatorDto.Email);
            result.Username.Should().Be(creatorDto.Username);
            
            return result.Id;
        }

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnCreator()
        {
            // Arrange
            // Usamos o ID fixo do criador que já está inicializado em TestContentCreatorController
            var creatorId = CREATOR_ID_1;

            // Act
            var response = await _client.GetAsync($"/api/v1/creators/{creatorId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ContentCreatorDto>(responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            result.Should().NotBeNull();
            result!.Id.Should().Be(creatorId);
            result.Name.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task UpdateCreator_WithValidData_ShouldSucceed()
        {
            // Arrange
            var creatorId = await CreateCreator_WithValidData_ShouldReturnCreatedCreator();
            
            var updateDto = new ContentCreatorDto
            {
                Name = "Updated Creator Name",
                Email = "updated_creator@example.com",
                Username = "updated_creator",
                Bio = "Updated bio information",
                ProfileImageUrl = "https://example.com/updated_avatar.jpg",
                WebsiteUrl = "https://example.com/updated_creator"
            };

            var json = JsonSerializer.Serialize(updateDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/api/v1/creators/{creatorId}", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            
            // Verifica se a atualização foi aplicada
            var getResponse = await _client.GetAsync($"/api/v1/creators/{creatorId}");
            getResponse.EnsureSuccessStatusCode();
            
            var responseContent = await getResponse.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<ContentCreatorDto>(responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            result.Should().NotBeNull();
            result!.Name.Should().Be(updateDto.Name);
            result.Email.Should().Be(updateDto.Email);
            result.Username.Should().Be(updateDto.Username);
        }

        [Fact]
        public async Task DeleteCreator_WithValidId_ShouldSucceed()
        {
            // Arrange
            var creatorId = await CreateCreator_WithValidData_ShouldReturnCreatedCreator();

            // Act
            var response = await _client.DeleteAsync($"/api/v1/creators/{creatorId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            
            // Verifica se foi realmente excluído
            var getResponse = await _client.GetAsync($"/api/v1/creators/{creatorId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

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
    }
} 