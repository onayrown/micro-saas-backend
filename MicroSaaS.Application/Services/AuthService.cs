using MicroSaaS.Application.DTOs;
using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Shared.Results;
using System.Security.Claims;

namespace MicroSaaS.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILoggingService _loggingService;

    public AuthService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILoggingService loggingService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _loggingService = loggingService ?? throw new ArgumentNullException(nameof(loggingService));
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
                Id = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive
            }
        };

        return Result<AuthResponse>.Ok(response);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        _loggingService.LogInformation("[AuthService] Iniciando tentativa de login para: {Email}", request.Email);
        
        var user = await _userRepository.GetByEmailAsync(request.Email);
        
        if (user == null)
        {
            _loggingService.LogWarning("[AuthService] Usuário não encontrado para o email: {Email}", request.Email);
            // Não logar hash aqui
        }
        else
        {
            // Logar o hash SOMENTE em ambiente de desenvolvimento para depuração
            #if DEBUG
            _loggingService.LogDebug("[AuthService] Usuário encontrado para {Email}. Hash armazenado: {PasswordHash}", request.Email, user.PasswordHash);
            #endif
        }

        bool isPasswordValid = user != null && ValidatePassword(user, request.Password);
        _loggingService.LogInformation("[AuthService] Resultado da validação da senha para {Email}: {IsValid}", request.Email, isPasswordValid);

        if (!isPasswordValid) // Combines null check and password check
        {
            _loggingService.LogWarning("[AuthService] Falha no login para {Email}. Motivo: Usuário nulo ou senha inválida.", request.Email);
            return Result<AuthResponse>.Fail("Invalid email or password");
        }

        _loggingService.LogInformation("[AuthService] Login bem-sucedido para {Email}. Gerando token.", request.Email);
        var token = _tokenService.GenerateToken(user);

        var response = new AuthResponse 
        { 
            Success = true, 
            Token = token,
            User = new UserDto 
            { 
                Id = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive
            }
        };

        return Result<AuthResponse>.Ok(response);
    }

    public async Task<Result<AuthResponse>> RefreshTokenAsync(string token)
    {
        var userIdString = _tokenService.GetUserIdFromToken(token);
        if (string.IsNullOrEmpty(userIdString))
        {
            return Result<AuthResponse>.Fail("Invalid token format (User ID)");
        }

        // Converter string para Guid
        if (!Guid.TryParse(userIdString, out var userIdGuid))
        {
            return Result<AuthResponse>.Fail("Invalid token format (User ID is not a valid Guid)");
        }

        var user = await _userRepository.GetByIdAsync(userIdGuid);
        
        if (user == null)
        {
            return Result<AuthResponse>.Fail("Invalid token (User not found)");
        }

        var newToken = _tokenService.GenerateToken(user);

        var response = new AuthResponse 
        { 
            Success = true, 
            Token = newToken,
            User = new UserDto 
            { 
                Id = user.Id.ToString(),
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
            return Result<bool>.Fail("Failed to revoke token: " + ex.Message);
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
            return Result<bool>.Fail("Failed to validate credentials: " + ex.Message);
        }
    }

    private bool ValidatePassword(User user, string password)
    {
        // Logar a verificação SOMENTE em ambiente de desenvolvimento
        #if DEBUG
        _loggingService.LogDebug("[AuthService] Verificando senha. Hash Armazenado: {StoredHash}", user.PasswordHash);
        #endif
        // --- INÍCIO: REVERTER TESTE COM INSTANCIAÇÃO DIRETA --- 
        var result = _passwordHasher.VerifyPassword(password, user.PasswordHash); // Chamada original via DI
        
        /* // Código do teste anterior removido
        // Tenta instanciar diretamente para bypassar DI
        var directHasher = new MicroSaaS.Infrastructure.Services.PasswordHasher(); 
        Console.WriteLine($"[DEBUG AuthService] Direct Instantiation Verify: '{password}' vs '{user.PasswordHash ?? "NULL HASH"}'");
        var result = directHasher.VerifyPassword(password, user.PasswordHash); 
        */
        // --- FIM: REVERTER TESTE COM INSTANCIAÇÃO DIRETA ---
        _loggingService.LogDebug("[AuthService] Resultado de BCrypt.Verify: {Result}", result);
        return result;
    }

    public async Task<Result<UserProfileResponse>> GetUserProfileAsync(ClaimsPrincipal claimsPrincipal)
    {
        try
        {
            var userIdString = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userIdString))
            {
                return Result<UserProfileResponse>.Fail("Usuário não autenticado");
            }

            // Converter string para Guid
            if (!Guid.TryParse(userIdString, out var userIdGuidProfile))
            {
                 // Usando a versão que não requer Exception (com severidade Error)
                 _loggingService.LogWarning("[AuthService] Falha ao converter UserID da claim para Guid: {UserIdString}", userIdString);
                 return Result<UserProfileResponse>.Fail("Formato inválido do ID do usuário na claim.");
            }

            var user = await _userRepository.GetByIdAsync(userIdGuidProfile);
            
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
            // Extrair userIdString para uma variável antes de logar
            var userIdForLog = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier) ?? "[ID não encontrado na claim]";
            
            // Correto: Passar a exceção como primeiro parâmetro
            _loggingService.LogError(ex, "AuthService: Erro ao obter perfil para UserID: {UserIdString}", userIdForLog);
            
            return Result<UserProfileResponse>.Fail("Erro ao obter perfil: " + ex.Message);
        }
    }
}
