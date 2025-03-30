using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroSaaS.IntegrationTests.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MicroSaaS.IntegrationTests
{
    public class RevenueTests : IClassFixture<SimplifiedTestFactory>
    {
        private readonly HttpClient _client;
        private readonly string _validToken = "valid-token";
        private readonly string _invalidToken = "invalid-token";
        private readonly JsonSerializerOptions _jsonOptions;

        public RevenueTests(SimplifiedTestFactory factory)
        {
            _client = factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [Fact]
        public async Task ConnectAdSense_WithValidParams_ShouldReturnOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
            var creatorId = Guid.NewGuid();
            var request = new ConnectAdSenseRequest { Email = "test@example.com" };
            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync($"/api/Revenue/connect-adsense/{creatorId}", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthUrlResponse>(responseContent, _jsonOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.NotNull(result.AuthorizationUrl);
            Assert.Contains("oauth2/auth", result.AuthorizationUrl);
        }

        [Fact]
        public async Task ConnectAdSense_WithInvalidToken_ShouldReturnForbidden()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _invalidToken);
            var creatorId = Guid.NewGuid();
            var request = new ConnectAdSenseRequest { Email = "test@example.com" };
            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync($"/api/Revenue/connect-adsense/{creatorId}", content);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task ConnectAdSense_WithInvalidParams_ShouldReturnBadRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
            var creatorId = Guid.NewGuid();
            var request = new ConnectAdSenseRequest { Email = "" }; // E-mail vazio inválido
            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync($"/api/Revenue/connect-adsense/{creatorId}", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetRevenueSummary_WithValidParams_ShouldReturnOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
            var creatorId = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;

            // Act
            var response = await _client.GetAsync(
                $"/api/Revenue/revenue/{creatorId}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<RevenueSummary>(content, _jsonOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(creatorId, result.CreatorId);
            Assert.Equal(startDate.Date, result.StartDate);
            Assert.Equal(endDate.Date, result.EndDate);
        }

        [Fact]
        public async Task GetRevenueSummary_WithInvalidToken_ShouldReturnForbidden()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _invalidToken);
            var creatorId = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;

            // Act
            var response = await _client.GetAsync(
                $"/api/Revenue/revenue/{creatorId}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetRevenueSummary_WithNonexistentAccount_ShouldReturnNotFound()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
            var creatorId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // ID especial para teste
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;

            // Act
            var response = await _client.GetAsync(
                $"/api/Revenue/revenue/{creatorId}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetRevenueByPlatform_WithValidParams_ShouldReturnOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
            var creatorId = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;

            // Act
            var response = await _client.GetAsync(
                $"/api/Revenue/revenue/{creatorId}/by-platform?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<PlatformRevenue>>(content, _jsonOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, item => Assert.Equal(creatorId, item.CreatorId));
        }

        [Fact]
        public async Task GetRevenueByDay_WithValidParams_ShouldReturnOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
            var creatorId = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-7); // Período menor para teste
            var endDate = DateTime.Now;

            // Act
            var response = await _client.GetAsync(
                $"/api/Revenue/revenue/{creatorId}/by-day?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<DailyRevenue>>(content, _jsonOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.All(result, item => Assert.Equal(creatorId, item.CreatorId));
            Assert.All(result, item => Assert.True(item.Date >= startDate.Date && item.Date <= endDate.Date));
        }

        [Fact]
        public async Task GetMonetizationMetrics_WithValidParams_ShouldReturnOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
            var creatorId = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;

            // Act
            var response = await _client.GetAsync(
                $"/api/Revenue/monetization/{creatorId}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<MonetizationMetricsDto>(content, _jsonOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.Equal(creatorId, result.CreatorId);
            Assert.NotNull(result.BestPerformingContent);
            Assert.NotEmpty(result.BestPerformingContent);
            Assert.NotNull(result.RevenueByPlatform);
            Assert.NotEmpty(result.RevenueByPlatform);
            Assert.NotNull(result.MonetizationSuggestions);
            Assert.NotEmpty(result.MonetizationSuggestions);
        }

        [Fact]
        public async Task RefreshAdSenseData_WithValidParams_ShouldReturnOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
            var creatorId = Guid.NewGuid();

            // Act
            var response = await _client.PostAsync($"/api/Revenue/adsense/refresh/{creatorId}", null);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<MessageResponse>(content, _jsonOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.NotNull(result.Message);
            Assert.Contains("sucesso", result.Message);
        }
    }

    // Classes auxiliares para deserialização das respostas
    internal class AuthUrlResponse
    {
        public string AuthorizationUrl { get; set; }
    }

    internal class MessageResponse
    {
        public string Message { get; set; }
    }
} 