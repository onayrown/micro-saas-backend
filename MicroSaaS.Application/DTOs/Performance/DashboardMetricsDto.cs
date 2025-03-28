namespace MicroSaaS.Application.DTOs.Performance;

public class DashboardMetricsDto
{
    public List<CreatorPerformanceDto> TopCreators { get; set; } = new();
    public int TotalCreators { get; set; }
    public int TotalPosts { get; set; }
    public decimal AverageEngagementRate { get; set; }
}

public class ContentPerformanceDto
{
    public string ContentId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string CreatorName { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public decimal EngagementRate { get; set; }
}

public class RevenueMetricsDto
{
    public decimal DailyRevenue { get; set; }
    public decimal WeeklyRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal RevenueGrowth { get; set; }
}

public class EngagementMetricsDto
{
    public int DailyEngagements { get; set; }
    public int WeeklyEngagements { get; set; }
    public int MonthlyEngagements { get; set; }
    public decimal EngagementGrowth { get; set; }
} 