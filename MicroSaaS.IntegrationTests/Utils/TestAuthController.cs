using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Shared.Models;
using MicroSaaS.Application.DTOs.Auth;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AppUserDto = MicroSaaS.Application.DTOs.UserDto;
using TestUserDto = MicroSaaS.IntegrationTests.Models.UserDto;
using AppAuthResponse = MicroSaaS.Application.DTOs.Auth.AuthResponse;

namespace MicroSaaS.IntegrationTests.Utils
{
    [ApiController]
    [Route("api/auth")]
    public class TestAuthController : ControllerBase
    {
        private readonly ILogger<TestAuthController> _logger;

        public TestAuthController(ILogger<TestAuthController> logger)
        {
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AppAuthResponse>> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("TestAuthController.Login: Processando login para {Email}", request.Email);
            
            // Simular autenticação bem-sucedida para credenciais de teste
            if (request.Email == "test@example.com" && request.Password == "Test@123")
            {
                return Ok(new AppAuthResponse
                {
                    Success = true,
                    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJ0ZXN0ZUBtaWNyb3NhYXMuY29tIiwianRpIjoiYWJjMTIzIiwiaWF0IjoxNTE2MjM5MDIyLCJuYmYiOjE1MTYyMzkwMjIsImV4cCI6MjUxNjIzOTAyMiwiYXVkIjoibWljcm9zYWFzLmNvbSIsImlzcyI6Im1pY3Jvc2Fhcy5jb20ifQ.VY6JUOK9gH3AQJl0KEhHYQ5MURKVc18WA5qmVxpWHaE",
                    User = new AppUserDto
                    {
                        Id = "10000000-1000-1000-1000-100000000000",
                        Username = "Usuário de Teste",
                        Email = request.Email,
                        Role = "user",
                        IsActive = true
                    },
                    Message = "Login realizado com sucesso"
                });
            }
            
            return Unauthorized(new AppAuthResponse
            {
                Success = false,
                Message = "Email ou senha inválidos"
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppAuthResponse>> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("TestAuthController.Register: Processando registro para {Email}", request.Email);
            
            // Simular registro bem-sucedido
            return Ok(new AppAuthResponse
            {
                Success = true,
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJub3ZvQG1pY3Jvc2Fhcy5jb20iLCJqdGkiOiJkZWYxMjMiLCJpYXQiOjE1MTYyMzkwMjIsIm5iZiI6MTUxNjIzOTAyMiwiZXhwIjoyNTE2MjM5MDIyLCJhdWQiOiJtaWNyb3NhYXMuY29tIiwiaXNzIjoibWljcm9zYWFzLmNvbSJ9.k21sdFt3ffYJR7vJIQ6HuVDFTDQKjbFN3KWQ2U16o44",
                User = new AppUserDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = request.Name,
                    Email = request.Email,
                    Role = "user",
                    IsActive = true
                },
                Message = "Usuário registrado com sucesso"
            });
        }
    }
} 