using MicroSaaS.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MicroSaaS.Domain.Entities
{
    public class SocialMediaAccount
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }

        [BsonElement("creatorId")]
        [BsonRepresentation(BsonType.String)]
        public Guid CreatorId { get; set; }

        [BsonElement("platform")]
        public SocialMediaPlatform Platform { get; set; }

        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("accessToken")]
        public string AccessToken { get; set; } = string.Empty;

        [BsonElement("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;

        [BsonElement("tokenExpiresAt")]
        public DateTime TokenExpiresAt { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }

        [BsonElement("followersCount")]
        public int FollowersCount { get; set; }

        [BsonElement("profileUrl")]
        public string ProfileUrl { get; set; } = string.Empty;

        [BsonElement("profileImageUrl")]
        public string ProfileImageUrl { get; set; } = string.Empty;

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
