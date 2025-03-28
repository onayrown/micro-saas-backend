using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MicroSaaS.Infrastructure.Entities;

public class DashboardInsightsEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("creatorId")]
    [BsonRepresentation(BsonType.String)]
    public Guid CreatorId { get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("periodStart")]
    public DateTime PeriodStart { get; set; }

    [BsonElement("periodEnd")]
    public DateTime PeriodEnd { get; set; }

    [BsonElement("generatedDate")]
    public DateTime GeneratedDate { get; set; }

    [BsonElement("platforms")]
    public List<SocialMediaPlatform> Platforms { get; set; } = new();

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

    [BsonElement("totalRevenueInPeriod")]
    public decimal TotalRevenueInPeriod { get; set; }

    [BsonElement("growthRate")]
    public decimal GrowthRate { get; set; }

    [BsonElement("type")]
    public InsightType Type { get; set; }

    [BsonElement("comparisonWithPreviousPeriod")]
    public Dictionary<string, decimal> ComparisonWithPreviousPeriod { get; set; } = new();

    [BsonElement("topContentInsights")]
    public List<ContentInsightEntity> TopContentInsights { get; set; } = new();

    [BsonElement("bestTimeToPost")]
    public List<PostTimeRecommendationEntity> BestTimeToPost { get; set; } = new();

    [BsonElement("recommendations")]
    public List<ContentRecommendationEntity> Recommendations { get; set; } = new();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("insights")]
    public List<ContentInsightEntity> Insights { get; set; } = new();
}

public class ContentInsightEntity
{
    [BsonElement("type")]
    public InsightType Type { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("metrics")]
    public Dictionary<string, double> Metrics { get; set; } = new();

    [BsonElement("trend")]
    public TrendDirection Trend { get; set; }

    [BsonElement("comparisonPeriod")]
    public string ComparisonPeriod { get; set; } = string.Empty;

    [BsonElement("percentageChange")]
    public double PercentageChange { get; set; }
}

public class PostTimeRecommendationEntity
{
    [BsonElement("dayOfWeek")]
    public DayOfWeek DayOfWeek { get; set; }

    [BsonElement("timeOfDay")]
    public TimeSpan TimeOfDay { get; set; }

    [BsonElement("engagementScore")]
    public double EngagementScore { get; set; }
} 