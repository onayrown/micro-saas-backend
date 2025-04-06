using System.Text.Json.Serialization;

namespace MicroSaaS.Application.DTOs.Auth;

public class UserProfileResponse
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("data")]
    public UserProfileData? Data { get; set; }

    public static UserProfileResponse SuccessResponse(UserProfileData data)
    {
        return new UserProfileResponse
        {
            Success = true,
            Data = data
        };
    }

    public static UserProfileResponse FailureResponse(string message)
    {
        return new UserProfileResponse
        {
            Success = false,
            Message = message
        };
    }
}

public class UserProfileData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;

    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("profileImageUrl")]
    public string? ProfileImageUrl { get; set; }
} 