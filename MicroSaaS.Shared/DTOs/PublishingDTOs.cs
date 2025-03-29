using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Shared.DTOs
{
    public class PublishedPostDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("publishedAt")]
        public DateTime PublishedAt { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("platform")]
        public SocialMediaPlatform Platform { get; set; }

        [JsonPropertyName("status")]
        public PostStatus Status { get; set; }

        [JsonPropertyName("mediaUrls")]
        public List<string> MediaUrls { get; set; } = new List<string>();

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new List<string>();

        [JsonPropertyName("isRepost")]
        public bool IsRepost { get; set; }

        [JsonPropertyName("originalPostId")]
        public Guid? OriginalPostId { get; set; }

        [JsonPropertyName("externalPostId")]
        public string? ExternalPostId { get; set; }

        [JsonPropertyName("externalPostUrl")]
        public string? ExternalPostUrl { get; set; }
    }

    public class PublishNowDto
    {
        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("platform")]
        public SocialMediaPlatform Platform { get; set; }

        [JsonPropertyName("mediaUrls")]
        public List<string> MediaUrls { get; set; } = new List<string>();

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new List<string>();
    }

    public class RepublishPostDto
    {
        [JsonPropertyName("postId")]
        public Guid PostId { get; set; }

        [JsonPropertyName("includeOriginalComments")]
        public bool IncludeOriginalComments { get; set; }

        [JsonPropertyName("additionalComment")]
        public string? AdditionalComment { get; set; }
    }

    public class PublishingStatsDto
    {
        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; set; }

        [JsonPropertyName("totalPublished")]
        public int TotalPublished { get; set; }

        [JsonPropertyName("totalScheduled")]
        public int TotalScheduled { get; set; }

        [JsonPropertyName("publishedByPlatform")]
        public Dictionary<SocialMediaPlatform, int> PublishedByPlatform { get; set; } = new Dictionary<SocialMediaPlatform, int>();

        [JsonPropertyName("mostEngagedPosts")]
        public List<PostEngagementSummaryDto> MostEngagedPosts { get; set; } = new List<PostEngagementSummaryDto>();
    }

    public class PostEngagementDto
    {
        [JsonPropertyName("postId")]
        public Guid PostId { get; set; }

        [JsonPropertyName("platform")]
        public SocialMediaPlatform Platform { get; set; }

        [JsonPropertyName("likes")]
        public int Likes { get; set; }

        [JsonPropertyName("comments")]
        public int Comments { get; set; }

        [JsonPropertyName("shares")]
        public int Shares { get; set; }

        [JsonPropertyName("views")]
        public int Views { get; set; }

        [JsonPropertyName("engagementRate")]
        public double EngagementRate { get; set; }

        [JsonPropertyName("lastUpdated")]
        public DateTime LastUpdated { get; set; }
    }

    public class PostEngagementSummaryDto
    {
        [JsonPropertyName("postId")]
        public Guid PostId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("platform")]
        public SocialMediaPlatform Platform { get; set; }

        [JsonPropertyName("publishedAt")]
        public DateTime PublishedAt { get; set; }

        [JsonPropertyName("totalEngagement")]
        public int TotalEngagement { get; set; }

        [JsonPropertyName("engagementRate")]
        public double EngagementRate { get; set; }
    }
} 