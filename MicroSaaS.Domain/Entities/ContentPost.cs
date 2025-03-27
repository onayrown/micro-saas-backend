using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MicroSaaS.Shared.Enums;

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

        [BsonElement("creator")]
        public required ContentCreator Creator { get; set; }

        [BsonElement("title")]
        public required string Title { get; set; }

        [BsonElement("content")]
        public required string Content { get; set; }

        [BsonElement("mediaUrl")]
        public required string MediaUrl { get; set; }

        [BsonElement("platform")]
        public SocialMediaPlatform Platform { get; set; }

        [BsonElement("status")]
        public PostStatus Status { get; set; }

        [BsonElement("scheduledTime")]
        public DateTime ScheduledTime { get; set; }
        
        [BsonElement("scheduledFor")]
        public DateTime? ScheduledFor { get; set; }

        [BsonElement("publishedAt")]
        public DateTime? PublishedAt { get; set; }

        [BsonElement("postedTime")]
        public DateTime? PostedTime { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
