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

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("periodStart")]
    public DateTime PeriodStart { get; set; }

    [BsonElement("periodEnd")]
    public DateTime PeriodEnd { get; set; }

    [BsonElement("generatedDate")]
    public DateTime GeneratedDate { get; set; }

    [BsonElement("generatedAt")]
    public DateTime GeneratedAt { get; set; }

    [BsonElement("platforms")]
    public List<SocialMediaPlatform> Platforms { get; set; } = new();

    [BsonElement("platformsPerformance")]
    public List<PlatformInsight> PlatformsPerformance { get; set; } = new();

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

    [BsonElement("revenueGrowth")]
    public decimal RevenueGrowth { get; set; }

    [BsonElement("postingConsistency")]
    public int PostingConsistency { get; set; }

    [BsonElement("audienceDemographics")]
    public AudienceDemographics AudionceDemographics { get; set; } = new();

    [BsonElement("contentTypePerformance")]
    public Dictionary<string, decimal> ContentTypePerformance { get; set; } = new();

    [BsonElement("keyInsights")]
    public List<string> KeyInsights { get; set; } = new();

    [BsonElement("actionRecommendations")]
    public List<string> ActionRecommendations { get; set; } = new();

    [BsonElement("growthOpportunities")]
    public List<GrowthOpportunity> GrowthOpportunities { get; set; } = new();

    [BsonElement("competitorBenchmark")]
    public List<CompetitorComparison> CompetitorBenchmark { get; set; } = new();

    [BsonElement("analysisPeriod")]
    public DateRange AnalysisPeriod { get; set; } = new();

    [BsonElement("type")]
    public InsightType Type { get; set; }

    [BsonElement("comparisonWithPreviousPeriod")]
    public Dictionary<string, decimal> ComparisonWithPreviousPeriod { get; set; } = new();

    [BsonElement("topContentInsights")]
    public List<ContentInsight> TopContentInsights { get; set; } = new();

    [BsonElement("bestTimeToPost")]
    public List<PostTimeRecommendation> BestTimeToPost { get; set; } = new();

    [BsonElement("recommendations")]
    public List<ContentRecommendation> Recommendations { get; set; } = new();

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("insights")]
    public List<ContentInsight> Insights { get; set; } = new();
}

public class PlatformInsight
{
    public SocialMediaPlatform Platform { get; set; }
    public int FollowerCount { get; set; }
    public int FollowerGrowth { get; set; }
    public decimal EngagementRate { get; set; }
    public decimal ReachGrowth { get; set; }
    public string TopPerformingContent { get; set; } = string.Empty;
}

public class AudienceDemographics
{
    public Dictionary<string, decimal> AgeGroups { get; set; } = new();
    public Dictionary<string, decimal> TopCountries { get; set; } = new();
    public Dictionary<string, decimal> Gender { get; set; } = new();
}

public class GrowthOpportunity
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PotentialRevenueIncrease { get; set; }
}

public class CompetitorComparison
{
    public string Metric { get; set; } = string.Empty;
    public decimal YourValue { get; set; }
    public decimal AverageInNiche { get; set; }
    public decimal Difference { get; set; }
    public bool IsPositive { get; set; }
}

public class DateRange
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
} 