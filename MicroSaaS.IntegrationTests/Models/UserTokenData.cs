using System;

namespace MicroSaaS.IntegrationTests.Models
{
    public class UserTokenData
    {
        public Guid UserId { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsValid { get; set; }
    }
} 