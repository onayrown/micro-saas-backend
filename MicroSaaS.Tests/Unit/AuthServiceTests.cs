using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Application.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Interfaces;
using Moq;
using FluentAssertions;
using Xunit;

namespace MicroSaaS.Tests.Unit;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();
        _authService = new AuthService(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WhenUsernameExists_ShouldReturnFailure()
    {
        // Arrange
        var username = "testuser";
        var email = "test@example.com";
        var password = "password123";

        _userRepositoryMock.Setup(x => x.CheckUsernameExistsAsync(username))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(username, email, password);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Username already exists");
    }

    [Fact]
    public async Task RegisterAsync_WhenEmailExists_ShouldReturnFailure()
    {
        // Arrange
        var username = "testuser";
        var email = "test@example.com";
        var password = "password123";

        _userRepositoryMock.Setup(x => x.CheckUsernameExistsAsync(username))
            .ReturnsAsync(false);

        _userRepositoryMock.Setup(x => x.CheckEmailExistsAsync(email))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.RegisterAsync(username, email, password);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Email already exists");
    }

    [Fact]
    public async Task RegisterAsync_WhenValidData_ShouldReturnSuccess()
    {
        // Arrange
        var username = "testuser";
        var email = "test@example.com";
        var password = "password123";
        var hashedPassword = "hashedpassword123";
        var token = "jwtToken123";

        _userRepositoryMock.Setup(x => x.CheckUsernameExistsAsync(username))
            .ReturnsAsync(false);

        _userRepositoryMock.Setup(x => x.CheckEmailExistsAsync(email))
            .ReturnsAsync(false);

        _passwordHasherMock.Setup(x => x.HashPassword(password))
            .Returns(hashedPassword);

        _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>()))
            .Returns(token);

        // Act
        var result = await _authService.RegisterAsync(username, email, password);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.User.Should().NotBeNull();
        result.User!.Username.Should().Be(username);
        result.User.Email.Should().Be(email);
        result.User.PasswordHash.Should().Be(hashedPassword);
        result.Token.Should().Be(token);
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFound_ShouldReturnFailure()
    {
        // Arrange
        var username = "testuser";
        var password = "password123";

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(username))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(username, password);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("User not found");
    }

    [Fact]
    public async Task LoginAsync_WhenInvalidPassword_ShouldReturnFailure()
    {
        // Arrange
        var username = "testuser";
        var password = "wrongpassword";
        var hashedPassword = "hashedpassword123";

        var user = new User
        {
            Username = username,
            PasswordHash = hashedPassword
        };

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(username))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(x => x.VerifyPassword(password, hashedPassword))
            .Returns(false);

        // Act
        var result = await _authService.LoginAsync(username, password);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Invalid password");
    }

    [Fact]
    public async Task LoginAsync_WhenValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var username = "testuser";
        var password = "correctpassword";
        var hashedPassword = "hashedpassword123";
        var token = "jwtToken123";

        var user = new User
        {
            Username = username,
            PasswordHash = hashedPassword
        };

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(username))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(x => x.VerifyPassword(password, hashedPassword))
            .Returns(true);

        _tokenServiceMock.Setup(x => x.GenerateToken(user))
            .Returns(token);

        // Act
        var result = await _authService.LoginAsync(username, password);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.User.Should().NotBeNull();
        result.User!.Username.Should().Be(username);
        result.Token.Should().Be(token);
    }
}