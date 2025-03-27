using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Shared.Models;

public class PostAnalytics
{
    public Guid PostId { get; set; }
    public SocialMediaPlatform Platform { get; set; }
    public int Likes { get; set; }
    public int Comments { get; set; }
    public int Shares { get; set; }
    public int Reach { get; set; }
    public double Engagement { get; set; }
    public DateTime CollectedAt { get; set; }
} 