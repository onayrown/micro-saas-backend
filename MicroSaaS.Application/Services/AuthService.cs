using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Interfaces.Repositories;

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

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return new AuthResponse { Success = false, Message = "Email already exists" };
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

        return new AuthResponse 
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
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !ValidatePassword(user, request.Password))
        {
            return new AuthResponse { Success = false, Message = "Invalid email or password" };
        }

        var token = _tokenService.GenerateToken(user);

        return new AuthResponse 
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
    }

    public async Task<AuthResponse> RefreshTokenAsync(string token)
    {
        var userId = _tokenService.GetUserIdFromToken(token);
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
        {
            return new AuthResponse { Success = false, Message = "Invalid token" };
        }

        var newToken = _tokenService.GenerateToken(user);

        return new AuthResponse 
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
    }

    public async Task RevokeTokenAsync(string token)
    {
        // Implementação do revoke token
        // Por enquanto, apenas valida o token
        _tokenService.ValidateToken(token);
    }

    public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        return user != null && ValidatePassword(user, password);
    }

    private bool ValidatePassword(User user, string password)
    {
        return _passwordHasher.VerifyPassword(password, user.PasswordHash);
    }
}
