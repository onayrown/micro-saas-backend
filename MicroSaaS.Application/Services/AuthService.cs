using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Interfaces.Repositories;
using MicroSaaS.Shared.Results;
using System.Security.Claims;

namespace MicroSaaS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result<AuthResponse>.Fail("Email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Name,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        var token = _tokenService.GenerateToken(user);

        var response = new AuthResponse 
        { 
            Success = true, 
            Token = token,
            User = new UserDto 
            { 
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive
            }
        };

        return Result<AuthResponse>.Ok(response);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !ValidatePassword(user, request.Password))
        {
            return Result<AuthResponse>.Fail("Invalid email or password");
        }

        var token = _tokenService.GenerateToken(user);

        var response = new AuthResponse 
        { 
            Success = true, 
            Token = token,
            User = new UserDto 
            { 
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive
            }
        };

        return Result<AuthResponse>.Ok(response);
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(string token)
    {
        var userId = _tokenService.GetUserIdFromToken(token);
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            return Result<AuthResponse>.Fail("Invalid token");
        }

        var newToken = _tokenService.GenerateToken(user);

        var response = new AuthResponse 
        { 
            Success = true, 
            Token = newToken,
            User = new UserDto 
            { 
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive
            }
        };

        return Result<AuthResponse>.Ok(response);
    }

    public async Task<Result<bool>> RevokeTokenAsync(string token)
    {
        try
        {
            // Implementação do revoke token
            // Por enquanto, apenas valida o token
            _tokenService.ValidateToken(token);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail($"Failed to revoke token: {ex.Message}");
        }
    }

    public async Task<Result<bool>> ValidateUserCredentialsAsync(string email, string password)
    {
        try
        {
            var user = await _userRepository.GetByEmailAsync(email);
            bool isValid = user != null && ValidatePassword(user, password);
            
            return isValid 
                ? Result<bool>.Ok(true) 
                : Result<bool>.Fail("Invalid email or password");
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail($"Failed to validate credentials: {ex.Message}");
        }
    }

    private bool ValidatePassword(User user, string password)
    {
        return _passwordHasher.VerifyPassword(password, user.PasswordHash);
    }

    public async Task<Result<UserProfileResponse>> GetUserProfileAsync(ClaimsPrincipal claimsPrincipal)
    {
        try
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId))
            {
                return Result<UserProfileResponse>.Fail("Usuário não autenticado");
            }

            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId));
            
            if (user == null)
            {
                return Result<UserProfileResponse>.Fail("Usuário não encontrado");
            }
            
            var userProfileData = new UserProfileData
            {
                Id = user.Id.ToString(),
                Name = user.Username,
                Email = user.Email,
                Role = "user",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                ProfileImageUrl = null
            };
            
            var response = UserProfileResponse.SuccessResponse(userProfileData);
            
            return Result<UserProfileResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            return Result<UserProfileResponse>.Fail($"Erro ao obter perfil: {ex.Message}");
        }
    }
}
