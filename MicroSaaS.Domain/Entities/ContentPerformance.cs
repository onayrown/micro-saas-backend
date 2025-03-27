using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Domain.Entities;

public class ContentPerformance
{
    public Guid Id { get; set; }
    public string? PostId { get; set; }
    public ContentPost? Post { get; set; }
    public SocialMediaPlatform Platform { get; set; }
    public long Views { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public decimal EstimatedRevenue { get; set; }
    public DateTime CollectedAt { get; set; }
    public DateTime Date { get; set; }
}
