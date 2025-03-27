using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Application.DTOs;

public class ContentPostDto
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public SocialMediaPlatform Platform { get; set; }
    public DateTime ScheduledTime { get; set; }
}
