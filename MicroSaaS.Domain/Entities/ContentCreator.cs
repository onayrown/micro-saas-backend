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
    public AdSenseSettings? AdSenseSettings { get; set; }
}

public class AdSenseSettings
{
    public bool IsConnected { get; set; } = false;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime TokenExpiresAt { get; set; }
    public string AccountId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTime ConnectedAt { get; set; }
    public DateTime LastUpdated { get; set; }
    public decimal EstimatedMonthlyRevenue { get; set; }
    public int TotalClicks { get; set; }
    public int TotalImpressions { get; set; }
    public decimal Ctr { get; set; } // Click-through rate
    public decimal Rpm { get; set; } // Revenue per mille (thousand impressions)
}
