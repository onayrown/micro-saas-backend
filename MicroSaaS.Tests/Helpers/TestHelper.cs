using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Tests.Helpers;

public static class TestHelper
{
    public static User CreateTestUser(
        string? username = null,
        string? email = null,
        string? passwordHash = null,
        bool isActive = true)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = username ?? "testuser",
            Email = email ?? "test@example.com",
            PasswordHash = passwordHash ?? "hashedpassword123",
            CreatedAt = DateTime.UtcNow,
            IsActive = isActive
        };
    }

    public static ContentCreator CreateTestContentCreator(
        string? name = null,
        string? email = null,
        string? username = null,
        SubscriptionPlan? subscriptionPlan = null)
    {
        return new ContentCreator
        {
            Id = Guid.NewGuid(),
            Name = name ?? "Test Creator",
            Email = email ?? "creator@example.com",
            Username = username ?? "testcreator",
            SocialMediaAccounts = new List<SocialMediaAccount>(),
            SubscriptionPlan = subscriptionPlan ?? new SubscriptionPlan
            {
                Id = Guid.NewGuid(),
                Name = "Free",
                Price = 0,
                MaxPosts = 10,
                IsFreePlan = true
            }
        };
    }

    public static SocialMediaAccount CreateTestSocialMediaAccount(
        SocialMediaPlatform platform = SocialMediaPlatform.Instagram,
        string? accountUsername = null,
        string? accessToken = null)
    {
        return new SocialMediaAccount
        {
            Id = Guid.NewGuid(),
            Platform = platform,
            AccountUsername = accountUsername ?? "testaccount",
            AccessToken = accessToken ?? "testtoken123",
            CreatedAt = DateTime.UtcNow,
            LastSynchronized = DateTime.UtcNow
        };
    }
}