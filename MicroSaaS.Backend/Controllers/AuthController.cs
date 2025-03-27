using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Application.Interfaces.Services;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthResponse>> RefreshTokenAsync([FromHeader(Name = "Authorization")] string token)
    {
        if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
        {
            return BadRequest(new AuthResponse { Success = false, Message = "Token inválido" });
        }

        token = token.Substring("Bearer ".Length);
        var result = await _authService.RefreshTokenAsync(token);
        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpPost("revoke-token")]
    public async Task<IActionResult> RevokeTokenAsync([FromHeader(Name = "Authorization")] string token)
    {
        if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
        {
            return BadRequest(new AuthResponse { Success = false, Message = "Token inválido" });
        }

        token = token.Substring("Bearer ".Length);
        await _authService.RevokeTokenAsync(token);
        return Ok(new AuthResponse { Success = true, Message = "Token revogado com sucesso" });
    }
} 