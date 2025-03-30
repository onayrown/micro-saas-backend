using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Enums;
using Moq;
using Xunit;
using MicroSaaS.IntegrationTests.Models;

namespace MicroSaaS.IntegrationTests
{
    public class DashboardTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly Mock<IDashboardService> _mockDashboardService;
        private readonly Mock<ITokenService> _mockTokenService;

        public DashboardTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _mockDashboardService = new Mock<IDashboardService>();
            _mockTokenService = new Mock<ITokenService>();
            
            // Configura o TokenService para sempre retornar válido para testes
            _mockTokenService.Setup(ts => ts.ValidateToken(It.IsAny<string>()))
                .Returns(true);
            
            _mockTokenService.Setup(ts => ts.GetUserIdFromToken(It.IsAny<string>()))
                .Returns(Guid.Parse("11111111-1111-1111-1111-111111111111"));
            
            // Configura o cliente HTTP com o factory personalizado
            _client = _factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped(_ => _mockDashboardService.Object);
                    services.AddScoped(_ => _mockTokenService.Object);
                });
            }).CreateClient();
            
            // Adiciona o token de autenticação para todos os requests
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-token");
        }

        [Fact]
        public async Task GetLatestInsights_WithValidToken_ReturnsOk()
        {
            // Arrange
            var creatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            
            // Configurar o serviço mock para retornar um token válido
            _mockTokenService.Setup(ts => ts.ValidateToken(It.IsAny<string>()))
                .Returns(true);
            
            _mockTokenService.Setup(ts => ts.GetUserIdFromToken(It.IsAny<string>()))
                .Returns(creatorId);
            
            // Act
            var response = await _client.GetAsync($"/api/Dashboard/insights/{creatorId}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var insights = JsonSerializer.Deserialize<DashboardInsights>(stringResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(insights);
            Assert.Equal(creatorId, insights.CreatorId);
            Assert.NotEmpty(insights.Recommendations);
            Assert.NotEmpty(insights.TopContentInsights);
        }

        [Fact]
        public async Task GenerateInsights_WithDateRange_ReturnsOk()
        {
            // Arrange
            var creatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var startDate = DateTime.UtcNow.AddDays(-30).ToString("yyyy-MM-dd");
            var endDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
            
            // Act
            var response = await _client.GetAsync($"/api/Dashboard/insights/{creatorId}/generate?startDate={startDate}&endDate={endDate}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var insights = JsonSerializer.Deserialize<DashboardInsights>(stringResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(insights);
            Assert.Equal(creatorId, insights.CreatorId);
            
            // Verificar se a entidade tem dados válidos
            Assert.True(insights.TotalFollowers > 0);
            Assert.True(insights.TotalPosts > 0);
            Assert.True(insights.TotalViews > 0);
            Assert.NotEmpty(insights.TopContentInsights);
            Assert.NotEmpty(insights.Recommendations);
            Assert.NotEmpty(insights.BestTimeToPost);
            
            // Verificar se o período está correto (aproximadamente)
            Assert.True(insights.PeriodStart <= DateTime.Parse(startDate).AddDays(1));
            Assert.True(insights.PeriodEnd >= DateTime.Parse(endDate).AddDays(-1));
        }

        [Fact]
        public async Task GetMetrics_WithValidCreatorId_ReturnsMetrics()
        {
            // Arrange
            var creatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            
            // Act
            var response = await _client.GetAsync($"/api/Dashboard/metrics/{creatorId}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var metrics = JsonSerializer.Deserialize<List<PerformanceMetrics>>(stringResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(metrics);
            Assert.NotEmpty(metrics);
            Assert.All(metrics, m => Assert.Equal(creatorId, m.CreatorId));
        }

        [Fact]
        public async Task GetMetrics_WithPlatformFilter_ReturnsFitleredMetrics()
        {
            // Arrange
            var creatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var platform = SocialMediaPlatform.Instagram;
            
            // Act
            var response = await _client.GetAsync($"/api/Dashboard/metrics/{creatorId}?platform={platform}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var metrics = JsonSerializer.Deserialize<List<PerformanceMetrics>>(stringResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(metrics);
            Assert.NotEmpty(metrics);
            Assert.All(metrics, m => Assert.Equal(platform, m.Platform));
        }

        [Fact]
        public async Task GetDailyMetrics_ReturnsMetricsForSpecificDay()
        {
            // Arrange
            var creatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var date = DateTime.UtcNow.Date.ToString("yyyy-MM-dd");
            
            // Act
            var response = await _client.GetAsync($"/api/Dashboard/metrics/{creatorId}/daily?date={date}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var metric = JsonSerializer.Deserialize<PerformanceMetrics>(stringResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(metric);
            Assert.Equal(creatorId, metric.CreatorId);
            Assert.Equal(DateTime.Parse(date).Date, metric.Date.Date);
        }

        [Fact]
        public async Task GetTopContent_LimitParameter_ReturnsRequestedNumberOfPosts()
        {
            // Arrange
            var creatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var limit = 3;
            
            // Act
            var response = await _client.GetAsync($"/api/Dashboard/content/{creatorId}/top?limit={limit}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var posts = JsonSerializer.Deserialize<List<ContentPost>>(stringResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(posts);
            Assert.Equal(limit, posts.Count);
            Assert.All(posts, p => Assert.Equal(creatorId, p.CreatorId));
        }

        [Fact]
        public async Task GetBestTimeToPost_ReturnsSortedRecommendations()
        {
            // Arrange
            var creatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            
            // Act
            var response = await _client.GetAsync($"/api/Dashboard/recommendations/{creatorId}/posting-times");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var recommendations = JsonSerializer.Deserialize<List<MicroSaaS.Domain.Entities.PostTimeRecommendation>>(stringResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            Assert.NotNull(recommendations);
            Assert.NotEmpty(recommendations);
            
            // Verificar se as recomendações estão ordenadas por EngagementScore (decrescente)
            double? lastScore = null;
            foreach (var rec in recommendations)
            {
                if (lastScore.HasValue)
                {
                    Assert.True(rec.EngagementScore <= lastScore.Value);
                }
                lastScore = rec.EngagementScore;
            }
        }

        [Fact]
        public async Task GetAverageEngagementRate_ReturnsDecimalValue()
        {
            // Arrange
            var creatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            
            // Act
            var response = await _client.GetAsync($"/api/Dashboard/analytics/{creatorId}/engagement");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var engagementRate = JsonSerializer.Deserialize<decimal>(stringResponse);
            
            Assert.True(engagementRate > 0);
        }

        [Fact]
        public async Task AddMetrics_WithValidData_ReturnsCreated()
        {
            // Arrange
            var creatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var metrics = new PerformanceMetrics
            {
                CreatorId = creatorId,
                Date = DateTime.UtcNow.Date,
                Platform = SocialMediaPlatform.Instagram,
                TotalViews = 1500,
                TotalLikes = 750,
                TotalComments = 120,
                TotalShares = 60,
                Followers = 5500,
                EngagementRate = 4.8m
            };
            
            var content = new StringContent(
                JsonSerializer.Serialize(metrics),
                Encoding.UTF8,
                "application/json");
            
            // Act
            var response = await _client.PostAsync("/api/Dashboard/metrics", content);
            
            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            // Não verificamos o header location exato pois depende da implementação do controller
            Assert.NotNull(response.Headers.Location);
        }

        [Fact]
        public async Task AddContentPerformance_WithValidData_ReturnsCreated()
        {
            // Arrange
            var creatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var contentId = Guid.NewGuid();
            var performance = new ContentPerformance
            {
                CreatorId = creatorId,
                PostId = contentId,
                Platform = SocialMediaPlatform.Instagram,
                Views = 1500,
                Likes = 750,
                Comments = 120,
                Shares = 60,
                EngagementRate = 4.8m
            };
            
            var content = new StringContent(
                JsonSerializer.Serialize(performance),
                Encoding.UTF8,
                "application/json");
            
            // Act
            var response = await _client.PostAsync("/api/Dashboard/content-performance", content);
            
            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            // Não verificamos o header location exato pois depende da implementação do controller
            Assert.NotNull(response.Headers.Location);
        }

        [Fact]
        public async Task GetRevenueGrowth_ReturnsDecimalValue()
        {
            // Arrange
            var creatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            
            // Act
            var response = await _client.GetAsync($"/api/Dashboard/analytics/{creatorId}/revenue-growth");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var revenueGrowth = JsonSerializer.Deserialize<decimal>(stringResponse);
            
            Assert.True(revenueGrowth > 0m);
        }

        [Fact]
        public async Task GetFollowerGrowth_ReturnsIntValue()
        {
            // Arrange
            var creatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var platform = SocialMediaPlatform.Instagram;
            
            // Act
            var response = await _client.GetAsync($"/api/Dashboard/analytics/{creatorId}/follower-growth?platform={platform}");
            
            // Assert
            response.EnsureSuccessStatusCode();
            var stringResponse = await response.Content.ReadAsStringAsync();
            var followerGrowth = JsonSerializer.Deserialize<int>(stringResponse);
            
            Assert.True(followerGrowth > 0);
        }

        [Fact]
        public async Task GetLatestInsights_WithInvalidToken_ReturnsForbidden()
        {
            // Arrange
            var creatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            
            // Substituindo a configuração do TokenService temporariamente para simular token inválido
            _mockTokenService.Setup(ts => ts.ValidateToken(It.IsAny<string>()))
                .Returns(false);
            
            // Limpa cabeçalho de autenticação atual
            _client.DefaultRequestHeaders.Authorization = null;
            
            // Adiciona um token inválido
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "invalid-token");
            
            // Act
            var response = await _client.GetAsync($"/api/Dashboard/insights/{creatorId}");
            
            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            
            // Restaurar a configuração para os testes subsequentes
            _mockTokenService.Setup(ts => ts.ValidateToken(It.IsAny<string>()))
                .Returns(true);
            
            _mockTokenService.Setup(ts => ts.GetUserIdFromToken(It.IsAny<string>()))
                .Returns(Guid.Parse("11111111-1111-1111-1111-111111111111"));
            
            // Restaura cabeçalho de autenticação com token válido
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test-token");
        }
    }
} 