using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Application.Services;
using MicroSaaS.Domain.Entities;

namespace MicroSaaS.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse> LoginAsync(LoginRequest request);
    Task<AuthResponse> RefreshTokenAsync(string token);
    Task RevokeTokenAsync(string token);
    Task<bool> ValidateUserCredentialsAsync(string email, string password);
}
