using MicroSaaS.Domain.Entities;

namespace MicroSaaS.Domain.Interfaces.Services;

public interface ITokenService
{
    Task<string> GenerateTokenAsync(User user);
    Task<bool> ValidateTokenAsync(string token);
    Task<string> RefreshTokenAsync(string token);
    Task RevokeTokenAsync(string token);
} 