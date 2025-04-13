using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MicroSaaS.Domain.Entities;

public class ContentCreator
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonRepresentation(BsonType.String)]
    [BsonElement("UserId")]
    public Guid UserId { get; set; }

    [BsonElement("Name")]
    public string Name { get; set; }

    [BsonElement("Email")]
    public string Email { get; set; }

    [BsonElement("Username")]
    public string Username { get; set; }

    [BsonElement("Bio")]
    public string Bio { get; set; }

    [BsonElement("ProfileImageUrl")]
    public string ProfileImageUrl { get; set; }

    [BsonElement("WebsiteUrl")]
    public string WebsiteUrl { get; set; }

    [BsonElement("CreatedAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("UpdatedAt")]
    public DateTime UpdatedAt { get; set; }

    public bool IsActive { get; set; }
    public string Niche { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public SubscriptionPlan? SubscriptionPlan { get; set; }
    public List<SocialMediaAccount> SocialMediaAccounts { get; set; } = new List<SocialMediaAccount>();
    public List<ContentPost> Posts { get; set; } = new();
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
