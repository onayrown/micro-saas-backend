using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Exceptions;

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

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
            throw new AuthenticationException("Email ou senha inválidos");

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            throw new AuthenticationException("Email ou senha inválidos");

        if (!user.IsActive)
            throw new AuthenticationException("Usuário inativo");

        var token = _tokenService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            }
        };
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
            throw new AuthenticationException("Email já cadastrado");

        var passwordHash = _passwordHasher.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = passwordHash,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);

        var token = _tokenService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            }
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(string token)
    {
        var newToken = await _tokenService.RefreshTokenAsync(token);

        var userId = _tokenService.GetUserIdFromToken(newToken);
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new AuthenticationException("Usuário não encontrado");

        return new AuthResponse
        {
            Token = newToken,
            User = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            }
        };
    }

    public async Task RevokeTokenAsync(string token)
    {
        await _tokenService.RevokeTokenAsync(token);
    }
    
    public async Task<bool> ValidateUserCredentialsAsync(string email, string password)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user == null || !user.IsActive)
            return false;

        return _passwordHasher.VerifyPassword(password, user.PasswordHash);
    }
} 