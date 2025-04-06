using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Results;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

namespace MicroSaaS.Infrastructure.Services;

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

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            return Result<AuthResponse>.Fail("Email ou senha inválidos");

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Result<AuthResponse>.Fail("Email ou senha inválidos");

        if (!user.IsActive)
            return Result<AuthResponse>.Fail("Usuário inativo");

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
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }
        };

        return Result<AuthResponse>.Ok(response);
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result<AuthResponse>.Fail("Email já cadastrado");
        }

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash,
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
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            }
        };

        return Result<AuthResponse>.Ok(response);
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(string token)
    {
        try
        {
            var newToken = await _tokenService.RefreshTokenAsync(token);

            var userId = _tokenService.GetUserIdFromToken(newToken);
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
                return Result<AuthResponse>.Fail("Usuário não encontrado");

            var response = new AuthResponse
            {
                Success = true,
                Token = newToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                }
            };

            return Result<AuthResponse>.Ok(response);
        }
        catch (Exception ex)
        {
            return Result<AuthResponse>.Fail($"Erro ao renovar token: {ex.Message}");
        }
    }

    public async Task<Result<bool>> RevokeTokenAsync(string token)
    {
        try
        {
            await _tokenService.RevokeTokenAsync(token);
            return Result<bool>.Ok(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Fail($"Erro ao revogar token: {ex.Message}");
        }
    }
    
    public async Task<Result<bool>> ValidateUserCredentialsAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !user.IsActive)
            return Result<bool>.Fail("Credenciais inválidas");

        var isValid = _passwordHasher.VerifyPassword(password, user.PasswordHash);
        if (!isValid)
            return Result<bool>.Fail("Credenciais inválidas");

        return Result<bool>.Ok(true);
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