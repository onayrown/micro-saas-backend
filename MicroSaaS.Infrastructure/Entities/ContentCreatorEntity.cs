using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Infrastructure.Entities
{
    public class ContentCreatorEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string? Name { get; set; }

        [BsonElement("email")]
        public string? Email { get; set; }

        [BsonElement("username")]
        public string? Username { get; set; }

        [BsonElement("bio")]
        public string? Bio { get; set; }

        [BsonElement("niche")]
        public string? Niche { get; set; }

        [BsonElement("content_type")]
        public ContentType ContentType { get; set; }

        [BsonElement("subscription_plan")]
        public SubscriptionPlanEntity? SubscriptionPlan { get; set; }

        [BsonElement("social_media_accounts")]
        public List<SocialMediaAccountEntity> SocialMediaAccounts { get; set; } = new();

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
