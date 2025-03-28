using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MicroSaaS.Infrastructure.Entities
{
    public class ContentCreatorEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonRepresentation(BsonType.String)]
        public Guid UserId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public string WebsiteUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SubscriptionPlanEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("max_posts")]
        public int MaxPosts { get; set; }

        [BsonElement("is_free_plan")]
        public bool IsFreePlan { get; set; }
    }
}
