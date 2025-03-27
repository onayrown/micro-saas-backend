using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Application.Services;
using MicroSaaS.Backend.Controllers;
using MicroSaaS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;
using System.Text.Json;

namespace MicroSaaS.Tests.Integration;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Register_WhenModelStateIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "", // Invalid username
            Email = "invalid-email", // Invalid email
            Password = "" // Invalid password
        };

        _authController.ModelState.AddModelError("Username", "Username is required");
        _authController.ModelState.AddModelError("Email", "Invalid email format");
        _authController.ModelState.AddModelError("Password", "Password is required");

        // Act
        var result = await _authController.Register(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<SerializableError>();
    }

    [Fact]
    public async Task Register_WhenRegistrationFails_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };

        _authServiceMock.Setup(x => x.RegisterAsync(
            request.Username,
            request.Email,
            request.Password))
            .ReturnsAsync(AuthenticationResult.Failure("Username already exists"));

        // Act
        var result = await _authController.Register(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        var response = JsonSerializer.Deserialize<Dictionary<string, string>>(
            JsonSerializer.Serialize(badRequestResult!.Value));
        response.Should().ContainKey("message");
        response["message"].Should().Be("Username already exists");
    }

    [Fact]
    public async Task Register_WhenRegistrationSucceeds_ShouldReturnOk()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email
        };

        var token = "jwtToken123";

        _authServiceMock.Setup(x => x.RegisterAsync(
            request.Username,
            request.Email,
            request.Password))
            .ReturnsAsync(AuthenticationResult.Success(user, token));

        // Act
        var result = await _authController.Register(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var response = JsonSerializer.Deserialize<Dictionary<string, object>>(
            JsonSerializer.Serialize(okResult!.Value));
        
        response.Should().ContainKey("user");
        response.Should().ContainKey("token");
        
        var responseUser = JsonSerializer.Deserialize<Dictionary<string, object>>(
            JsonSerializer.Serialize(response["user"]));
        
        responseUser.Should().NotBeNull();
        responseUser!["id"].ToString().Should().Be(user.Id.ToString());
        responseUser["username"].ToString().Should().Be(user.Username);
        responseUser["email"].ToString().Should().Be(user.Email);
        response["token"].ToString().Should().Be(token);
    }

    [Fact]
    public async Task Login_WhenModelStateIsInvalid_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "", // Invalid username
            Password = "" // Invalid password
        };

        _authController.ModelState.AddModelError("Username", "Username is required");
        _authController.ModelState.AddModelError("Password", "Password is required");

        // Act
        var result = await _authController.Login(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<SerializableError>();
    }

    [Fact]
    public async Task Login_WhenLoginFails_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "wrongpassword"
        };

        _authServiceMock.Setup(x => x.LoginAsync(
            request.Username,
            request.Password))
            .ReturnsAsync(AuthenticationResult.Failure("Invalid credentials"));

        // Act
        var result = await _authController.Login(request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        var response = JsonSerializer.Deserialize<Dictionary<string, string>>(
            JsonSerializer.Serialize(badRequestResult!.Value));
        response.Should().ContainKey("message");
        response["message"].Should().Be("Invalid credentials");
    }

    [Fact]
    public async Task Login_WhenLoginSucceeds_ShouldReturnOk()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "correctpassword"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = "test@example.com"
        };

        var token = "jwtToken123";

        _authServiceMock.Setup(x => x.LoginAsync(
            request.Username,
            request.Password))
            .ReturnsAsync(AuthenticationResult.Success(user, token));

        // Act
        var result = await _authController.Login(request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var response = JsonSerializer.Deserialize<Dictionary<string, object>>(
            JsonSerializer.Serialize(okResult!.Value));
        
        response.Should().ContainKey("user");
        response.Should().ContainKey("token");
        
        var responseUser = JsonSerializer.Deserialize<Dictionary<string, object>>(
            JsonSerializer.Serialize(response["user"]));
        
        responseUser.Should().NotBeNull();
        responseUser!["id"].ToString().Should().Be(user.Id.ToString());
        responseUser["username"].ToString().Should().Be(user.Username);
        responseUser["email"].ToString().Should().Be(user.Email);
        response["token"].ToString().Should().Be(token);
    }
}