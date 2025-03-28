using MicroSaaS.Shared.Enums;
using System;

namespace MicroSaaS.Domain.Entities;

public class ContentPerformance
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public Guid CreatorId { get; set; }
    public SocialMediaPlatform Platform { get; set; }
    public DateTime Date { get; set; }
    public DateTime CollectedAt { get; set; }
    public long Views { get; set; }
    public long Likes { get; set; }
    public long Comments { get; set; }
    public long Shares { get; set; }
    public decimal EngagementRate { get; set; }
    public decimal EstimatedRevenue { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
