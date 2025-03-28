using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace MicroSaaS.IntegrationTests.Controllers;

public class RecommendationControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public RecommendationControllerTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine("Initializing RecommendationControllerTests");
        
        // Cria um cliente HTTP com configurações específicas para teste
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true,
            MaxAutomaticRedirections = 0
        });
        
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        // Adicionar token de autorização para todas as requisições
        // Em um caso real, este token seria obtido através de login
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "test.jwt.token");
        
        _output.WriteLine("HttpClient initialized");
    }

    [Fact]
    public async Task GetBestTimeToPost_WithValidInput_ReturnsRecommendations()
    {
        // Arrange
        _output.WriteLine("Starting GetBestTimeToPost_WithValidInput_ReturnsRecommendations test");
        
        var creatorId = Guid.NewGuid();
        var platform = SocialMediaPlatform.Instagram;
        
        // Act
        _output.WriteLine($"Sending request for creator {creatorId} and platform {platform}");
        var response = await _client.GetAsync($"/api/v1/recommendation/best-times/{creatorId}?platform={platform}");
        var responseString = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response status: {response.StatusCode}");
        _output.WriteLine($"Response content: {responseString}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var recommendations = JsonSerializer.Deserialize<List<MicroSaaS.Domain.Entities.PostTimeRecommendation>>(responseString, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(recommendations);
        Assert.NotEmpty(recommendations);
        foreach (var recommendation in recommendations)
        {
            Assert.True(recommendation.EngagementScore >= 0);
        }
    }

    [Fact]
    public async Task GetContentRecommendations_WithValidCreatorId_ReturnsRecommendations()
    {
        // Arrange
        _output.WriteLine("Starting GetContentRecommendations_WithValidCreatorId_ReturnsRecommendations test");
        
        var creatorId = Guid.NewGuid();
        
        // Act
        _output.WriteLine($"Sending request for creator {creatorId}");
        var response = await _client.GetAsync($"/api/v1/recommendation/content/{creatorId}");
        var responseString = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response status: {response.StatusCode}");
        _output.WriteLine($"Response content: {responseString}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        
        var recommendations = JsonSerializer.Deserialize<List<MicroSaaS.Domain.Entities.ContentRecommendation>>(responseString, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.NotNull(recommendations);
        Assert.NotEmpty(recommendations);
        foreach (var recommendation in recommendations)
        {
            Assert.NotEqual(Guid.Empty, recommendation.Id);
            Assert.Equal(creatorId, recommendation.CreatorId);
            Assert.NotEmpty(recommendation.Title);
            Assert.NotEmpty(recommendation.Description);
        }
    }
} 