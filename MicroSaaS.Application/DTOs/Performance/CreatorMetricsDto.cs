namespace MicroSaaS.Application.DTOs.Performance;

public class CreatorMetricsDto
{
    public Guid CreatorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime JoinedAt { get; set; }
    public int TotalContent { get; set; }
    public int TotalFollowers { get; set; }
    public decimal AverageEngagementRate { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<PlatformFollowersDto> PlatformFollowers { get; set; } = new();
    public List<MonthlyRevenueDto> MonthlyRevenue { get; set; } = new();
    public List<ContentPerformanceSummaryDto> TopPerformingContent { get; set; } = new();
}

public class PlatformFollowersDto
{
    public string Platform { get; set; } = string.Empty;
    public int Followers { get; set; }
    public int Growth { get; set; }
    public decimal GrowthRate { get; set; }
}

public class MonthlyRevenueDto
{
    public DateTime Month { get; set; }
    public decimal Revenue { get; set; }
    public decimal Growth { get; set; }
    public decimal GrowthRate { get; set; }
}