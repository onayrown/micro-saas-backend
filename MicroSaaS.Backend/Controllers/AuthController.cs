using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Application.Interfaces.Services;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILoggingService _loggingService;

    public AuthController(IAuthService authService, ILoggingService loggingService)
    {
        _authService = authService;
        _loggingService = loggingService;
    }

    /// <summary>
    /// Registra um novo usuário no sistema
    /// </summary>
    /// <param name="request">Dados para registro do usuário</param>
    /// <returns>Resposta com token de autenticação</returns>
    /// <response code="200">Usuário registrado com sucesso</response>
    /// <response code="400">Dados de registro inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        try
        {
            _loggingService.LogInformation("Tentativa de registro para o email: {Email}", request.Email);
            
            var result = await _authService.RegisterAsync(request);
            
            if (!result.Success)
            {
                _loggingService.LogWarning("Falha no registro para o email: {Email}. Motivo: {ErrorMessage}", request.Email, result.ErrorMessage);
                return BadRequest(new AuthResponse { Success = false, Message = result.ErrorMessage }); 
            }

            _loggingService.LogInformation("Registro bem-sucedido para o email: {Email}", request.Email);
            return Ok(result.Data); 
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro durante o registro para o email: {Email}", request.Email);
            return StatusCode(500, new AuthResponse { Success = false, Message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Autentica um usuário existente no sistema
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <returns>Resposta com token de autenticação</returns>
    /// <response code="200">Login realizado com sucesso</response>
    /// <response code="401">Credenciais inválidas</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        try
        {
            _loggingService.LogInformation("Tentativa de login para o email: {Email}", request.Email);
            var result = await _authService.LoginAsync(request);
            
            if (!result.Success)
            {
                _loggingService.LogWarning("Falha no login para o email: {Email}", request.Email);
                return Unauthorized(new { message = result.ErrorMessage });
            }

            _loggingService.LogInformation("Login bem-sucedido para o email: {Email}", request.Email);
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro durante o login para o email: {Email}", request.Email);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Obtém o perfil do usuário autenticado
    /// </summary>
    /// <returns>Dados do perfil do usuário</returns>
    /// <response code="200">Perfil obtido com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("user-profile")]
    [Authorize]
    [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserProfileResponse>> GetUserProfileAsync()
    {
        try
        {
            _loggingService.LogInformation("Solicitação de perfil de usuário");
            var result = await _authService.GetUserProfileAsync(User);
            
            if (!result.Success)
            {
                _loggingService.LogWarning("Falha ao obter perfil do usuário. Motivo: {ErrorMessage}", result.ErrorMessage);
                return Unauthorized(new { message = result.ErrorMessage });
            }

            _loggingService.LogInformation("Perfil de usuário obtido com sucesso");
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter perfil do usuário");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Renova um token de autenticação existente
    /// </summary>
    /// <param name="token">Token atual no formato Bearer</param>
    /// <returns>Resposta com novo token de autenticação</returns>
    /// <response code="200">Token renovado com sucesso</response>
    /// <response code="400">Token inválido ou expirado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<AuthResponse>> RefreshTokenAsync([FromHeader(Name = "Authorization")] string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
            {
                _loggingService.LogWarning("Tentativa de refresh token com formato inválido");
                return BadRequest(new { message = "Token inválido" });
            }

            token = token.Substring("Bearer ".Length);
            _loggingService.LogInformation("Tentativa de refresh token");
            
            var result = await _authService.RefreshTokenAsync(token);
            if (!result.Success)
            {
                _loggingService.LogWarning("Falha no refresh token. Motivo: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(new { message = result.ErrorMessage });
            }

            _loggingService.LogInformation("Refresh token bem-sucedido");
            return Ok(result.Data);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro durante o refresh token");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    /// <summary>
    /// Revoga um token de autenticação existente
    /// </summary>
    /// <param name="token">Token a ser revogado no formato Bearer</param>
    /// <returns>Confirmação de revogação</returns>
    /// <response code="200">Token revogado com sucesso</response>
    /// <response code="400">Token inválido</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("revoke-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RevokeTokenAsync([FromHeader(Name = "Authorization")] string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
            {
                _loggingService.LogWarning("Tentativa de revogação de token com formato inválido");
                return BadRequest(new { message = "Token inválido" });
            }

            token = token.Substring("Bearer ".Length);
            _loggingService.LogInformation("Tentativa de revogação de token");
            
            var result = await _authService.RevokeTokenAsync(token);
            if (!result.Success)
            {
                _loggingService.LogWarning("Falha na revogação do token. Motivo: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(new { message = result.ErrorMessage });
            }
            
            _loggingService.LogInformation("Token revogado com sucesso");
            return Ok(new { message = "Token revogado com sucesso" });
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro durante a revogação do token");
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }
} 