using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroSaaS.Application.DTOs.Auth;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using MongoDB.Driver;
using Moq;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;

namespace MicroSaaS.IntegrationTests.Controllers;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ITestOutputHelper _output;

    public AuthControllerTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _output = output;
        _output.WriteLine("Initializing AuthControllerTests");
        
        // Cria um cliente HTTP com configurações específicas para teste
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            HandleCookies = true,
            MaxAutomaticRedirections = 0
        });
        
        _client.DefaultRequestHeaders.Accept.Clear();
        _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        _output.WriteLine("HttpClient initialized");
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        _output.WriteLine("Starting test: Login_WithValidCredentials_ShouldReturnToken");
        
        // Arrange
        var loginRequest = new LoginRequest { Email = "test@example.com", Password = "Test@123" };
        var jsonContent = JsonSerializer.Serialize(loginRequest);
        _output.WriteLine($"Request content: {jsonContent}");
        
        var content = new StringContent(
            jsonContent,
            Encoding.UTF8,
            "application/json");

        // Act
        _output.WriteLine("Sending request to /api/auth/login");
        var response = await _client.PostAsync("/api/auth/login", content);
        
        // Log da resposta para debug
        _output.WriteLine($"Response status code: {response.StatusCode}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response body: {responseBody}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        if (response.IsSuccessStatusCode)
        {
            var deserializeOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var authResponse = JsonSerializer.Deserialize<AuthResponse>(responseBody, deserializeOptions);

            Assert.NotNull(authResponse);
            Assert.True(authResponse.Success);
            Assert.NotNull(authResponse.Token);
            Assert.NotNull(authResponse.User);
            Assert.Equal("test@example.com", authResponse.User.Email);
        }
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        _output.WriteLine("Starting test: Login_WithInvalidCredentials_ShouldReturnUnauthorized");
        
        // Arrange
        var loginRequest = new LoginRequest { Email = "invalid@example.com", Password = "WrongPassword" };
        var jsonContent = JsonSerializer.Serialize(loginRequest);
        _output.WriteLine($"Request content: {jsonContent}");
        
        var content = new StringContent(
            jsonContent,
            Encoding.UTF8,
            "application/json");

        // Act
        _output.WriteLine("Sending request to /api/auth/login");
        var response = await _client.PostAsync("/api/auth/login", content);
        
        // Log da resposta para debug
        _output.WriteLine($"Response status code: {response.StatusCode}");
        var responseBody = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response body: {responseBody}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
} 