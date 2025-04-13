using System.Text.Json.Serialization;

namespace MicroSaaS.IntegrationTests.Models
{
    public class AuthResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }

        [JsonPropertyName("user")]
        public UserDto? User { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;
    }
} 