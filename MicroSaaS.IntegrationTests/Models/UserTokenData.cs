using System;

namespace MicroSaaS.IntegrationTests.Models
{
    public class UserTokenData
    {
        public Guid UserId { get; set; }
        public string Role { get; set; }
        public bool IsValid { get; set; }
    }
} 