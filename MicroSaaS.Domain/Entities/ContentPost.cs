using MicroSaaS.Shared.Enums;
using System;

namespace MicroSaaS.Domain.Entities
{
    public class ContentPost
    {
        public Guid Id { get; set; }
        public Guid CreatorId { get; set; }
        public ContentCreator Creator { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string MediaUrl { get; set; } = string.Empty;
        public SocialMediaPlatform Platform { get; set; }
        public PostStatus Status { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public DateTime? ScheduledFor { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? PostedTime { get; set; }
        public long Views { get; set; }
        public long Likes { get; set; }
        public long Comments { get; set; }
        public long Shares { get; set; }
        public decimal EngagementRate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
