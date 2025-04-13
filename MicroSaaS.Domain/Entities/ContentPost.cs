using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MicroSaaS.Domain.Entities
{
    public class ContentPost
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("creatorId")]
        [BsonRepresentation(BsonType.String)]
        public Guid CreatorId { get; set; }

        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;

        [BsonElement("content")]
        public string Content { get; set; } = string.Empty;

        [BsonElement("mediaUrl")]
        public string MediaUrl { get; set; } = string.Empty;

        [BsonElement("mediaUrls")]
        public List<string> MediaUrls { get; set; } = new List<string>();

        [BsonElement("hashtags")]
        public List<string> Hashtags { get; set; } = new List<string>();

        [BsonElement("platformId")]
        public string PlatformId { get; set; } = string.Empty;

        [BsonElement("platform")]
        public SocialMediaPlatform Platform { get; set; }

        [BsonElement("status")]
        public PostStatus Status { get; set; }

        [BsonElement("scheduledTime")]
        public DateTime? ScheduledTime { get; set; }

        [BsonElement("scheduledFor")]
        public DateTime? ScheduledFor { get; set; }

        [BsonElement("publishedAt")]
        public DateTime? PublishedAt { get; set; }

        [BsonElement("postedTime")]
        public DateTime? PostedTime { get; set; }

        [BsonElement("views")]
        public long Views { get; set; }

        [BsonElement("likes")]
        public long Likes { get; set; }

        [BsonElement("comments")]
        public long Comments { get; set; }

        [BsonElement("shares")]
        public long Shares { get; set; }

        [BsonElement("engagementRate")]
        public decimal EngagementRate { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
