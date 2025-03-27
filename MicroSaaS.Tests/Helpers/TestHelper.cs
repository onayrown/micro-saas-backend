using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Tests.Helpers;

public static class TestHelper
{
    public static User CreateTestUser(
        string? name = null,
        string? email = null,
        string? passwordHash = null,
        bool isActive = true)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = name ?? "testuser",
            Email = email ?? "test@example.com",
            PasswordHash = passwordHash ?? "hashedpassword123",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = isActive
        };
    }

    public static ContentCreator CreateTestContentCreator(
        string? name = null,
        string? email = null,
        string? username = null,
        Domain.Entities.SubscriptionPlan? subscriptionPlan = null)
    {
        return new ContentCreator
        {
            Id = Guid.NewGuid(),
            Name = name ?? "Test Creator",
            Email = email ?? "creator@example.com",
            Username = username ?? "testcreator",
            Bio = "Test bio",
            Niche = "Test niche",
            SocialMediaAccounts = new List<SocialMediaAccount>(),
            SubscriptionPlan = subscriptionPlan ?? new Domain.Entities.SubscriptionPlan
            {
                Id = Guid.NewGuid(),
                Name = Shared.Enums.SubscriptionPlan.Free.ToString(),
                Price = 0,
                MaxPosts = 10,
                IsFreePlan = true
            }
        };
    }

    public static SocialMediaAccount CreateTestSocialMediaAccount(
        Guid creatorId,
        SocialMediaPlatform? platform = null)
    {
        return new SocialMediaAccount
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Platform = platform ?? SocialMediaPlatform.Instagram,
            Username = "testaccount",
            AccessToken = "test_access_token",
            RefreshToken = "test_refresh_token",
            TokenExpiresAt = DateTime.UtcNow.AddHours(1),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static ContentPost CreateTestContentPost(
        Guid creatorId,
        string? title = null,
        string? content = null,
        PostStatus status = PostStatus.Draft)
    {
        var creator = CreateTestContentCreator();
        
        return new ContentPost
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Creator = creator,
            Title = title ?? "Test Post",
            Content = content ?? "Test content",
            MediaUrl = "https://example.com/test.jpg",
            Platform = SocialMediaPlatform.Instagram,
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ScheduledTime = DateTime.UtcNow.AddDays(1),
            PublishedAt = status == PostStatus.Published ? DateTime.UtcNow : null
        };
    }

    public static ContentChecklist CreateTestContentChecklist(
        Guid creatorId,
        string? title = null,
        ChecklistStatus status = ChecklistStatus.InProgress)
    {
        var creator = CreateTestContentCreator();
        
        return new ContentChecklist
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Creator = creator,
            Title = title ?? "Test Checklist",
            Description = "Test checklist description",
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Items = new List<ChecklistItem>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Test item 1",
                    Description = "Test item 1 description",
                    IsCompleted = false
                },
                new()
                {
                    Id = Guid.NewGuid(),
                    Title = "Test item 2",
                    Description = "Test item 2 description",
                    IsCompleted = true
                }
            }
        };
    }
} 