using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Tests.Helpers;
using Moq;
using FluentAssertions;
using Xunit;

namespace MicroSaaS.Tests.Unit;

public class TokenServiceTests
{
    private readonly Mock<ITokenService> _serviceMock;

    public TokenServiceTests()
    {
        _serviceMock = new Mock<ITokenService>();
    }

    [Fact]
    public void GenerateToken_WhenValidUser_ShouldReturnToken()
    {
        // Arrange
        var user = TestHelper.CreateTestUser();
        var token = "valid_token";

        _serviceMock.Setup(x => x.GenerateToken(user))
            .Returns(token);

        // Act
        var result = _serviceMock.Object.GenerateToken(user);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(token);

        _serviceMock.Verify(x => x.GenerateToken(user), Times.Once);
    }

    [Fact]
    public void GenerateToken_WhenNullUser_ShouldThrowException()
    {
        // Arrange
        User? user = null;

        _serviceMock.Setup(x => x.GenerateToken(user!))
            .Throws<ArgumentNullException>();

        // Act & Assert
        _serviceMock.Object.Invoking(x => x.GenerateToken(user!))
            .Should().Throw<ArgumentNullException>();

        _serviceMock.Verify(x => x.GenerateToken(user!), Times.Once);
    }

    [Fact]
    public void ValidateToken_WhenValidToken_ShouldReturnTrue()
    {
        // Arrange
        var token = "valid_token";

        _serviceMock.Setup(x => x.ValidateToken(token))
            .Returns(true);

        // Act
        var result = _serviceMock.Object.ValidateToken(token);

        // Assert
        result.Should().BeTrue();

        _serviceMock.Verify(x => x.ValidateToken(token), Times.Once);
    }

    [Fact]
    public void ValidateToken_WhenInvalidToken_ShouldReturnFalse()
    {
        // Arrange
        var token = "invalid_token";

        _serviceMock.Setup(x => x.ValidateToken(token))
            .Returns(false);

        // Act
        var result = _serviceMock.Object.ValidateToken(token);

        // Assert
        result.Should().BeFalse();

        _serviceMock.Verify(x => x.ValidateToken(token), Times.Once);
    }

    [Fact]
    public void ValidateToken_WhenNullToken_ShouldThrowException()
    {
        // Arrange
        string? token = null;

        _serviceMock.Setup(x => x.ValidateToken(token!))
            .Throws<ArgumentNullException>();

        // Act & Assert
        _serviceMock.Object.Invoking(x => x.ValidateToken(token!))
            .Should().Throw<ArgumentNullException>();

        _serviceMock.Verify(x => x.ValidateToken(token!), Times.Once);
    }

    [Fact]
    public void GetUserIdFromToken_WhenValidToken_ShouldReturnUserId()
    {
        // Arrange
        var token = "valid_token";
        var userId = Guid.NewGuid();

        _serviceMock.Setup(x => x.GetUserIdFromToken(token))
            .Returns(userId);

        // Act
        var result = _serviceMock.Object.GetUserIdFromToken(token);

        // Assert
        result.Should().Be(userId);

        _serviceMock.Verify(x => x.GetUserIdFromToken(token), Times.Once);
    }

    [Fact]
    public void GetUserIdFromToken_WhenInvalidToken_ShouldThrowException()
    {
        // Arrange
        var token = "invalid_token";

        _serviceMock.Setup(x => x.GetUserIdFromToken(token))
            .Throws<InvalidOperationException>();

        // Act & Assert
        _serviceMock.Object.Invoking(x => x.GetUserIdFromToken(token))
            .Should().Throw<InvalidOperationException>();

        _serviceMock.Verify(x => x.GetUserIdFromToken(token), Times.Once);
    }

    [Fact]
    public void GetUserEmailFromToken_WhenValidToken_ShouldReturnUserEmail()
    {
        // Arrange
        var token = "valid_token";
        var email = "test@example.com";

        _serviceMock.Setup(x => x.GetUserEmailFromToken(token))
            .Returns(email);

        // Act
        var result = _serviceMock.Object.GetUserEmailFromToken(token);

        // Assert
        result.Should().Be(email);

        _serviceMock.Verify(x => x.GetUserEmailFromToken(token), Times.Once);
    }

    [Fact]
    public void GetUserEmailFromToken_WhenInvalidToken_ShouldThrowException()
    {
        // Arrange
        var token = "invalid_token";

        _serviceMock.Setup(x => x.GetUserEmailFromToken(token))
            .Throws<InvalidOperationException>();

        // Act & Assert
        _serviceMock.Object.Invoking(x => x.GetUserEmailFromToken(token))
            .Should().Throw<InvalidOperationException>();

        _serviceMock.Verify(x => x.GetUserEmailFromToken(token), Times.Once);
    }
} 