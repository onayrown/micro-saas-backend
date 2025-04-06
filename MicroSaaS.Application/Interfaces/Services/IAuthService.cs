using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Application.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Results;
using System.Security.Claims;

namespace MicroSaaS.Application.Interfaces.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
    Task<Result<AuthResponse>> RefreshTokenAsync(string token);
    Task<Result<bool>> RevokeTokenAsync(string token);
    Task<Result<bool>> ValidateUserCredentialsAsync(string email, string password);
    Task<Result<UserProfileResponse>> GetUserProfileAsync(ClaimsPrincipal user);
}
