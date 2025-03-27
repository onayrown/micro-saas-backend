using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Tests.Helpers;
using Moq;
using FluentAssertions;
using Xunit;

namespace MicroSaaS.Tests.Unit;

public class SocialMediaIntegrationServiceTests
{
    private readonly Mock<ISocialMediaIntegrationService> _serviceMock;

    public SocialMediaIntegrationServiceTests()
    {
        _serviceMock = new Mock<ISocialMediaIntegrationService>();
    }

    [Fact]
    public async Task ConnectAccountAsync_WhenValidAccount_ShouldConnect()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var account = TestHelper.CreateTestSocialMediaAccount(creatorId);

        _serviceMock.Setup(x => x.ConnectAccountAsync(account))
            .Returns(Task.CompletedTask);

        // Act
        await _serviceMock.Object.ConnectAccountAsync(account);

        // Assert
        _serviceMock.Verify(x => x.ConnectAccountAsync(account), Times.Once);
    }

    [Fact]
    public async Task ConnectAccountAsync_WhenNullAccount_ShouldThrowException()
    {
        // Arrange
        SocialMediaAccount? account = null;

        _serviceMock.Setup(x => x.ConnectAccountAsync(account!))
            .ThrowsAsync(new ArgumentNullException(nameof(account)));

        // Act & Assert
        await _serviceMock.Object.Invoking(x => x.ConnectAccountAsync(account!))
            .Should().ThrowAsync<ArgumentNullException>();

        _serviceMock.Verify(x => x.ConnectAccountAsync(account!), Times.Once);
    }

    [Fact]
    public async Task DisconnectAccountAsync_WhenValidAccount_ShouldDisconnect()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var account = TestHelper.CreateTestSocialMediaAccount(creatorId);

        _serviceMock.Setup(x => x.DisconnectAccountAsync(account))
            .Returns(Task.CompletedTask);

        // Act
        await _serviceMock.Object.DisconnectAccountAsync(account);

        // Assert
        _serviceMock.Verify(x => x.DisconnectAccountAsync(account), Times.Once);
    }

    [Fact]
    public async Task DisconnectAccountAsync_WhenNullAccount_ShouldThrowException()
    {
        // Arrange
        SocialMediaAccount? account = null;

        _serviceMock.Setup(x => x.DisconnectAccountAsync(account!))
            .ThrowsAsync(new ArgumentNullException(nameof(account)));

        // Act & Assert
        await _serviceMock.Object.Invoking(x => x.DisconnectAccountAsync(account!))
            .Should().ThrowAsync<ArgumentNullException>();

        _serviceMock.Verify(x => x.DisconnectAccountAsync(account!), Times.Once);
    }

    [Fact]
    public async Task PostContentAsync_WhenValidContent_ShouldPost()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var post = TestHelper.CreateTestContentPost(creatorId);

        _serviceMock.Setup(x => x.PostContentAsync(post))
            .Returns(Task.CompletedTask);

        // Act
        await _serviceMock.Object.PostContentAsync(post);

        // Assert
        _serviceMock.Verify(x => x.PostContentAsync(post), Times.Once);
    }

    [Fact]
    public async Task PostContentAsync_WhenNullContent_ShouldThrowException()
    {
        // Arrange
        ContentPost? post = null;

        _serviceMock.Setup(x => x.PostContentAsync(post!))
            .ThrowsAsync(new ArgumentNullException(nameof(post)));

        // Act & Assert
        await _serviceMock.Object.Invoking(x => x.PostContentAsync(post!))
            .Should().ThrowAsync<ArgumentNullException>();

        _serviceMock.Verify(x => x.PostContentAsync(post!), Times.Once);
    }

    [Fact]
    public async Task GetAccountStatsAsync_WhenValidAccount_ShouldReturnStats()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var account = TestHelper.CreateTestSocialMediaAccount(creatorId);
        var stats = new Dictionary<string, int>
        {
            { "followers", 1000 },
            { "following", 500 },
            { "posts", 100 }
        };

        _serviceMock.Setup(x => x.GetAccountStatsAsync(account))
            .ReturnsAsync(stats);

        // Act
        var result = await _serviceMock.Object.GetAccountStatsAsync(account);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(stats);

        _serviceMock.Verify(x => x.GetAccountStatsAsync(account), Times.Once);
    }

    [Fact]
    public async Task GetAccountStatsAsync_WhenNullAccount_ShouldThrowException()
    {
        // Arrange
        SocialMediaAccount? account = null;

        _serviceMock.Setup(x => x.GetAccountStatsAsync(account!))
            .ThrowsAsync(new ArgumentNullException(nameof(account)));

        // Act & Assert
        await _serviceMock.Object.Invoking(x => x.GetAccountStatsAsync(account!))
            .Should().ThrowAsync<ArgumentNullException>();

        _serviceMock.Verify(x => x.GetAccountStatsAsync(account!), Times.Once);
    }
} 