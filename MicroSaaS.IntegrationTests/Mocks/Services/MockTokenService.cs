using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MicroSaaS.IntegrationTests.Mocks.Services
{
    public class MockTokenService : ITokenService
    {
        public string GenerateToken(User user)
        {
            return "valid_token_for_testing";
        }

        public string GetUserIdFromToken(string token)
        {
            return token == "valid_token_for_testing" ? Guid.NewGuid().ToString() : string.Empty;
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            if (token == "valid_token_for_testing")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Email, "test@example.com"),
                    new Claim(ClaimTypes.Role, "user")
                };
                var identity = new ClaimsIdentity(claims, "TestAuth");
                return new ClaimsPrincipal(identity);
            }
            return new ClaimsPrincipal();
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            return Task.FromResult(token == "valid_token_for_testing");
        }

        public string GetUserEmailFromToken(string token)
        {
            return token == "valid_token_for_testing" ? "test@example.com" : string.Empty;
        }

        public Task<string> RefreshTokenAsync(string token)
        {
            return Task.FromResult("new_valid_token_for_testing");
        }

        public Task RevokeTokenAsync(string token)
        {
            return Task.CompletedTask;
        }
    }
} 