using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace MicroSaaS.IntegrationTests;

[ApiController]
[Route("api/auth")]
public class TestAuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public TestAuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Simular a validação de credenciais
        bool isValid = await _authService.ValidateUserCredentialsAsync(request.Email, request.Password);
        
        if (!isValid)
            return Unauthorized();
            
        // Se as credenciais são válidas, retorna uma resposta de autenticação mock
        return Ok(new AuthResponse
        {
            Success = true,
            Token = "valid_token_for_testing",
            Message = "Login successful",
            User = new UserDto
            {
                Id = Guid.NewGuid(),
                Username = "Test User",
                Email = request.Email,
                IsActive = true
            }
        });
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        // Sempre retorna sucesso para testes
        return Ok(new AuthResponse
        {
            Success = true,
            Token = "valid_token_for_testing",
            Message = "Registration successful",
            User = new UserDto
            {
                Id = Guid.NewGuid(),
                Username = request.Name,
                Email = request.Email,
                IsActive = true
            }
        });
    }
} 