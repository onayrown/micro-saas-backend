using System;

namespace MicroSaaS.Shared.Models;

public class CreatorAnalytics
{
    public Guid CreatorId { get; set; }
    public int TotalPosts { get; set; }
    public int TotalEngagement { get; set; }
    public double AverageEngagementRate { get; set; }
    public PostAnalytics[] TopPerformingPosts { get; set; }
} 