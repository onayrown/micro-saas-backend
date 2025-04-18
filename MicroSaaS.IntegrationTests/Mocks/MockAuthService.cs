using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Results;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MicroSaaS.IntegrationTests.Mocks; // Adicionado para usar outros mocks
using MicroSaaS.Application.DTOs; // Adicionar using para UserDto

namespace MicroSaaS.IntegrationTests.Mocks
{
    public class MockAuthService : IAuthService
    {
        // Dependências mock (poderiam ser injetadas, mas simplificado aqui)
        private readonly IUserRepository _userRepository = new MockUserRepository();
        private readonly ITokenService _tokenService = new MockTokenService();

        public Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
        {
            if (request.Email == "test@example.com" && request.Password == "password")
            {
                var user = _userRepository.GetByEmailAsync(request.Email).Result;
                if (user != null)
                {
                    var token = _tokenService.GenerateToken(user);
                    var userDto = new UserDto
                    {
                        Id = user.Id.ToString(),
                        Username = user.Username,
                        Email = user.Email,
                        Role = "User",
                        IsActive = user.IsActive
                    };
                    var response = AuthResponse.SuccessResponse(userDto, token);
                    return Task.FromResult(Result<AuthResponse>.Ok(response));
                }
            }
             if (request.Email == "fail@example.com")
            {
                 var failureResponse = AuthResponse.FailureResponse("Login failed for specific test case.");
                 return Task.FromResult(Result<AuthResponse>.Fail(failureResponse.Message ?? "Login failed"));
            }

            var defaultFailureResponse = AuthResponse.FailureResponse("Invalid credentials");
            return Task.FromResult(Result<AuthResponse>.Fail(defaultFailureResponse.Message ?? "Invalid credentials"));
        }

        public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            if (!await _userRepository.IsEmailUniqueAsync(request.Email))
            {
                var failureResponse = AuthResponse.FailureResponse("Email or Username already exists.");
                return Result<AuthResponse>.Fail(failureResponse.Message ?? "Registration failed");
            }

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Name,
                Email = request.Email,
                Name = request.Name,
                PasswordHash = "hashed_password",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(newUser);

            var token = _tokenService.GenerateToken(newUser);
            var userDto = new UserDto
            {
                Id = newUser.Id.ToString(),
                Username = newUser.Username,
                Email = newUser.Email,
                Role = "User",
                IsActive = newUser.IsActive
            };
            var response = AuthResponse.SuccessResponse(userDto, token);

            return Result<AuthResponse>.Ok(response);
        }

        public async Task<Result<AuthResponse>> RefreshTokenAsync(string token)
        {
            var isValid = await _tokenService.ValidateTokenAsync(token);
            if (!isValid)
            {
                 var failureResponse = AuthResponse.FailureResponse("Invalid token");
                 return Result<AuthResponse>.Fail(failureResponse.Message ?? "Invalid token");
            }

            var userIdStr = _tokenService.GetUserIdFromToken(token);
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                 var failureResponse = AuthResponse.FailureResponse("Cannot extract user ID from token");
                 return Result<AuthResponse>.Fail(failureResponse.Message ?? "Invalid token format");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                 var failureResponse = AuthResponse.FailureResponse("User not found for token refresh");
                 return Result<AuthResponse>.Fail(failureResponse.Message ?? "User not found");
            }

            var newToken = await _tokenService.RefreshTokenAsync(token);
            var userDto = new UserDto
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email,
                Role = "User",
                IsActive = user.IsActive
            };
            var response = AuthResponse.SuccessResponse(userDto, newToken);
            return Result<AuthResponse>.Ok(response);
        }

        public async Task<Result<bool>> RevokeTokenAsync(string token)
        {
            await _tokenService.RevokeTokenAsync(token);
            return Result<bool>.Ok(true);
        }

        public Task<Result<bool>> ValidateUserCredentialsAsync(string email, string password)
        {
            bool isValid = (email == "test@example.com" && password == "password");
            return Task.FromResult(isValid ? Result<bool>.Ok(isValid) : Result<bool>.Fail("Invalid credentials"));
        }

        public async Task<Result<UserProfileResponse>> GetUserProfileAsync(ClaimsPrincipal claimsPrincipal)
        {
            var userIdClaim = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                var failureResponse = UserProfileResponse.FailureResponse("Invalid user identifier in token");
                return Result<UserProfileResponse>.Fail(failureResponse.Message ?? "Invalid identifier");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                var failureResponse = UserProfileResponse.FailureResponse("User not found");
                return Result<UserProfileResponse>.Fail(failureResponse.Message ?? "User not found");
            }

            var profileData = new UserProfileData
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                Name = user.Name,
                Role = "User",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            var response = UserProfileResponse.SuccessResponse(profileData);
            return Result<UserProfileResponse>.Ok(response);
        }
    }
} 