using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using Xunit;
using FluentAssertions;
using FluentAssertions.Numeric;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using MicroSaaS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Microsoft.AspNetCore.Mvc.Routing;

namespace MicroSaaS.IntegrationTests;

public class PublishingTests : IClassFixture<SimplifiedTestFactory>
{
    private readonly HttpClient _client;

    public PublishingTests(SimplifiedTestFactory factory)
    {
        // Usar a factory simplificada para evitar problemas
        _client = factory.CreateClient();
        
        // Configurar o token de autenticação para todos os testes
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0ZUBtaWNyb3NhYXMuY29tIiwianRpIjoiYWJjMTIzIiwiaWF0IjoxNTE2MjM5MDIyLCJuYmYiOjE1MTYyMzkwMjIsImV4cCI6MjUxNjIzOTAyMiwiYXVkIjoibWljcm9zYWFzLmNvbSIsImlzcyI6Im1pY3Jvc2Fhcy5jb20ifQ.VY6JUOK9gH3AQJl0KEhHYQ5MURKVc18WA5qmVxpWHaE";
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task PublishNow_WithValidData_ShouldReturnOkAndPublishPost()
    {
        // Arrange
        var creatorId = await GetCreatorId();
        creatorId.Should().NotBeEmpty("a creator ID is required for this test");

        var publishRequest = new PublishNowDto
        {
            CreatorId = creatorId,
            Title = "Publicação de Teste Imediata",
            Content = "Este é um teste de publicação imediata via API de integração.",
            Platform = SocialMediaPlatform.Twitter,
            MediaUrls = new List<string>(),
            Tags = new List<string> { "teste", "integracao", "publicacao" }
        };

        var json = JsonSerializer.Serialize(publishRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/publishing/publish-now", content);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<PublishedPostDto>>(responseContent, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().NotBeEmpty();
        result.Data.Status.Should().Be(PostStatus.Published);
        result.Data.Title.Should().Be(publishRequest.Title);
        result.Data.PublishedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(5));
    }

    [Fact]
    public async Task GetPublishedPosts_WithValidToken_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/publishing/history");

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        var posts = JsonSerializer.Deserialize<List<PublishedPostDto>>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        posts.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPublishingStats_WithValidToken_ShouldReturnStats()
    {
        // Arrange
        var creatorId = await GetCreatorId();
        creatorId.Should().NotBeEmpty("a creator ID is required for this test");

        // Act
        var response = await _client.GetAsync($"/api/v1/publishing/stats?creatorId={creatorId}");

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var stats = JsonSerializer.Deserialize<ApiResponse<PublishingStatsDto>>(responseContent, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        stats.Should().NotBeNull();
        stats!.Success.Should().BeTrue();
        stats.Data.Should().NotBeNull();
        stats.Data!.TotalPublished.Should().BeGreaterThanOrEqualTo(0);
        stats.Data.PublishedByPlatform.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPostEngagement_WithValidPostId_ShouldReturnEngagementData()
    {
        // Arrange
        var creatorId = await GetCreatorId();
        creatorId.Should().NotBeEmpty("a creator ID is required for this test");

        var postId = await PublishTestPost(creatorId);
        postId.Should().NotBeEmpty("a valid post ID is required for this test");

        // Act
        var response = await _client.GetAsync($"/api/v1/publishing/engagement/{postId}");

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var engagement = JsonSerializer.Deserialize<ApiResponse<PostEngagementDto>>(responseContent, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        engagement.Should().NotBeNull();
        engagement!.Success.Should().BeTrue();
        engagement.Data.Should().NotBeNull();
        engagement.Data!.PostId.Should().Be(postId);
        engagement.Data.Likes.Should().BeGreaterThanOrEqualTo(0);
        engagement.Data.Comments.Should().BeGreaterThanOrEqualTo(0);
        engagement.Data.Shares.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task RepublishPost_WithValidId_ShouldReturnOkAndRepublishPost()
    {
        // Arrange
        var creatorId = await GetCreatorId();
        var postId = await PublishTestPost(creatorId);

        var republishRequest = new RepublishPostDto
        {
            PostId = postId,
            Platform = SocialMediaPlatform.Facebook,
            AdditionalText = "Republicando este post com algumas atualizações",
            AdditionalTags = new List<string> { "republicado", "atualizado" }
        };

        var json = JsonSerializer.Serialize(republishRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/publishing/republish", content);

        // Assert
        response.EnsureSuccessStatusCode();
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<ApiResponse<PublishedPostDto>>(responseContent, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().NotBeEmpty();
        result.Data.Id.Should().NotBe(postId); // Should be a new post ID
        result.Data.Status.Should().Be(PostStatus.Published);
        result.Data.IsRepost.Should().BeTrue();
        result.Data.OriginalPostId.Should().Be(postId);
    }

    private async Task<Guid> GetCreatorId()
    {
        // Usar o ID fixo do criador de teste definido em TestCreatorController
        return Guid.Parse("11111111-1111-1111-1111-111111111111");
    }

    private async Task<Guid> PublishTestPost(Guid creatorId)
    {
        if (creatorId == Guid.Empty)
            throw new InvalidOperationException("Valid creator ID is required to create a test post");
        
        var publishRequest = new PublishNowDto
        {
            CreatorId = creatorId,
            Title = "Post de Teste para Republicação",
            Content = "Este é um post de teste criado para teste de republicação.",
            Platform = SocialMediaPlatform.Facebook,
            MediaUrls = new List<string>(),
            Tags = new List<string> { "teste", "integracao" }
        };

        var json = JsonSerializer.Serialize(publishRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _client.PostAsync("/api/v1/publishing/publish-now", content);
        
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException("Failed to create test post for integration test");
            
        var responseContent = await response.Content.ReadAsStringAsync();
        var createdPost = JsonSerializer.Deserialize<ApiResponse<PublishedPostDto>>(responseContent, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
        return createdPost?.Data?.Id ?? Guid.Empty;
    }
} 