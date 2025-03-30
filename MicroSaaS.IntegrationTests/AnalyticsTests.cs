using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroSaaS.Application.DTOs;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;

namespace MicroSaaS.IntegrationTests
{
    public class AnalyticsTests : IClassFixture<SimplifiedTestFactory>
    {
        private readonly HttpClient _client;
        private readonly string _validToken = "valid-token";
        private readonly string _invalidToken = "invalid-token";
        private readonly JsonSerializerOptions _jsonOptions;

        public AnalyticsTests(SimplifiedTestFactory factory)
        {
            _client = factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        [Fact]
        public async Task GetPostAnalytics_WithValidToken_ShouldReturnOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
            string postId = "post-1";

            // Act
            var response = await _client.GetAsync($"/api/Analytics/post/{postId}");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ContentPerformanceDto>>(content, _jsonOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(postId, result[0].PostId);
        }

        [Fact]
        public async Task GetPostAnalytics_WithInvalidToken_ShouldReturnForbidden()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _invalidToken);
            string postId = "post-1";

            // Act
            var response = await _client.GetAsync($"/api/Analytics/post/{postId}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetPostAnalytics_WithInvalidPostId_ShouldReturnBadRequest()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
            string postId = "invalid-id";

            // Act
            var response = await _client.GetAsync($"/api/Analytics/post/{postId}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetCreatorAnalytics_WithValidParameters_ShouldReturnOk()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
            var creatorId = Guid.NewGuid();
            var platform = SocialMediaPlatform.Instagram;
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;

            // Act
            var response = await _client.GetAsync(
                $"/api/Analytics/creator/{creatorId}?platform={platform}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ContentPerformanceDto>>(content, _jsonOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.All(result, item => Assert.Equal(platform, item.Platform));
            Assert.All(result, item => Assert.True(item.Date >= startDate && item.Date <= endDate));
        }

        [Fact]
        public async Task GetCreatorAnalytics_WithInvalidToken_ShouldReturnForbidden()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _invalidToken);
            var creatorId = Guid.NewGuid();
            var platform = SocialMediaPlatform.Instagram;
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;

            // Act
            var response = await _client.GetAsync(
                $"/api/Analytics/creator/{creatorId}?platform={platform}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GetCreatorAnalytics_WithNonexistentAccount_ShouldReturnNotFound()
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
            var creatorId = Guid.Parse("00000000-0000-0000-0000-000000000001"); // ID especial para teste
            var platform = SocialMediaPlatform.Instagram;
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;

            // Act
            var response = await _client.GetAsync(
                $"/api/Analytics/creator/{creatorId}?platform={platform}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData(SocialMediaPlatform.Instagram)]
        [InlineData(SocialMediaPlatform.YouTube)]
        [InlineData(SocialMediaPlatform.TikTok)]
        public async Task GetCreatorAnalytics_WithDifferentPlatforms_ShouldReturnCorrectPlatformData(SocialMediaPlatform platform)
        {
            // Arrange
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
            var creatorId = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;

            // Act
            var response = await _client.GetAsync(
                $"/api/Analytics/creator/{creatorId}?platform={platform}&startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<List<ContentPerformanceDto>>(content, _jsonOptions);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(result);
            Assert.All(result, item => Assert.Equal(platform, item.Platform));
        }
    }
} 