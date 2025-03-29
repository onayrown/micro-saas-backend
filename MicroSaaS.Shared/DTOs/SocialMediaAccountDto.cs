using System;
using System.Text.Json.Serialization;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Shared.DTOs
{
    public class SocialMediaAccountDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; set; }

        [JsonPropertyName("platform")]
        public SocialMediaPlatform Platform { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("followers")]
        public int Followers { get; set; }

        [JsonPropertyName("profileUrl")]
        public string ProfileUrl { get; set; } = string.Empty;

        [JsonPropertyName("profileImageUrl")]
        public string ProfileImageUrl { get; set; } = string.Empty;

        [JsonPropertyName("isVerified")]
        public bool IsVerified { get; set; }
    }
} 