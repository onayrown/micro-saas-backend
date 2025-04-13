using MicroSaaS.Domain.Entities;
using System.Security.Claims;

namespace MicroSaaS.Application.Interfaces.Services;

public interface ITokenService
{
    string GenerateToken(User user);
    ClaimsPrincipal ValidateToken(string token);
    Task<bool> ValidateTokenAsync(string token);
    Task<string> RefreshTokenAsync(string token);
    Task RevokeTokenAsync(string token);
    string GetUserIdFromToken(string token);
    string GetUserEmailFromToken(string token);
}
