using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Shared.DTOs
{
    public class ContentCreatorDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("userId")]
        public Guid UserId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("username")]
        public string Username { get; set; } = string.Empty;

        [JsonPropertyName("bio")]
        public string Bio { get; set; } = string.Empty;

        [JsonPropertyName("profileImageUrl")]
        public string ProfileImageUrl { get; set; } = string.Empty;

        [JsonPropertyName("websiteUrl")]
        public string WebsiteUrl { get; set; } = string.Empty;

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("totalFollowers")]
        public int TotalFollowers { get; set; }

        [JsonPropertyName("totalPosts")]
        public int TotalPosts { get; set; }
        
        [JsonPropertyName("platforms")]
        public List<SocialMediaPlatform> Platforms { get; set; } = new List<SocialMediaPlatform>();
        
        [JsonPropertyName("socialMediaAccounts")]
        public List<SocialMediaAccountDto> SocialMediaAccounts { get; set; } = new List<SocialMediaAccountDto>();
    }
} 