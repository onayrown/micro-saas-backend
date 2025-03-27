using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Tests.Helpers;
using Moq;
using FluentAssertions;
using Xunit;

namespace MicroSaaS.Tests.Unit;

public class PasswordHasherTests
{
    private readonly Mock<IPasswordHasher> _serviceMock;

    public PasswordHasherTests()
    {
        _serviceMock = new Mock<IPasswordHasher>();
    }

    [Fact]
    public void HashPassword_WhenValidPassword_ShouldReturnHash()
    {
        // Arrange
        var password = "Test@123";
        var hash = "hashed_password";

        _serviceMock.Setup(x => x.HashPassword(password))
            .Returns(hash);

        // Act
        var result = _serviceMock.Object.HashPassword(password);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Be(hash);

        _serviceMock.Verify(x => x.HashPassword(password), Times.Once);
    }

    [Fact]
    public void HashPassword_WhenNullPassword_ShouldThrowException()
    {
        // Arrange
        string? password = null;

        _serviceMock.Setup(x => x.HashPassword(password!))
            .Throws<ArgumentNullException>();

        // Act & Assert
        _serviceMock.Object.Invoking(x => x.HashPassword(password!))
            .Should().Throw<ArgumentNullException>();

        _serviceMock.Verify(x => x.HashPassword(password!), Times.Once);
    }

    [Fact]
    public void HashPassword_WhenEmptyPassword_ShouldThrowException()
    {
        // Arrange
        var password = "";

        _serviceMock.Setup(x => x.HashPassword(password))
            .Throws<ArgumentException>();

        // Act & Assert
        _serviceMock.Object.Invoking(x => x.HashPassword(password))
            .Should().Throw<ArgumentException>();

        _serviceMock.Verify(x => x.HashPassword(password), Times.Once);
    }

    [Fact]
    public void VerifyPassword_WhenValidPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "Test@123";
        var hash = "hashed_password";

        _serviceMock.Setup(x => x.VerifyPassword(password, hash))
            .Returns(true);

        // Act
        var result = _serviceMock.Object.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();

        _serviceMock.Verify(x => x.VerifyPassword(password, hash), Times.Once);
    }

    [Fact]
    public void VerifyPassword_WhenInvalidPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "WrongPassword";
        var hash = "hashed_password";

        _serviceMock.Setup(x => x.VerifyPassword(password, hash))
            .Returns(false);

        // Act
        var result = _serviceMock.Object.VerifyPassword(password, hash);

        // Assert
        result.Should().BeFalse();

        _serviceMock.Verify(x => x.VerifyPassword(password, hash), Times.Once);
    }

    [Fact]
    public void VerifyPassword_WhenNullPassword_ShouldThrowException()
    {
        // Arrange
        string? password = null;
        var hash = "hashed_password";

        _serviceMock.Setup(x => x.VerifyPassword(password!, hash))
            .Throws<ArgumentNullException>();

        // Act & Assert
        _serviceMock.Object.Invoking(x => x.VerifyPassword(password!, hash))
            .Should().Throw<ArgumentNullException>();

        _serviceMock.Verify(x => x.VerifyPassword(password!, hash), Times.Once);
    }

    [Fact]
    public void VerifyPassword_WhenEmptyPassword_ShouldThrowException()
    {
        // Arrange
        var password = "";
        var hash = "hashed_password";

        _serviceMock.Setup(x => x.VerifyPassword(password, hash))
            .Throws<ArgumentException>();

        // Act & Assert
        _serviceMock.Object.Invoking(x => x.VerifyPassword(password, hash))
            .Should().Throw<ArgumentException>();

        _serviceMock.Verify(x => x.VerifyPassword(password, hash), Times.Once);
    }

    [Fact]
    public void VerifyPassword_WhenNullHash_ShouldThrowException()
    {
        // Arrange
        var password = "Test@123";
        string? hash = null;

        _serviceMock.Setup(x => x.VerifyPassword(password, hash!))
            .Throws<ArgumentNullException>();

        // Act & Assert
        _serviceMock.Object.Invoking(x => x.VerifyPassword(password, hash!))
            .Should().Throw<ArgumentNullException>();

        _serviceMock.Verify(x => x.VerifyPassword(password, hash!), Times.Once);
    }

    [Fact]
    public void VerifyPassword_WhenEmptyHash_ShouldThrowException()
    {
        // Arrange
        var password = "Test@123";
        var hash = "";

        _serviceMock.Setup(x => x.VerifyPassword(password, hash))
            .Throws<ArgumentException>();

        // Act & Assert
        _serviceMock.Object.Invoking(x => x.VerifyPassword(password, hash))
            .Should().Throw<ArgumentException>();

        _serviceMock.Verify(x => x.VerifyPassword(password, hash), Times.Once);
    }
} 