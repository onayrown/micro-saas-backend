using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MicroSaaS.Domain.Entities;

public class ContentInsight
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public SocialMediaPlatform Platform { get; set; }
    public DateTime PublishedAt { get; set; }
    public long Views { get; set; }
    public decimal EngagementRate { get; set; }
    public decimal Revenue { get; set; }
    public InsightType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public InsightType InsightType { get; set; }
    public string Insight { get; set; } = string.Empty;
    public Dictionary<string, double> Metrics { get; set; } = new();
    public TrendDirection Trend { get; set; }
    public string ComparisonPeriod { get; set; } = string.Empty;
    public double PercentageChange { get; set; }
    public string RecommendedAction { get; set; } = string.Empty;
} 