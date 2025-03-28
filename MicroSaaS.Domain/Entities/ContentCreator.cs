using System;
using System.Collections.Generic;

namespace MicroSaaS.Domain.Entities;

public class ContentCreator
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;
    public string WebsiteUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Niche { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public SubscriptionPlan? SubscriptionPlan { get; set; }
    public List<SocialMediaAccount> SocialMediaAccounts { get; set; } = new List<SocialMediaAccount>();
    public List<ContentPost> Posts { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public int TotalFollowers { get; set; }
    public int TotalPosts { get; set; }
    public decimal AverageEngagementRate { get; set; }
    public decimal TotalRevenue { get; set; }
}
