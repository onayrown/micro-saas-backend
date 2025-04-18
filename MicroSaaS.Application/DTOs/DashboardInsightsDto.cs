using System;
using System.Collections.Generic;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;

namespace MicroSaaS.Application.DTOs;

public class DashboardInsightsDto
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public DateTime Date { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime GeneratedDate { get; set; }
    public List<SocialMediaPlatform> Platforms { get; set; } = new();
    public List<PlatformInsightDto> PlatformsPerformance { get; set; } = new();
    public int TotalFollowers { get; set; }
    public int TotalPosts { get; set; }
    public int TotalViews { get; set; }
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public int TotalShares { get; set; }
    public decimal AverageEngagementRate { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalRevenueInPeriod { get; set; }
    public decimal GrowthRate { get; set; }
    public decimal RevenueGrowth { get; set; }
    public int PostingConsistency { get; set; }
    public AudienceDemographicsDto AudienceDemographics { get; set; } = new();
    public Dictionary<string, decimal> ContentTypePerformance { get; set; } = new();
    public List<string> KeyInsights { get; set; } = new();
    public List<string> ActionRecommendations { get; set; } = new();
    public List<GrowthOpportunityDto> GrowthOpportunities { get; set; } = new();
    public List<CompetitorComparisonDto> CompetitorBenchmark { get; set; } = new();
    public DateRangeDto AnalysisPeriod { get; set; } = new();
    public InsightType Type { get; set; }
    public Dictionary<string, decimal> ComparisonWithPreviousPeriod { get; set; } = new();
    public List<ContentInsightDto> TopContentInsights { get; set; } = new();
    public List<PostTimeRecommendation> BestTimeToPost { get; set; } = new();
    public List<ContentRecommendationDto> Recommendations { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ContentInsightDto> Insights { get; set; } = new();
}

public class PlatformInsightDto
{
    public SocialMediaPlatform Platform { get; set; }
    public int FollowerCount { get; set; }
    public int FollowerGrowth { get; set; }
    public decimal EngagementRate { get; set; }
    public decimal ReachGrowth { get; set; }
    public string TopPerformingContent { get; set; } = string.Empty;
}

public class AudienceDemographicsDto
{
    public Dictionary<string, decimal> AgeGroups { get; set; } = new();
    public Dictionary<string, decimal> TopCountries { get; set; } = new();
    public Dictionary<string, decimal> Gender { get; set; } = new();
}

public class GrowthOpportunityDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PotentialRevenueIncrease { get; set; }
}

public class CompetitorComparisonDto
{
    public string Metric { get; set; } = string.Empty;
    public decimal YourValue { get; set; }
    public decimal AverageInNiche { get; set; }
    public decimal Difference { get; set; }
    public bool IsPositive { get; set; }
}

public class DateRangeDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}

public class ContentInsightDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Score { get; set; }
}

public class ContentRecommendationDto
{
    public string Recommendation { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
} 