using FluentAssertions;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Repositories;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Tests.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace MicroSaaS.Tests.Unit;

public class SocialMediaAccountRepositoryTests
{
    private readonly Mock<ISocialMediaAccountRepository> _repositoryMock;

    public SocialMediaAccountRepositoryTests()
    {
        _repositoryMock = new Mock<ISocialMediaAccountRepository>();
    }

    [Fact]
    public async Task GetByIdAsync_WhenAccountExists_ShouldReturnAccount()
    {
        // Arrange
        var creator = TestHelper.CreateTestContentCreator();
        var account = TestHelper.CreateTestSocialMediaAccount(creator.Id);

        _repositoryMock.Setup(x => x.GetByIdAsync(account.Id))
            .ReturnsAsync(account);

        // Act
        var result = await _repositoryMock.Object.GetByIdAsync(account.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(account.Id);
        result.CreatorId.Should().Be(creator.Id);
        result.Platform.Should().Be(account.Platform);
        result.Username.Should().NotBeNullOrEmpty();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNull();
        result.TokenExpiresAt.Should().Be(account.TokenExpiresAt);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetByIdAsync_WhenAccountDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        _repositoryMock.Setup(x => x.GetByIdAsync(accountId))
            .ReturnsAsync((SocialMediaAccount)null);

        // Act
        var result = await _repositoryMock.Object.GetByIdAsync(accountId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCreatorIdAsync_WhenAccountsExist_ShouldReturnAccounts()
    {
        // Arrange
        var creator = TestHelper.CreateTestContentCreator();
        var accounts = new List<SocialMediaAccount>
        {
            TestHelper.CreateTestSocialMediaAccount(creator.Id),
            TestHelper.CreateTestSocialMediaAccount(creator.Id),
            TestHelper.CreateTestSocialMediaAccount(creator.Id)
        };

        _repositoryMock.Setup(x => x.GetByCreatorIdAsync(creator.Id))
            .ReturnsAsync(accounts);

        // Act
        var result = await _repositoryMock.Object.GetByCreatorIdAsync(creator.Id);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(accounts.Count);
        result.Should().AllSatisfy(account => 
        {
            account.Should().NotBeNull();
            account.CreatorId.Should().Be(creator.Id);
            account.Platform.Should().Be(account.Platform);
            account.Username.Should().NotBeNullOrEmpty();
            account.AccessToken.Should().NotBeNullOrEmpty();
            account.RefreshToken.Should().NotBeNull();
            account.TokenExpiresAt.Should().Be(account.TokenExpiresAt);
            account.IsActive.Should().BeTrue();
        });
    }

    [Fact]
    public async Task GetByCreatorIdAsync_WhenNoAccountsExist_ShouldReturnEmptyList()
    {
        // Arrange
        var creatorId = Guid.NewGuid();

        _repositoryMock.Setup(x => x.GetByCreatorIdAsync(creatorId))
            .ReturnsAsync(new List<SocialMediaAccount>());

        // Act
        var result = await _repositoryMock.Object.GetByCreatorIdAsync(creatorId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByCreatorIdAndPlatformAsync_WhenAccountExists_ShouldReturnAccount()
    {
        // Arrange
        var creator = TestHelper.CreateTestContentCreator();
        var platform = SocialMediaPlatform.Instagram;
        var account = TestHelper.CreateTestSocialMediaAccount(creator.Id, platform);

        _repositoryMock.Setup(x => x.GetByCreatorIdAndPlatformAsync(creator.Id, platform))
            .ReturnsAsync(account);

        // Act
        var result = await _repositoryMock.Object.GetByCreatorIdAndPlatformAsync(creator.Id, platform);

        // Assert
        result.Should().NotBeNull();
        result.CreatorId.Should().Be(creator.Id);
        result.Platform.Should().Be(platform);
        result.Username.Should().NotBeNullOrEmpty();
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNull();
        result.TokenExpiresAt.Should().Be(account.TokenExpiresAt);
        result.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetByCreatorIdAndPlatformAsync_WhenNoAccountExists_ShouldReturnNull()
    {
        // Arrange
        var creatorId = Guid.NewGuid();
        var platform = SocialMediaPlatform.Instagram;

        _repositoryMock.Setup(x => x.GetByCreatorIdAndPlatformAsync(creatorId, platform))
            .ReturnsAsync((SocialMediaAccount)null);

        // Act
        var result = await _repositoryMock.Object.GetByCreatorIdAndPlatformAsync(creatorId, platform);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldAddAccount()
    {
        // Arrange
        var creator = TestHelper.CreateTestContentCreator();
        var account = TestHelper.CreateTestSocialMediaAccount(creator.Id);

        _repositoryMock.Setup(x => x.AddAsync(account))
            .ReturnsAsync(account);

        // Act
        var result = await _repositoryMock.Object.AddAsync(account);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(account.Id);
        result.CreatorId.Should().Be(account.CreatorId);
        result.Platform.Should().Be(account.Platform);
        result.Username.Should().Be(account.Username);
        result.AccessToken.Should().Be(account.AccessToken);
        result.RefreshToken.Should().Be(account.RefreshToken);
        result.TokenExpiresAt.Should().Be(account.TokenExpiresAt);
        result.IsActive.Should().Be(account.IsActive);

        _repositoryMock.Verify(x => x.AddAsync(account), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateAccount()
    {
        // Arrange
        var creator = TestHelper.CreateTestContentCreator();
        var account = TestHelper.CreateTestSocialMediaAccount(creator.Id);
        var updatedAccount = TestHelper.CreateTestSocialMediaAccount(creator.Id);

        _repositoryMock.Setup(x => x.UpdateAsync(account))
            .ReturnsAsync(updatedAccount);

        // Act
        var result = await _repositoryMock.Object.UpdateAsync(account);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(updatedAccount);
        _repositoryMock.Verify(x => x.UpdateAsync(account), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteAccount()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        _repositoryMock.Setup(x => x.DeleteAsync(accountId))
            .ReturnsAsync(true);

        // Act
        var result = await _repositoryMock.Object.DeleteAsync(accountId);

        // Assert
        result.Should().BeTrue();
        _repositoryMock.Verify(x => x.DeleteAsync(accountId), Times.Once);
    }

    [Fact]
    public async Task UpdateTokenAsync_ShouldUpdateToken()
    {
        // Arrange
        var creator = TestHelper.CreateTestContentCreator();
        var account = TestHelper.CreateTestSocialMediaAccount(creator.Id);
        var newAccessToken = "new_access_token";
        var newRefreshToken = "new_refresh_token";
        var newExpiresAt = DateTime.UtcNow.AddHours(1);

        _repositoryMock.Setup(x => x.UpdateTokenAsync(account.Id, newAccessToken, newRefreshToken, newExpiresAt))
            .Returns(Task.CompletedTask);

        // Act
        await _repositoryMock.Object.UpdateTokenAsync(account.Id, newAccessToken, newRefreshToken, newExpiresAt);

        // Assert
        _repositoryMock.Verify(x => x.UpdateTokenAsync(account.Id, newAccessToken, newRefreshToken, newExpiresAt), Times.Once);
    }
} 