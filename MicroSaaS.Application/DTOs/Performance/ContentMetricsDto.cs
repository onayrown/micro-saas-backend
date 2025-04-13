namespace MicroSaaS.Application.DTOs.Performance;

public class ContentMetricsDto
{
    public Guid ContentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CreatorName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public decimal EngagementRate { get; set; }
    public decimal Revenue { get; set; }
    public List<PlatformMetricsDto> PlatformMetrics { get; set; } = new();
    public List<DailyMetricsDto> DailyMetrics { get; set; } = new();
}

public class PlatformMetricsDto
{
    public string Platform { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public decimal EngagementRate { get; set; }
}

public class DailyMetricsDto
{
    public DateTime Date { get; set; }
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public decimal EngagementRate { get; set; }
    public decimal Revenue { get; set; }
} 