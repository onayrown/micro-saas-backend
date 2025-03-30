using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.IntegrationTests.Utils;
using Xunit;
using Xunit.Abstractions;
using FluentAssertions;

namespace MicroSaaS.IntegrationTests.Controllers
{
    public class AuthControllerTests : IClassFixture<SimplifiedTestFactory>
    {
        private readonly HttpClient _client;
        private readonly ITestOutputHelper _output;

        public AuthControllerTests(SimplifiedTestFactory factory, ITestOutputHelper output)
        {
            _output = output;
            _output.WriteLine("Initializing AuthControllerTests");
            _client = factory.CreateClient();
            _output.WriteLine("HttpClient initialized");
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            _output.WriteLine("Starting test: Login_WithValidCredentials_ShouldReturnToken");
            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Test@123"
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _output.WriteLine($"Request content: {json}");

            // Act
            _output.WriteLine("Sending request to /api/auth/login");
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"Response: {responseContent}");
            
            var result = JsonSerializer.Deserialize<AuthResponse>(responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            result.Should().NotBeNull();
            result!.Success.Should().BeTrue();
            result.Token.Should().NotBeNullOrEmpty();
            result.User.Should().NotBeNull();
            result.User.Email.Should().Be(loginRequest.Email);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Arrange
            _output.WriteLine("Starting test: Login_WithInvalidCredentials_ShouldReturnUnauthorized");
            var loginRequest = new LoginRequest
            {
                Email = "invalid@example.com",
                Password = "WrongPassword"
            };

            var json = JsonSerializer.Serialize(loginRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            _output.WriteLine($"Request content: {json}");

            // Act
            _output.WriteLine("Sending request to /api/auth/login");
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsSuccess()
        {
            // Arrange
            _output.WriteLine("Starting Register_WithValidData_ReturnsSuccess test");
            var registerRequest = new RegisterRequest
            {
                Name = "Test User",
                Email = "newuser@example.com",
                Password = "Test@123"
            };

            var json = JsonSerializer.Serialize(registerRequest);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            _output.WriteLine("Sending register request");
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthResponse>(responseContent, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            result.Should().NotBeNull();
            result!.Success.Should().BeTrue();
            result.Token.Should().NotBeNullOrEmpty();
            result.User.Should().NotBeNull();
            result.User.Email.Should().Be(registerRequest.Email);
        }
    }
} 