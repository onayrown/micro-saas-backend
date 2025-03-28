using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MicroSaaS.Tests.Helpers;

public static class TestHelper
{
    public static User CreateTestUser(string username = "testuser", string email = "test@example.com")
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            PasswordHash = "hashedpassword",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static ContentCreator CreateTestContentCreator(Guid? userId = null)
    {
        userId ??= Guid.NewGuid();
        
        return new ContentCreator
        {
            Id = Guid.NewGuid(),
            UserId = userId.Value,
            Name = "Test Creator",
            Username = "testcreator",
            Email = "creator@example.com",
            Bio = "Test bio",
            Niche = "Technology",
            ContentType = "Blog",
            SubscriptionPlan = CreateTestSubscriptionPlan(),
            SocialMediaAccounts = new List<SocialMediaAccount>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static Domain.Entities.SubscriptionPlan CreateTestSubscriptionPlan()
    {
        return new Domain.Entities.SubscriptionPlan
        {
            Id = Guid.NewGuid(),
            Name = "Test Plan",
            Price = 9.99m,
            MaxPosts = 10,
            IsFreePlan = false
        };
    }

    public static SocialMediaAccount CreateTestSocialMediaAccount(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
    {
        return new SocialMediaAccount
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Platform = platform,
            Username = "testaccount",
            AccessToken = "test_access_token",
            RefreshToken = "test_refresh_token",
            TokenExpiresAt = DateTime.UtcNow.AddDays(30),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static ContentPost CreateTestContentPost(Guid creatorId, PostStatus status = PostStatus.Draft)
    {
        var now = DateTime.UtcNow;
        var scheduledTime = now.Date.Add(TimeSpan.FromHours(12));

        return new ContentPost
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Title = "Test Post",
            Content = "Test content",
            MediaUrl = "https://example.com/media.jpg",
            Platform = SocialMediaPlatform.Instagram,
            Status = status,
            ScheduledTime = scheduledTime,
            ScheduledFor = now.AddDays(1),
            PublishedAt = status == PostStatus.Published ? now : null,
            PostedTime = status == PostStatus.Published ? now : null,
            CreatedAt = now,
            UpdatedAt = now
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