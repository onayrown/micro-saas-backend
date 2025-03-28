using MicroSaaS.Shared.Enums;
using System;

namespace MicroSaaS.Domain.Entities
{
    public class SocialMediaAccount
    {
        public Guid Id { get; set; }
        public Guid CreatorId { get; set; }
        public SocialMediaPlatform Platform { get; set; }
        public string Username { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int FollowersCount { get; set; }
    }
}
