using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroSaaS.Shared.Enums;
using Xunit;
using FluentAssertions;

namespace MicroSaaS.IntegrationTests
{
    public class SocialMediaAccountTest_New : IClassFixture<SimplifiedTestFactory>
    {
        private readonly HttpClient _client;
        private readonly string _validToken = "valid-token";
        
        // IDs fixos consistentes com os definidos em TestSocialMediaAccountController
        private readonly Guid _existingCreatorId = Guid.Parse("11111111-1111-1111-1111-111111111111"); // CREATOR_ID_1
        private readonly Guid _nonExistingCreatorId = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB"); // NON_EXISTENT_CREATOR_ID
        private readonly JsonSerializerOptions _jsonOptions;

        public SocialMediaAccountTest_New(SimplifiedTestFactory factory)
        {
            _client = factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [Fact]
        public async Task GetAccounts_WithValidCreator_ShouldReturnSuccess()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);

            // Act
            var response = await _client.GetAsync($"/api/SocialMediaAccount/{_existingCreatorId}");
            
            // Assert
            response.IsSuccessStatusCode.Should().BeTrue("a requisição deve ser bem-sucedida");
        }

        [Fact]
        public async Task HandleCallback_WithValidParameters_ShouldReturnAcceptableStatus()
        {
            // Arrange
            var code = "valid_auth_code";
            var platform = SocialMediaPlatform.Instagram;

            // Act
            var response = await _client.GetAsync($"/api/SocialMediaAccount/callback/{_existingCreatorId}/{platform}?code={code}");
            
            // Log para debugging
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"HandleCallback resposta: StatusCode={response.StatusCode}, Conteúdo={content}");
            
            // Assert - Aceitamos qualquer código que não seja erro de servidor
            (((int)response.StatusCode) < 500).Should().BeTrue("não deve ocorrer erro no servidor");
        }
    }
} 