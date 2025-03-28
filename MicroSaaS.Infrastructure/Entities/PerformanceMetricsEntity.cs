using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MicroSaaS.Infrastructure.Entities;

public class PerformanceMetricsEntity
{
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

    [BsonElement("followersGrowth")]
    public int FollowersGrowth { get; set; }

    [BsonElement("totalViews")]
    public long TotalViews { get; set; }

    [BsonElement("totalLikes")]
    public int TotalLikes { get; set; }

    [BsonElement("totalComments")]
    public int TotalComments { get; set; }

    [BsonElement("totalShares")]
    public int TotalShares { get; set; }

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