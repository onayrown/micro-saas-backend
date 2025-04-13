using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroSaaS.Application.DTOs.ContentPost;
using MicroSaaS.Shared.Enums;
using Xunit;
using FluentAssertions;

namespace MicroSaaS.IntegrationTests
{
    public class ContentPostTest_New : IClassFixture<SimplifiedTestFactory>
    {
        private readonly HttpClient _client;
        private readonly string _validToken = "valid-token";
        private readonly JsonSerializerOptions _jsonOptions;

        public ContentPostTest_New(SimplifiedTestFactory factory)
        {
            _client = factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [Fact]
        public async Task CreatePost_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
            
            // Usar um ID de criador conhecido
            var createRequest = new CreatePostRequest
            {
                CreatorId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Title = "Post de Teste Simplificado",
                Content = "Este é um post de teste simplificado",
                Platform = SocialMediaPlatform.Instagram,
                MediaUrl = "https://exemplo.com/imagem.jpg"
            };
            
            var content = new StringContent(
                JsonSerializer.Serialize(createRequest),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/ContentPost", content);
            
            // Assert - Aceitamos qualquer status code de sucesso
            response.IsSuccessStatusCode.Should().BeTrue("o post deve ser criado com sucesso");
            
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ContentPostDto>(responseContent, _jsonOptions);
                
                result.Should().NotBeNull();
                result!.Id.Should().NotBe(Guid.Empty);
            }
        }
    }

    // Classes para request/response
    public class CreatePostRequest
    {
        public Guid CreatorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public SocialMediaPlatform Platform { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
    }
} 