using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MicroSaaS.IntegrationTests.Mocks
{
    public class MockTokenService : ITokenService
    {
        // Simula a geração de um token simples (não seguro para produção)
        public string GenerateToken(User user)
        {
            return $"mock-token-for-{user.Id}-{Guid.NewGuid()}";
        }

        // Simula a extração do ID do usuário de um token mock
        public string GetUserIdFromToken(string token)
        {
            if (string.IsNullOrEmpty(token) || !token.StartsWith("mock-token-for-"))
                return string.Empty;

            var parts = token.Split('-');
            // Assume formato "mock-token-for-UserId-Guid"
            return parts.Length > 3 ? parts[3] : string.Empty;
        }

        // Simula a validação de um token mock
        public ClaimsPrincipal ValidateToken(string token)
        {
            var userId = GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId))
            {
                 throw new Exception("Invalid mock token"); // Ou retornar null/lançar exceção específica
            }

            // Cria um ClaimsPrincipal simples para o teste
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, "test@example.com") // Email fixo para simplicidade
                // Adicione outras claims necessárias para os testes
            };
            var identity = new ClaimsIdentity(claims, "mock");
            return new ClaimsPrincipal(identity);
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            // Simulação simples: considera o token válido se não estiver vazio
            return Task.FromResult(!string.IsNullOrEmpty(token) && token.StartsWith("mock-token-for-"));
        }

        // Simula a extração do email de um token mock (valor fixo)
        public string GetUserEmailFromToken(string token)
        {
             if (string.IsNullOrEmpty(token) || !token.StartsWith("mock-token-for-"))
                return string.Empty;
             return "test@example.com"; // Retorna email fixo
        }

        // Simula a renovação de um token (gera um novo token mock)
        public Task<string> RefreshTokenAsync(string token)
        {
            var userId = GetUserIdFromToken(token);
             if (string.IsNullOrEmpty(userId))
             {
                throw new Exception("Invalid mock token for refresh");
             }
            // Simula gerar um novo token para o mesmo usuário
            return Task.FromResult($"refreshed-mock-token-for-{userId}-{Guid.NewGuid()}");
        }

        // Simula a revogação de um token
        public Task RevokeTokenAsync(string token)
        {
            Console.WriteLine($"Mock RevokeTokenAsync called for token starting with: {token.Substring(0, Math.Min(token.Length, 20))}...");
            // Nenhuma ação real de revogação no mock
            return Task.CompletedTask;
        }
    }
} 