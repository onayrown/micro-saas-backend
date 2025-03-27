using MicroSaaS.Domain.Entities;

namespace MicroSaaS.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
    Task<bool> ValidateTokenAsync(string token);
    Task<string> RefreshTokenAsync(string token);
    Task RevokeTokenAsync(string token);
    Guid GetUserIdFromToken(string token);
    string GetUserEmailFromToken(string token);
}
