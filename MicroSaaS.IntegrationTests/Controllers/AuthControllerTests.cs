using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using MicroSaaS.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using MongoDB.Driver;
using Moq;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;

namespace MicroSaaS.IntegrationTests.Controllers;

public class AuthControllerTests : IntegrationTestBase
{
    public AuthControllerTests(WebApplicationFactory<MicroSaaS.Backend.Program> factory) : base(factory)
    {
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var content = new StringContent(
            @"{""email"":""test@example.com"",""password"":""Test@123""}",
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/login", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var deserializeOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseString, deserializeOptions);

        Assert.NotNull(authResponse);
        Assert.True(authResponse.Success);
        Assert.NotNull(authResponse.Token);
        Assert.NotNull(authResponse.User);
        Assert.Equal("test@example.com", authResponse.User.Email);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var content = new StringContent(
            @"{""email"":""invalid@example.com"",""password"":""WrongPassword""}",
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await _client.PostAsync("/api/auth/login", content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
} 