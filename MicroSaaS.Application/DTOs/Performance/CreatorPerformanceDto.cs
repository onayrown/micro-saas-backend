namespace MicroSaaS.Application.DTOs.Performance;

public class CreatorPerformanceDto
{
    public Guid CreatorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public decimal EngagementRate { get; set; }
} 