using System;

namespace MicroSaaS.Shared.Models;

public class PostTimeRecommendation
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan TimeOfDay { get; set; }
    public double EngagementScore { get; set; }
} 