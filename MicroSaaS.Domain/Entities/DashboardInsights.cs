using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MicroSaaS.Domain.Entities;

public class DashboardInsights
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("creatorId")]
    [BsonRepresentation(BsonType.String)]
    public Guid CreatorId { get; set; }

    [BsonElement("generatedDate")]
    public DateTime GeneratedDate { get; set; }

    [BsonElement("platforms")]
    public List<SocialMediaPlatform> Platforms { get; set; } = new();

    [BsonElement("periodStart")]
    public DateTime PeriodStart { get; set; }

    [BsonElement("periodEnd")]
    public DateTime PeriodEnd { get; set; }

    [BsonElement("growthRate")]
    public decimal GrowthRate { get; set; }

    [BsonElement("totalRevenueInPeriod")]
    public decimal TotalRevenueInPeriod { get; set; }

    [BsonElement("comparisonWithPreviousPeriod")]
    public decimal ComparisonWithPreviousPeriod { get; set; }

    [BsonElement("topContentInsights")]
    public List<ContentInsight> TopContentInsights { get; set; } = new();

    [BsonElement("recommendations")]
    public List<ContentRecommendation> Recommendations { get; set; } = new();

    [BsonElement("bestTimeToPost")]
    public List<PostTimeRecommendation> BestTimeToPost { get; set; } = new();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("totalFollowers")]
    public int TotalFollowers { get; set; }

    [BsonElement("totalPosts")]
    public int TotalPosts { get; set; }

    [BsonElement("totalViews")]
    public int TotalViews { get; set; }

    [BsonElement("totalLikes")]
    public int TotalLikes { get; set; }

    [BsonElement("totalComments")]
    public int TotalComments { get; set; }

    [BsonElement("totalShares")]
    public int TotalShares { get; set; }

    [BsonElement("averageEngagementRate")]
    public decimal AverageEngagementRate { get; set; }

    [BsonElement("totalRevenue")]
    public decimal TotalRevenue { get; set; }

    [BsonElement("type")]
    public InsightType Type { get; set; }

    [BsonElement("insights")]
    public List<ContentInsight> Insights { get; set; } = new();
} 