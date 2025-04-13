using MicroSaaS.Application.DTOs;
using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.IntegrationTests.Models;
using MicroSaaS.Shared.Results;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MicroSaaS.IntegrationTests.Mocks.Services
{
    public class MockAuthService : IAuthService
    {
        public Task<Result<MicroSaaS.Application.DTOs.Auth.AuthResponse>> LoginAsync(LoginRequest request)
        {
            if (request.Email == "test@example.com" && request.Password == "Test@123")
            {
                var userDto = new MicroSaaS.IntegrationTests.Models.UserDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Username = "testuser",
                    Name = "Test User",
                    Email = request.Email,
                    Role = "user",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                
                return Task.FromResult(Result<MicroSaaS.Application.DTOs.Auth.AuthResponse>.Ok(new MicroSaaS.Application.DTOs.Auth.AuthResponse
                {
                    Success = true,
                    Token = "test.jwt.token",
                    User = new MicroSaaS.Application.DTOs.UserDto
                    {
                        Id = userDto.Id,
                        Username = userDto.Username,
                        Email = userDto.Email,
                        Role = userDto.Role,
                        IsActive = userDto.IsActive
                    },
                    Message = "Login successful"
                }));
            }
            return Task.FromResult(Result<MicroSaaS.Application.DTOs.Auth.AuthResponse>.Fail("Credenciais inválidas"));
        }

        public Task<Result<MicroSaaS.Application.DTOs.Auth.AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            var userDto = new MicroSaaS.IntegrationTests.Models.UserDto
            {
                Id = Guid.NewGuid().ToString(),
                Username = request.Name,
                Name = request.Name,
                Email = request.Email,
                Role = "user",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            return Task.FromResult(Result<MicroSaaS.Application.DTOs.Auth.AuthResponse>.Ok(new MicroSaaS.Application.DTOs.Auth.AuthResponse
            {
                Success = true,
                Token = "test.jwt.token",
                User = new MicroSaaS.Application.DTOs.UserDto
                {
                    Id = userDto.Id,
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Role = userDto.Role,
                    IsActive = userDto.IsActive
                },
                Message = "Registration successful"
            }));
        }

        public Task<Result<MicroSaaS.Application.DTOs.Auth.AuthResponse>> RefreshTokenAsync(string token)
        {
            var userDto = new MicroSaaS.IntegrationTests.Models.UserDto
            {
                Id = Guid.NewGuid().ToString(),
                Username = "testuser",
                Name = "Test User",
                Email = "test@example.com",
                Role = "user",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            return Task.FromResult(Result<MicroSaaS.Application.DTOs.Auth.AuthResponse>.Ok(new MicroSaaS.Application.DTOs.Auth.AuthResponse
            {
                Success = true,
                Token = "new.jwt.token",
                User = new MicroSaaS.Application.DTOs.UserDto
                {
                    Id = userDto.Id,
                    Username = userDto.Username,
                    Email = userDto.Email,
                    Role = userDto.Role,
                    IsActive = userDto.IsActive
                },
                Message = "Token refreshed successfully"
            }));
        }

        public Task<Result<bool>> RevokeTokenAsync(string token)
        {
            return Task.FromResult(Result<bool>.Ok(true));
        }

        public Task<Result<bool>> ValidateUserCredentialsAsync(string email, string password)
        {
            var isValid = email == "test@example.com" && password == "Test@123";
            return Task.FromResult(Result<bool>.Ok(isValid));
        }

        public Task<Result<UserProfileResponse>> GetUserProfileAsync(ClaimsPrincipal claimsPrincipal)
        {
            // Implementação de mock para o método GetUserProfileAsync
            var userId = Guid.NewGuid();
            var userProfileData = new UserProfileData
            {
                Id = userId.ToString(),
                Name = "Test User",
                Email = "test@example.com",
                Role = "user",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ProfileImageUrl = null
            };
            
            var response = UserProfileResponse.SuccessResponse(userProfileData);
            return Task.FromResult(Result<UserProfileResponse>.Ok(response));
        }
    }
} 