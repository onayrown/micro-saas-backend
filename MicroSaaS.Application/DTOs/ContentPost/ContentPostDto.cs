using MicroSaaS.Shared.Enums;
using System;

namespace MicroSaaS.Application.DTOs.ContentPost;

public class ContentPostDto
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public SocialMediaPlatform Platform { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public TimeSpan? ScheduledTime { get; set; }
    public DateTime? ScheduledFor { get; set; }
    public PostStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreatePostRequest
{
    public Guid CreatorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public SocialMediaPlatform Platform { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public TimeSpan? ScheduledTime { get; set; }
    public DateTime? ScheduledFor { get; set; }
}

public class UpdatePostRequest
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public SocialMediaPlatform Platform { get; set; }
    public string MediaUrl { get; set; } = string.Empty;
    public TimeSpan? ScheduledTime { get; set; }
    public DateTime? ScheduledFor { get; set; }
    public PostStatus Status { get; set; }
} 