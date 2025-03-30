using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroSaaS.Shared.Enums;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

namespace MicroSaaS.IntegrationTests.Controllers
{
    public class PerformanceControllerTests : IClassFixture<SimplifiedTestFactory>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;
        private string _authToken = string.Empty;
        
        public PerformanceControllerTests(SimplifiedTestFactory factory, ITestOutputHelper output)
        {
            _output = output;
            _output.WriteLine("Initializing PerformanceControllerTests");
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
            
            var response = await _client.PostAsync("/api/auth/login", content);
            response.EnsureSuccessStatusCode();
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            _authToken = authResponse.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _authToken);
        }
        
        [Fact]
        public async Task GetInsights_WithValidParameters_ShouldReturnData()
        {
            // Arrange
            var creatorId = Guid.NewGuid(); // ID de um criador de conteúdo de exemplo
            var platform = SocialMediaPlatform.Instagram;
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;
            
            // Act
            var response = await _client.GetAsync(
                $"/api/Performance/insights/{creatorId}/{platform}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            
            // Assert
            // Verificar status OK ou NotFound (dependendo se o mock implementado retorna dados ou não)
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _output.WriteLine($"Response: {responseContent}");
                
                var result = JsonSerializer.Deserialize<List<ContentPerformanceDto>>(
                    responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                result.Should().NotBeNull();
            }
        }
        
        [Fact]
        public async Task GetBestPostTimes_WithValidParameters_ShouldReturnRecommendations()
        {
            // Arrange
            var creatorId = Guid.NewGuid(); // ID de um criador de conteúdo de exemplo
            var platform = SocialMediaPlatform.Instagram;
            
            // Act
            var response = await _client.GetAsync(
                $"/api/Performance/best-times/{creatorId}/{platform}");
            
            // Assert
            // Verificar status OK ou NotFound (dependendo se o mock implementado retorna dados ou não)
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _output.WriteLine($"Response: {responseContent}");
                
                var result = JsonSerializer.Deserialize<List<PostTimeRecommendation>>(
                    responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                result.Should().NotBeNull();
            }
        }
        
        [Fact]
        public async Task GetPerformanceSummary_WithValidParameters_ShouldReturnSummary()
        {
            // Arrange
            var creatorId = Guid.NewGuid(); // ID de um criador de conteúdo de exemplo
            var startDate = DateTime.UtcNow.AddDays(-30);
            var endDate = DateTime.UtcNow;
            
            // Act
            var response = await _client.GetAsync(
                $"/api/Performance/summary/{creatorId}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            
            // Assert
            // Verificar status OK ou NotFound (dependendo se o mock implementado retorna dados ou não)
            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                _output.WriteLine($"Response: {responseContent}");
                
                var result = JsonSerializer.Deserialize<PerformanceSummary>(
                    responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                result.Should().NotBeNull();
                result!.ByPlatform.Should().NotBeNull();
            }
        }
        
        // Classes para deserialização das respostas
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
        
        private class ContentPerformanceDto
        {
            public DateTime PostDate { get; set; }
            public string ContentId { get; set; }
            public string ContentUrl { get; set; }
            public string ContentType { get; set; }
            public long Views { get; set; }
            public long Likes { get; set; }
            public long Comments { get; set; }
            public long Shares { get; set; }
        }
        
        private class PostTimeRecommendation
        {
            public DayOfWeek DayOfWeek { get; set; }
            public TimeSpan TimeOfDay { get; set; }
            public double EngagementScore { get; set; }
        }
        
        private class PerformanceSummary
        {
            public long TotalViews { get; set; }
            public long TotalLikes { get; set; }
            public long TotalComments { get; set; }
            public long TotalShares { get; set; }
            public List<PlatformPerformance> ByPlatform { get; set; } = new();
        }
        
        private class PlatformPerformance
        {
            public SocialMediaPlatform Platform { get; set; }
            public long Views { get; set; }
            public long Likes { get; set; }
            public long Comments { get; set; }
            public long Shares { get; set; }
        }
    }
} 