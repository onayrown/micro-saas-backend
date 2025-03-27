using System;

namespace MicroSaaS.Shared.Models;

public class PostTimeRecommendation
{
    public Guid Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan TimeOfDay { get; set; }
    public decimal EngagementScore { get; set; }
} 