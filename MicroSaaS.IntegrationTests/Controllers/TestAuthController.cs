using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Application.Interfaces.Services;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MicroSaaS.IntegrationTests.Controllers
{
    [ApiController]
    [Route("api/auth")]  // Rota para o login 
    [Route("api/v1/auth")]  // Rota para as APIs v1
    public class TestAuthController : ControllerBase
    {
        private readonly ILogger<TestAuthController> _logger;
        private readonly IAuthService _authService;

        public TestAuthController(ILogger<TestAuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("TestAuthController.Login: Processing login request for {Email}", request?.Email);

            if (request == null)
            {
                _logger.LogWarning("TestAuthController.Login: Request body is null");
                return BadRequest("Request body is required");
            }

            if (request.Email == "test@example.com" && request.Password == "Test@123")
            {
                _logger.LogInformation("TestAuthController.Login: Valid credentials provided");
                
                var response = new AuthResponse
                {
                    Success = true,
                    Token = "valid_token_for_testing",
                    Message = "Login successful",
                    User = new UserDto
                    {
                        Id = Guid.NewGuid(),
                        Username = "Test User",
                        Email = request.Email,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                };
                
                return Ok(response);
            }
            
            _logger.LogWarning("TestAuthController.Login: Invalid credentials provided");
            return Unauthorized(new { Success = false, Message = "Invalid credentials" });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("TestAuthController.Register: Processing register request for {Email}", request?.Email);

            if (request == null)
            {
                _logger.LogWarning("TestAuthController.Register: Request body is null");
                return BadRequest("Request body is required");
            }

            var response = new AuthResponse
            {
                Success = true,
                Token = "valid_token_for_testing",
                Message = "Registration successful",
                User = new UserDto
                {
                    Id = Guid.NewGuid(),
                    Username = request.Name,
                    Email = request.Email,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
            
            _logger.LogInformation("TestAuthController.Register: Successfully registered user {Email}", request.Email);
            return Ok(response);
        }
    }
} 