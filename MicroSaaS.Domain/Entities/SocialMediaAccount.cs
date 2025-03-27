using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Domain.Entities
{
    public class SocialMediaAccount
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("creator_id")]
        [BsonRepresentation(BsonType.String)]
        public Guid CreatorId { get; set; }

        [BsonElement("platform")]
        public SocialMediaPlatform Platform { get; set; }

        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [BsonElement("refresh_token")]
        public string? RefreshToken { get; set; }

        [BsonElement("token_expires_at")]
        public DateTime? TokenExpiresAt { get; set; }

        [BsonElement("is_active")]
        public bool IsActive { get; set; } = true;

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
