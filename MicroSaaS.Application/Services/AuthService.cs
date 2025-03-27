using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Interfaces;

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

    public async Task<AuthenticationResult> RegisterAsync(string username, string email, string password)
    {
        // Verificar se usuário já existe
        if (await _userRepository.CheckUsernameExistsAsync(username))
            return AuthenticationResult.Failure("Username already exists");

        if (await _userRepository.CheckEmailExistsAsync(email))
            return AuthenticationResult.Failure("Email already exists");

        // Criar novo usuário
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            Email = email,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        user.PasswordHash = _passwordHasher.HashPassword(password);

        await _userRepository.CreateAsync(user);

        // Gerar token
        var token = _tokenService.GenerateToken(user);

        return AuthenticationResult.Success(user, token);
    }

    public async Task<AuthenticationResult> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);

        if (user == null)
            return AuthenticationResult.Failure("User not found");

        if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
            return AuthenticationResult.Failure("Invalid password");

        // Atualizar último login
        user.LastLoginAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        // Gerar token
        var token = _tokenService.GenerateToken(user);

        return AuthenticationResult.Success(user, token);
    }

    public async Task<string> GenerateJwtTokenAsync(User user)
    {
        return await Task.FromResult(_tokenService.GenerateToken(user));
    }

    public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null)
            return false;

        return _passwordHasher.VerifyPassword(password, user.PasswordHash);
    }
}
