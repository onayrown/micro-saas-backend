using System.Text.Json.Serialization;

namespace MicroSaaS.Application.DTOs.Auth;

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

    public static AuthResponse SuccessResponse(UserDto user, string token)
    {
        return new AuthResponse
        {
            Success = true,
            User = user,
            Token = token
        };
    }

    public static AuthResponse FailureResponse(string message)
    {
        return new AuthResponse
        {
            Success = false,
            Message = message
        };
    }
} 