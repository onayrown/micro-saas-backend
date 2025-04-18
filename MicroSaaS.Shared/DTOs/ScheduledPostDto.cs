using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Shared.DTOs;

public class ScheduledPostDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("creatorId")]
    public Guid CreatorId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("scheduledFor")]
    public DateTime? ScheduledFor { get; set; }

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("platform")]
    public SocialMediaPlatform Platform { get; set; }

    [JsonPropertyName("status")]
    public PostStatus Status { get; set; }

    [JsonPropertyName("mediaUrls")]
    public List<string> MediaUrls { get; set; } = new List<string>();

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new List<string>();
}

public class CreateScheduledPostDto
{
    [JsonPropertyName("creatorId")]
    public Guid CreatorId { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonPropertyName("scheduledFor")]
    public DateTime ScheduledFor { get; set; }

    [JsonPropertyName("platform")]
    public SocialMediaPlatform Platform { get; set; }

    [JsonPropertyName("mediaUrls")]
    public List<string> MediaUrls { get; set; } = new List<string>();

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new List<string>();
}

public class UpdateScheduledPostDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonPropertyName("scheduledFor")]
    public DateTime? ScheduledFor { get; set; }

    [JsonPropertyName("mediaUrls")]
    public List<string>? MediaUrls { get; set; }

    [JsonPropertyName("tags")]
    public List<string>? Tags { get; set; }
} 