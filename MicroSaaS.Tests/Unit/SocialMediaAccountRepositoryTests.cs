using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Tests.Helpers;
using Moq;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

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
        var creator = TestHelper.CreateTestContentCreator();
        var account = TestHelper.CreateTestSocialMediaAccount(creator.Id);
        _repositoryMock.Setup(x => x.GetByIdAsync(account.Id)).ReturnsAsync(account);
        var result = await _repositoryMock.Object.GetByIdAsync(account.Id);
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
        var accountId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetByIdAsync(accountId)).ReturnsAsync((SocialMediaAccount)null);
        var result = await _repositoryMock.Object.GetByIdAsync(accountId);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCreatorIdAsync_WhenAccountsExist_ShouldReturnAccounts()
    {
        var creator = TestHelper.CreateTestContentCreator();
        var accounts = new List<SocialMediaAccount>
        {
            TestHelper.CreateTestSocialMediaAccount(creator.Id),
            TestHelper.CreateTestSocialMediaAccount(creator.Id),
            TestHelper.CreateTestSocialMediaAccount(creator.Id)
        };
        _repositoryMock.Setup(x => x.GetByCreatorIdAsync(creator.Id)).ReturnsAsync(accounts);
        var result = await _repositoryMock.Object.GetByCreatorIdAsync(creator.Id);
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
        var creatorId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.GetByCreatorIdAsync(creatorId)).ReturnsAsync(new List<SocialMediaAccount>());
        var result = await _repositoryMock.Object.GetByCreatorIdAsync(creatorId);
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddAsync_ShouldAddAccount()
    {
        var creator = TestHelper.CreateTestContentCreator();
        var account = TestHelper.CreateTestSocialMediaAccount(creator.Id);
        _repositoryMock.Setup(x => x.AddAsync(account)).ReturnsAsync(account);
        var result = await _repositoryMock.Object.AddAsync(account);
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
        var creator = TestHelper.CreateTestContentCreator();
        var account = TestHelper.CreateTestSocialMediaAccount(creator.Id);
        var updatedAccount = TestHelper.CreateTestSocialMediaAccount(creator.Id);
        _repositoryMock.Setup(x => x.UpdateAsync(account)).ReturnsAsync(updatedAccount);
        var result = await _repositoryMock.Object.UpdateAsync(account);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(updatedAccount);
        _repositoryMock.Verify(x => x.UpdateAsync(account), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteAccount()
    {
        var accountId = Guid.NewGuid();
        _repositoryMock.Setup(x => x.DeleteAsync(accountId)).ReturnsAsync(true);
        var result = await _repositoryMock.Object.DeleteAsync(accountId);
        result.Should().BeTrue();
        _repositoryMock.Verify(x => x.DeleteAsync(accountId), Times.Once);
    }

    [Fact]
    public async Task UpdateTokenAsync_ShouldUpdateToken()
    {
        var creator = TestHelper.CreateTestContentCreator();
        var account = TestHelper.CreateTestSocialMediaAccount(creator.Id);
        var newAccessToken = "new_access_token";
        var newRefreshToken = "new_refresh_token";
        var newExpiresAt = DateTime.UtcNow.AddHours(1);
        _repositoryMock.Setup(x => x.UpdateTokenAsync(account.Id, newAccessToken, newRefreshToken, newExpiresAt)).Returns(Task.CompletedTask);
        await _repositoryMock.Object.UpdateTokenAsync(account.Id, newAccessToken, newRefreshToken, newExpiresAt);
        _repositoryMock.Verify(x => x.UpdateTokenAsync(account.Id, newAccessToken, newRefreshToken, newExpiresAt), Times.Once);
    }
} 