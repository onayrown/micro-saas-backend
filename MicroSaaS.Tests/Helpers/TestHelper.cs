using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using MongoDB.Bson; // Para ObjectId

namespace MicroSaaS.Tests.Helpers;

public static class TestHelper
{
    public static User CreateTestUser(string id = null, string username = "testuser", string email = "test@example.com")
    {
        Guid userId;
        
        if (id != null && Guid.TryParse(id, out Guid parsedId))
        {
            userId = parsedId;
        }
        else
        {
            userId = Guid.NewGuid();
        }
        
        return new User
        {
            Id = userId,
            Username = username,
            Name = "Test User",
            Email = email,
            PasswordHash = "hashedpassword", // Senha hash fictícia
            Role = "user",
            IsActive = true,
            CreatedAt = DateTime.UtcNow.AddDays(-7),
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static ContentCreator CreateTestContentCreator(string? userId = null)
    {
        // Correção CS0019: Converter string? para Guid
        Guid creatorGuid;
        if (userId != null && Guid.TryParse(userId, out Guid parsedGuid))
        {
            creatorGuid = parsedGuid;
        }
        else
        {
            creatorGuid = Guid.NewGuid(); // Gerar novo Guid se userId for nulo ou inválido
        }

        return new ContentCreator
        {
            Id = Guid.NewGuid(), 
            UserId = creatorGuid, // Atribuir o Guid corrigido
            Name = "Creator Name",
            Email = "creator@example.com",
            Username = "creator_username",
            Bio = "Creator Bio",
            ProfileImageUrl = "http://example.com/profile.jpg",
            WebsiteUrl = "http://example.com",
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            IsActive = true,
            Niche = "Technology",
            ContentType = "Videos",
            SocialMediaAccounts = new List<SocialMediaAccount>(),
            Posts = new List<ContentPost>()
        };
    }

    public static ContentPost CreateTestContentPost(Guid creatorId, PostStatus status = PostStatus.Draft)
    {
        return new ContentPost
        {
            Id = Guid.NewGuid(), 
            CreatorId = creatorId, 
            Title = "Test Post Title",
            Content = "This is the content of the test post.",
            MediaUrl = "http://example.com/media.mp4",
            Platform = SocialMediaPlatform.YouTube,
            Status = status,
            ScheduledTime = null,
            PublishedAt = null,
            Views = 0,
            Likes = 0,
            Comments = 0,
            Shares = 0,
            EngagementRate = 0m,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow
        };
    }

    public static SocialMediaAccount CreateTestSocialMediaAccount(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
    {
        return new SocialMediaAccount
        {
            Id = Guid.NewGuid(), // SocialMediaAccount ID continua Guid
            CreatorId = creatorId, // CreatorId continua Guid
            Platform = platform,
            Username = $"{platform.ToString().ToLower()}_user",
            AccessToken = "dummy_access_token",
            RefreshToken = "dummy_refresh_token",
            TokenExpiresAt = DateTime.UtcNow.AddHours(1),
            IsActive = true,
            FollowersCount = 1000,
            ProfileUrl = $"http://{platform}.com/user",
            ProfileImageUrl = $"http://{platform}.com/user/pic.jpg",
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-5)
        };
    }

    public static ContentChecklist CreateTestContentChecklist(string? creatorId = null, string title = "Test Checklist")
    {
        var guidCreatorId = creatorId != null ? Guid.Parse(creatorId) : Guid.NewGuid();
        return new ContentChecklist
        {
            Id = Guid.NewGuid(), 
            CreatorId = guidCreatorId, 
            Title = title,
            Description = "Test checklist description",
            Status = ChecklistStatus.InProgress,
            Items = new List<ChecklistItem> { CreateTestChecklistItem(), CreateTestChecklistItem("Item 2") },
            CreatedAt = DateTime.UtcNow.AddDays(-3),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
    }

    public static ChecklistItem CreateTestChecklistItem(string title = "Test Item 1", string description = "Item description")
    {
        return new ChecklistItem
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            IsCompleted = false,
            IsRequired = false,
            Order = 1,
            CreatedAt = DateTime.UtcNow.AddHours(-5)
        };
    }
} 