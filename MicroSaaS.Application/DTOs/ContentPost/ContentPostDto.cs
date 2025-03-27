using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Application.DTOs.ContentPost;

public class ContentPostDto
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; }
    public SocialMediaPlatform Platform { get; set; }
    public string MediaUrl { get; set; }
    public DateTime ScheduledTime { get; set; }
    public PostStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreatePostRequest
{
    public Guid CreatorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; }
    public SocialMediaPlatform Platform { get; set; }
    public string MediaUrl { get; set; }
    public DateTime? ScheduledTime { get; set; }
}

public class UpdatePostRequest
{
    public string Title { get; set; }
    public string Content { get; set; }
    public SocialMediaPlatform Platform { get; set; }
    public string MediaUrl { get; set; }
    public DateTime? ScheduledTime { get; set; }
    public PostStatus Status { get; set; }
} 