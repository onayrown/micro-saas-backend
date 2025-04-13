using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MicroSaaS.Domain.Entities;

public class PerformanceMetrics
{
    public PerformanceMetrics()
    {
        Id = Guid.NewGuid().ToString();
        CreatorId = string.Empty;
        Date = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        TopPerformingContentIds = new List<string>();
    }

    public PerformanceMetrics(string creatorId, SocialMediaPlatform platform)
    {
        Id = Guid.NewGuid().ToString();
        CreatorId = creatorId;
        Platform = platform;
        Date = DateTime.UtcNow;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        TopPerformingContentIds = new List<string>();
    }

    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; }

    [BsonElement("creatorId")]
    [BsonRepresentation(BsonType.String)]
    public string CreatorId { get; set; }

    [BsonElement("platform")]
    public SocialMediaPlatform Platform { get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("followers")]
    public int Followers { get; set; }
    
    [BsonElement("followerCount")]
    public int FollowerCount { get; set; }
    
    [BsonElement("newFollowers")]
    public int NewFollowers { get; set; }

    [BsonElement("followersGrowth")]
    public int FollowersGrowth { get; set; }

    [BsonElement("totalViews")]
    public long TotalViews { get; set; }
    
    [BsonElement("views")]
    public long Views { get; set; }

    [BsonElement("totalLikes")]
    public int TotalLikes { get; set; }
    
    [BsonElement("likes")]
    public int Likes { get; set; }

    [BsonElement("totalComments")]
    public int TotalComments { get; set; }
    
    [BsonElement("comments")]
    public int Comments { get; set; }

    [BsonElement("totalShares")]
    public int TotalShares { get; set; }
    
    [BsonElement("shares")]
    public int Shares { get; set; }
    
    [BsonElement("saves")]
    public int Saves { get; set; }
    
    [BsonElement("reach")]
    public int Reach { get; set; }
    
    [BsonElement("impressions")]
    public int Impressions { get; set; }

    [BsonElement("engagementRate")]
    public decimal EngagementRate { get; set; }

    [BsonElement("estimatedRevenue")]
    public decimal EstimatedRevenue { get; set; }

    [BsonElement("topPerformingContentIds")]
    public List<string> TopPerformingContentIds { get; set; } = new();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
} 