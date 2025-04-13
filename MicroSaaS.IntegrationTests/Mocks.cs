using MicroSaaS.Application.DTOs;
using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using MicroSaaS.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Hosting;
using System.Linq;
using MicroSaaS.Shared.Results;
using System.Security.Claims;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.IntegrationTests.Models;

namespace MicroSaaS.IntegrationTests
{
    public class MockUserRepository : IUserRepository
    {
        public Task<User?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Parse("10000000-1000-1000-1000-100000000000"))
            {
                return Task.FromResult<User?>(new User
                {
                    Id = id,
                    Username = "Test User",
                    Name = "Test User",
                    Email = "test@example.com",
                    PasswordHash = "hashed_password",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            return Task.FromResult<User?>(null);
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            if (email == "test@example.com")
            {
                return Task.FromResult<User?>(new User
                {
                    Id = Guid.Parse("10000000-1000-1000-1000-100000000000"),
                    Username = "Test User",
                    Name = "Test User",
                    Email = email,
                    PasswordHash = "hashed_password",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            return Task.FromResult<User?>(null);
        }

        public Task<User?> GetByUsernameAsync(string username)
        {
            if (username == "testuser")
            {
                return Task.FromResult<User?>(new User
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    Email = "test@example.com",
                    PasswordHash = "hashedpassword",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
            return Task.FromResult<User?>(null);
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            return Task.FromResult<IEnumerable<User>>(new List<User>());
        }

        public Task AddAsync(User user)
        {
            // Apenas retorna uma tarefa concluída
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User user)
        {
            // Apenas retorna uma tarefa concluída
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            return Task.CompletedTask;
        }

        public Task<bool> IsEmailUniqueAsync(string email)
        {
            // Considera o email único se não for test@example.com
            return Task.FromResult(email != "test@example.com");
        }

        public Task<bool> IsUsernameUniqueAsync(string username)
        {
            // Considera o username único se não for testuser
            return Task.FromResult(username != "testuser");
        }
    }

    public class MockSocialMediaAccountRepository : ISocialMediaAccountRepository
    {
        private readonly List<SocialMediaAccount> _accounts = new();
        
        // IDs fixos para testes consistentes
        private static readonly Guid ACCOUNT_ID_1 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid ACCOUNT_ID_2 = Guid.Parse("33333333-3333-3333-3333-333333333333");
        private static readonly Guid CREATOR_ID_1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        
        // Construtor que inicializa contas padrão para testes
        public MockSocialMediaAccountRepository()
        {
            // Inicializar com algumas contas de teste se a lista estiver vazia
            if (_accounts.Count == 0)
            {
                _accounts.Add(new SocialMediaAccount
                {
                    Id = ACCOUNT_ID_1,
                    CreatorId = CREATOR_ID_1,
                    Platform = SocialMediaPlatform.Instagram,
                    Username = "testcreator1_instagram",
                    AccessToken = "token_instagram_1",
                    RefreshToken = "refresh_instagram_1",
                    TokenExpiresAt = DateTime.UtcNow.AddDays(60),
                    IsActive = true,
                    ProfileUrl = "profile_instagram_1",
                    CreatedAt = DateTime.UtcNow.AddDays(-25),
                    UpdatedAt = DateTime.UtcNow.AddDays(-25)
                });
                
                _accounts.Add(new SocialMediaAccount
                {
                    Id = ACCOUNT_ID_2,
                    CreatorId = CREATOR_ID_1,
                    Platform = SocialMediaPlatform.YouTube,
                    Username = "testcreator1_youtube",
                    AccessToken = "token_youtube_1",
                    RefreshToken = "refresh_youtube_1",
                    TokenExpiresAt = DateTime.UtcNow.AddDays(60),
                    IsActive = true,
                    ProfileUrl = "profile_youtube_1",
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    UpdatedAt = DateTime.UtcNow.AddDays(-20)
                });
            }
        }

        public Task<SocialMediaAccount> AddAsync(SocialMediaAccount account)
        {
            // Se o ID não foi especificado, atribuir um novo
            account.Id = account.Id == Guid.Empty ? Guid.NewGuid() : account.Id;
            _accounts.Add(account);
            return Task.FromResult(account);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var account = _accounts.FirstOrDefault(a => a.Id == id);
            if (account != null)
            {
                _accounts.Remove(account);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<SocialMediaAccount?> GetByIdAsync(Guid id)
        {
            return Task.FromResult(_accounts.FirstOrDefault(a => a.Id == id));
        }

        public Task<IEnumerable<SocialMediaAccount>> GetByCreatorIdAsync(Guid creatorId)
        {
            return Task.FromResult(_accounts.Where(a => a.CreatorId == creatorId));
        }

        public Task<SocialMediaAccount?> GetByCreatorIdAndPlatformAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            return Task.FromResult(_accounts.FirstOrDefault(a => a.CreatorId == creatorId && a.Platform == platform));
        }

        public Task<SocialMediaAccount> UpdateAsync(SocialMediaAccount account)
        {
            var existingAccount = _accounts.FirstOrDefault(a => a.Id == account.Id);
            if (existingAccount != null)
            {
                _accounts.Remove(existingAccount);
                _accounts.Add(account);
                return Task.FromResult(account);
            }
            throw new InvalidOperationException("Account not found for update");
        }

        public Task UpdateTokenAsync(Guid id, string accessToken, string refreshToken, DateTime expiresAt)
        {
            var account = _accounts.FirstOrDefault(a => a.Id == id);
            if (account != null)
            {
                account.AccessToken = accessToken;
                account.RefreshToken = refreshToken;
                account.TokenExpiresAt = expiresAt;
            }
            return Task.CompletedTask;
        }

        // Método para verificar se existe um criador
        public Task<bool> CreatorExistsAsync(Guid creatorId)
        {
            // Para fins de teste, vamos considerar que qualquer ID fornecido existe
            return Task.FromResult(true);
        }

        // Método para obter contas por plataforma
        public Task<IEnumerable<SocialMediaAccount>> GetByPlatformAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            return Task.FromResult<IEnumerable<SocialMediaAccount>>(new List<SocialMediaAccount>());
        }

        // Método para obter o total de seguidores
        public Task<int> GetTotalFollowersAsync()
        {
            return Task.FromResult(1000);
        }

        // Método para obter o total de seguidores por criador
        public Task<int> GetTotalFollowersByCreatorAsync(Guid creatorId)
        {
            return Task.FromResult(500);
        }

        // Método para atualizar métricas de redes sociais
        public Task RefreshSocialMediaMetricsAsync()
        {
            return Task.CompletedTask;
        }
    }

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

    public class MockSocialMediaIntegrationService : ISocialMediaIntegrationService
    {
        public Task<string> GetAuthUrlAsync(SocialMediaPlatform platform)
        {
            return Task.FromResult($"https://auth.test.com/{platform.ToString().ToLower()}/authorize");
        }

        public Task<SocialMediaAccount> HandleAuthCallbackAsync(SocialMediaPlatform platform, string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("Invalid authorization code", nameof(code));
            }

            var account = new SocialMediaAccount
            {
                Id = Guid.NewGuid(),
                CreatorId = Guid.NewGuid(),
                Platform = platform,
                Username = $"test_user_{platform.ToString().ToLower()}",
                AccessToken = "mock_token",
                RefreshToken = "mock_refresh_token",
                TokenExpiresAt = DateTime.UtcNow.AddDays(30),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return Task.FromResult(account);
        }

        public Task<bool> ValidateTokenAsync(SocialMediaAccount account)
        {
            // Considera todos os tokens válidos para teste
            return Task.FromResult(true);
        }

        public Task RefreshTokenAsync(SocialMediaAccount account)
        {
            // Atualiza o token da conta mock
            account.AccessToken = "new_mock_token";
            account.RefreshToken = "new_mock_refresh_token";
            account.TokenExpiresAt = DateTime.UtcNow.AddDays(30);
            
            return Task.CompletedTask;
        }

        public Task ConnectAccountAsync(SocialMediaAccount account)
        {
            // Simula a conexão de uma conta
            return Task.CompletedTask;
        }

        public Task DisconnectAccountAsync(SocialMediaAccount account)
        {
            // Simula a desconexão de uma conta
            return Task.CompletedTask;
        }

        public Task<Dictionary<string, int>> GetAccountStatsAsync(SocialMediaAccount account)
        {
            // Retorna estatísticas de exemplo para a conta
            return Task.FromResult(new Dictionary<string, int>
            {
                { "followers", 1000 },
                { "following", 500 },
                { "posts", 50 }
            });
        }

        public Task PostContentAsync(ContentPost post)
        {
            // Simula o post de conteúdo
            post.Status = PostStatus.Published;
            post.PublishedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }

        public Task SchedulePostAsync(ContentPost post, DateTime scheduledTime)
        {
            // Simula o agendamento de um post
            post.ScheduledTime = scheduledTime;
            return Task.CompletedTask;
        }

        public Task CancelScheduledPostAsync(Guid postId)
        {
            // Simula o cancelamento de um post agendado
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ContentPost>> GetScheduledPostsAsync(Guid creatorId)
        {
            // Retornar alguns posts agendados de exemplo
            var posts = new List<ContentPost>
            {
                new ContentPost
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Post agendado 1",
                    Content = "Conteúdo do post agendado 1",
                    Platform = SocialMediaPlatform.Instagram,
                    Status = PostStatus.Scheduled,
                    ScheduledTime = DateTime.UtcNow.AddDays(1),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new ContentPost
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Post agendado 2",
                    Content = "Conteúdo do post agendado 2",
                    Platform = SocialMediaPlatform.YouTube,
                    Status = PostStatus.Scheduled,
                    ScheduledTime = DateTime.UtcNow.AddDays(2),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
            
            return Task.FromResult<IEnumerable<ContentPost>>(posts);
        }

        public Task<IEnumerable<ContentPost>> GetPublishedPostsAsync(Guid creatorId)
        {
            // Retornar alguns posts publicados de exemplo
            var posts = new List<ContentPost>
            {
                new ContentPost
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Post publicado 1",
                    Content = "Conteúdo do post publicado 1",
                    Platform = SocialMediaPlatform.Instagram,
                    Status = PostStatus.Published,
                    PublishedAt = DateTime.UtcNow.AddDays(-1),
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new ContentPost
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Post publicado 2",
                    Content = "Conteúdo do post publicado 2",
                    Platform = SocialMediaPlatform.YouTube,
                    Status = PostStatus.Published,
                    PublishedAt = DateTime.UtcNow.AddDays(-2),
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                }
            };
            
            return Task.FromResult<IEnumerable<ContentPost>>(posts);
        }

        public Task<IEnumerable<MicroSaaS.Application.DTOs.ContentPerformanceDto>> GetPostPerformanceAsync(Guid postId)
        {
            // Retorna dados de performance de exemplo para um post
            var performances = new List<MicroSaaS.Application.DTOs.ContentPerformanceDto>
            {
                new MicroSaaS.Application.DTOs.ContentPerformanceDto
                {
                    PostId = postId,
                    Platform = SocialMediaPlatform.Instagram,
                    Likes = 100,
                    Comments = 25,
                    Shares = 10,
                    Views = 1000,
                    Date = DateTime.UtcNow.AddDays(-1)
                }
            };
            
            return Task.FromResult<IEnumerable<MicroSaaS.Application.DTOs.ContentPerformanceDto>>(performances);
        }

        public Task<IEnumerable<MicroSaaS.Application.DTOs.ContentPerformanceDto>> GetAccountPerformanceAsync(Guid accountId, DateTime startDate, DateTime endDate)
        {
            // Retorna dados de performance de exemplo para uma conta
            var performances = new List<MicroSaaS.Application.DTOs.ContentPerformanceDto>
            {
                new MicroSaaS.Application.DTOs.ContentPerformanceDto
                {
                    AccountId = accountId,
                    Platform = SocialMediaPlatform.Instagram,
                    Likes = 500,
                    Comments = 120,
                    Shares = 50,
                    Views = 5000,
                    Date = DateTime.UtcNow.AddDays(-7)
                }
            };
            
            return Task.FromResult<IEnumerable<MicroSaaS.Application.DTOs.ContentPerformanceDto>>(performances);
        }

        public Task<Dictionary<string, decimal>> GetRevenueMetricsAsync(Guid accountId, DateTime startDate, DateTime endDate)
        {
            // Retornar métricas de receita de exemplo para testes
            var revenueMetrics = new Dictionary<string, decimal>
            {
                { "Total", 1200.00m },
                { "Direta", 800.00m },
                { "Afiliados", 250.00m },
                { "Parcerias", 150.00m }
            };
            
            return Task.FromResult(revenueMetrics);
        }

        public Task<IEnumerable<MicroSaaS.Shared.Models.PostTimeRecommendation>> GetBestPostingTimesAsync(Guid accountId)
        {
            // Retorna recomendações de horário de exemplo
            var recommendations = new List<MicroSaaS.Shared.Models.PostTimeRecommendation>
            {
                new MicroSaaS.Shared.Models.PostTimeRecommendation
                {
                    DayOfWeek = DayOfWeek.Monday,
                    TimeOfDay = new TimeSpan(18, 0, 0),
                    EngagementScore = 0.85m
                },
                new MicroSaaS.Shared.Models.PostTimeRecommendation
                {
                    DayOfWeek = DayOfWeek.Wednesday,
                    TimeOfDay = new TimeSpan(12, 0, 0),
                    EngagementScore = 0.75m
                }
            };
            
            return Task.FromResult<IEnumerable<MicroSaaS.Shared.Models.PostTimeRecommendation>>(recommendations);
        }

        public Task<List<ContentRecommendation>> GetContentRecommendationsAsync(Guid creatorId)
        {
            // Criar uma recomendação de exemplo para o criador de conteúdo
            var recommendation = new ContentRecommendation
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Title = "Use mais hashtags relacionadas a tecnologia",
                Description = "Nossa análise mostra que posts com hashtags como #tecnologia #inovação #programação têm desempenho 30% melhor",
                Priority = RecommendationPriority.High,
                Type = MicroSaaS.Shared.Enums.RecommendationType.ContentTopic,
                SuggestedTopics = new List<string> { "Tecnologia", "Programação", "Desenvolvimento" },
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };
            
            return Task.FromResult(new List<ContentRecommendation> { recommendation });
        }
    }

    public class MockRecommendationService : IRecommendationService
    {
        public Task<List<MicroSaaS.Domain.Entities.PostTimeRecommendation>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            // Gerar recomendações de teste para hora de postagem
            var recommendations = new List<MicroSaaS.Domain.Entities.PostTimeRecommendation>
            {
                new MicroSaaS.Domain.Entities.PostTimeRecommendation 
                { 
                    DayOfWeek = DayOfWeek.Monday, 
                    TimeOfDay = new TimeSpan(12, 0, 0),
                    EngagementScore = 7.5
                },
                new MicroSaaS.Domain.Entities.PostTimeRecommendation 
                { 
                    DayOfWeek = DayOfWeek.Wednesday, 
                    TimeOfDay = new TimeSpan(18, 0, 0),
                    EngagementScore = 8.2
                },
                new MicroSaaS.Domain.Entities.PostTimeRecommendation 
                { 
                    DayOfWeek = DayOfWeek.Friday, 
                    TimeOfDay = new TimeSpan(17, 0, 0),
                    EngagementScore = 8.7
                }
            };
            
            return Task.FromResult(recommendations);
        }

        public Task<Dictionary<SocialMediaPlatform, List<MicroSaaS.Domain.Entities.PostTimeRecommendation>>> GetBestTimeToPostAllPlatformsAsync(Guid creatorId)
        {
            var result = new Dictionary<SocialMediaPlatform, List<MicroSaaS.Domain.Entities.PostTimeRecommendation>>
            {
                { 
                    SocialMediaPlatform.Instagram, 
                    new List<MicroSaaS.Domain.Entities.PostTimeRecommendation>
                    {
                        new MicroSaaS.Domain.Entities.PostTimeRecommendation 
                        { 
                            DayOfWeek = DayOfWeek.Monday, 
                            TimeOfDay = new TimeSpan(12, 0, 0),
                            EngagementScore = 7.5
                        }
                    }
                },
                { 
                    SocialMediaPlatform.YouTube, 
                    new List<MicroSaaS.Domain.Entities.PostTimeRecommendation>
                    {
                        new MicroSaaS.Domain.Entities.PostTimeRecommendation 
                        { 
                            DayOfWeek = DayOfWeek.Tuesday, 
                            TimeOfDay = new TimeSpan(16, 0, 0),
                            EngagementScore = 8.0
                        }
                    }
                }
            };
            
            return Task.FromResult(result);
        }

        // Implementar outros métodos conforme necessário
        public Task<List<ContentRecommendation>> GetTopicRecommendationsAsync(Guid creatorId)
        {
            return Task.FromResult(new List<ContentRecommendation>());
        }

        public Task<List<ContentRecommendation>> GetFormatRecommendationsAsync(Guid creatorId)
        {
            return Task.FromResult(new List<ContentRecommendation>());
        }

        public Task<List<string>> GetHashtagRecommendationsAsync(Guid creatorId, string contentDescription, SocialMediaPlatform platform)
        {
            return Task.FromResult(new List<string> { "#exemplo", "#teste", "#mock" });
        }

        public Task<List<TrendTopic>> GetTrendingTopicsAsync(SocialMediaPlatform platform)
        {
            return Task.FromResult(new List<TrendTopic>());
        }

        public Task<List<TrendTopic>> GetNicheTrendingTopicsAsync(Guid creatorId)
        {
            return Task.FromResult(new List<TrendTopic>());
        }

        public Task<List<ContentRecommendation>> GetMonetizationRecommendationsAsync(Guid creatorId)
        {
            return Task.FromResult(new List<ContentRecommendation>());
        }

        public Task<List<ContentRecommendation>> GetAudienceGrowthRecommendationsAsync(Guid creatorId)
        {
            return Task.FromResult(new List<ContentRecommendation>());
        }

        public Task<List<ContentRecommendation>> GetEngagementImprovementRecommendationsAsync(Guid creatorId)
        {
            return Task.FromResult(new List<ContentRecommendation>());
        }

        public Task<ContentAnalysis> AnalyzeContentPerformanceAsync(Guid contentId)
        {
            return Task.FromResult(new ContentAnalysis());
        }

        public Task RefreshRecommendationsAsync(Guid creatorId)
        {
            return Task.CompletedTask;
        }

        public Task<List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>> GetBestPostingTimesAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            return Task.FromResult(new List<MicroSaaS.Shared.DTOs.BestTimeSlotDto> 
            {
                new MicroSaaS.Shared.DTOs.BestTimeSlotDto 
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Platform = platform,
                    DayOfWeek = DayOfWeek.Monday,
                    TimeOfDay = new TimeSpan(12, 0, 0),
                    Hour = 12,
                    EngagementScore = 8.5m,
                    ConfidenceScore = 0.85m,
                    EngagementPotential = 85,
                    RecommendationStrength = "Alto",
                    CreatedAt = DateTime.UtcNow
                }
            });
        }

        public Task<List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>> GetContentRecommendationsAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            return Task.FromResult(new List<MicroSaaS.Shared.DTOs.ContentRecommendationDto> 
            {
                new MicroSaaS.Shared.DTOs.ContentRecommendationDto 
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Recomendação de teste",
                    Description = "Esta é uma recomendação de teste para testes de integração",
                    Type = MicroSaaS.Shared.DTOs.RecommendationType.Topic,
                    Platform = platform,
                    ConfidenceScore = 0.9m,
                    ExampleContentIds = new List<Guid> { Guid.Parse("00000000-0000-0000-0000-000000000123"), Guid.Parse("00000000-0000-0000-0000-000000000456") },
                    SuggestedHashtags = new List<string> { "#teste", "#integracao" },
                    SuggestedKeywords = new List<string> { "teste", "integração" },
                    CreatedAt = DateTime.UtcNow
                }
            });
        }

        public Task<List<MicroSaaS.Shared.DTOs.GrowthRecommendationDto>> GetGrowthRecommendationsAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            return Task.FromResult(new List<MicroSaaS.Shared.DTOs.GrowthRecommendationDto> 
            {
                new MicroSaaS.Shared.DTOs.GrowthRecommendationDto 
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Recomendação de crescimento",
                    Description = "Esta é uma recomendação de crescimento para testes",
                    Category = MicroSaaS.Shared.DTOs.GrowthCategory.Engagement,
                    Platform = platform,
                    ImplementationSteps = new List<string> { "Passo 1", "Passo 2" },
                    ExpectedOutcome = "Resultado esperado para teste",
                    Difficulty = 2,
                    TimeToImplement = "1 semana",
                    CreatedAt = DateTime.UtcNow
                }
            });
        }

        public Task<MicroSaaS.Shared.DTOs.ContentRecommendationDto> GenerateCustomRecommendationAsync(MicroSaaS.Shared.DTOs.CustomRecommendationRequestDto request)
        {
            return Task.FromResult(new MicroSaaS.Shared.DTOs.ContentRecommendationDto 
            {
                Id = Guid.NewGuid(),
                CreatorId = request.CreatorId,
                Title = "Recomendação personalizada",
                Description = "Esta é uma recomendação personalizada para testes",
                Type = request.RecommendationType,
                Platform = request.Platform,
                ConfidenceScore = 0.85m,
                ExampleContentIds = new List<Guid> { Guid.Parse("00000000-0000-0000-0000-000000000123"), Guid.Parse("00000000-0000-0000-0000-000000000456") },
                SuggestedHashtags = new List<string> { "#personalizado", "#teste" },
                SuggestedKeywords = new List<string> { "personalizado", "teste" },
                CreatedAt = DateTime.UtcNow
            });
        }

        public Task<MicroSaaS.Shared.DTOs.AudienceSensitivityDto> GetAudienceSensitivityAnalysisAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            return Task.FromResult(new MicroSaaS.Shared.DTOs.AudienceSensitivityDto
            {
                CreatorId = creatorId,
                Platform = platform.ToString(),
                TopResponsiveTopics = new List<string> { "Educação", "Tecnologia" },
                TopResponsiveFormats = new List<string> { "Vídeos curtos", "Carrosséis" },
                BestTimeOfDay = new List<TimeSpan> { new TimeSpan(12, 0, 0), new TimeSpan(18, 0, 0) },
                BestDaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Friday },
                ConfidenceScore = 0.85m,
                LastUpdated = DateTime.UtcNow,
                SensitivityTopics = new List<MicroSaaS.Shared.DTOs.TopicSensitivityDto> 
                {
                    new MicroSaaS.Shared.DTOs.TopicSensitivityDto 
                    {
                        Topic = "Política",
                        SensitivityLevel = 8,
                        RecommendedApproach = "Evitar ou abordar com neutralidade"
                    },
                    new MicroSaaS.Shared.DTOs.TopicSensitivityDto 
                    {
                        Topic = "Saúde",
                        SensitivityLevel = 6,
                        RecommendedApproach = "Abordar com responsabilidade e fontes"
                    }
                },
                OverallSensitivity = 7.0,
                Analysis = "Sua audiência é sensível a tópicos políticos e prefere conteúdo educacional",
                RecommendedContentApproach = "Foque em conteúdo educacional e evite polarizações"
            });
        }

        public Task<MicroSaaS.Shared.DTOs.GrowthRecommendationDto> GenerateCustomGrowthRecommendationAsync(MicroSaaS.Shared.DTOs.CustomRecommendationRequestDto request)
        {
            return Task.FromResult(new MicroSaaS.Shared.DTOs.GrowthRecommendationDto
            {
                Id = Guid.NewGuid(),
                CreatorId = request.CreatorId,
                Title = "Recomendação de crescimento personalizada",
                Description = "Esta é uma recomendação de crescimento personalizada para testes",
                Category = MicroSaaS.Shared.DTOs.GrowthCategory.ContentQuality,
                Platform = request.Platform,
                ImplementationSteps = new List<string> { "Passo 1 personalizado", "Passo 2 personalizado" },
                ExpectedOutcome = "Resultado esperado personalizado para teste",
                Difficulty = 3,
                TimeToImplement = "2 semanas",
                CreatedAt = DateTime.UtcNow
            });
        }

        public Task<List<MicroSaaS.Domain.Entities.ContentRecommendation>> GetContentRecommendationsAsync(Guid creatorId)
        {
            return Task.FromResult(new List<MicroSaaS.Domain.Entities.ContentRecommendation>
            {
                new MicroSaaS.Domain.Entities.ContentRecommendation
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Recomendação de conteúdo teste",
                    Description = "Esta é uma recomendação de conteúdo para testes de mock",
                    Type = MicroSaaS.Shared.Enums.RecommendationType.Topic,
                    Platform = SocialMediaPlatform.Instagram,
                    CreatedAt = DateTime.UtcNow
                }
            });
        }

        public Task<List<ContentRecommendation>> GetRecommendationsAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var recommendations = new List<ContentRecommendation>
            {
                new ContentRecommendation
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Aumentar frequência de postagem no Instagram",
                    Description = "Análise mostra que aumentar a frequência de postagem em 30% pode melhorar seu alcance",
                    Priority = RecommendationPriority.High,
                    Type = MicroSaaS.Shared.Enums.RecommendationType.PostingFrequency
                },
                new ContentRecommendation
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Criar conteúdo em formato carrossel",
                    Description = "Conteúdos em formato carrossel têm engajamento 25% maior",
                    Priority = RecommendationPriority.Medium,
                    Type = MicroSaaS.Shared.Enums.RecommendationType.ContentFormat
                }
            };

            return Task.FromResult(recommendations);
        }
    }

    public class MockLoggingService : ILoggingService
    {
        public void LogInformation(string message, params object[] args)
        {
            Console.WriteLine($"[INFO] {string.Format(message, args)}");
        }

        public void LogWarning(string message, params object[] args)
        {
            Console.WriteLine($"[WARN] {string.Format(message, args)}");
        }

        public void LogError(Exception ex, string message, params object[] args)
        {
            Console.WriteLine($"[ERROR] {string.Format(message, args)}: {ex?.Message}");
        }

        public void LogDebug(string message, params object[] args)
        {
            Console.WriteLine($"[DEBUG] {string.Format(message, args)}");
        }

        public void LogCritical(Exception ex, string message, params object[] args)
        {
            Console.WriteLine($"[CRITICAL] {string.Format(message, args)}: {ex?.Message}");
        }
    }

    public class MockSchedulerService : ISchedulerService, IHostedService
    {
        private readonly List<ScheduledPostDto> _scheduledPosts = new();

        public Task<ContentPost> SchedulePostAsync(ContentPost post)
        {
            return Task.FromResult(post);
        }

        public Task CancelScheduledPostAsync(Guid postId)
        {
            // Simula o cancelamento de um post agendado
            var post = _scheduledPosts.FirstOrDefault(p => p.Id == postId);
            if (post != null)
            {
                post.Status = PostStatus.Cancelled;
                post.UpdatedAt = DateTime.UtcNow;
            }
            return Task.CompletedTask;
        }

        public Task ProcessScheduledPostsAsync()
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ContentPost>> GetScheduledPostsInRangeAsync(DateTime startDate, DateTime endDate)
        {
            return Task.FromResult<IEnumerable<ContentPost>>(new List<ContentPost>());
        }

        public Task SendUpcomingPostNotificationsAsync(int hoursAhead = 1)
        {
            return Task.CompletedTask;
        }

        public Task StartAsync()
        {
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            return Task.CompletedTask;
        }

        // Métodos adicionais para suportar testes de integração
        public Task<ScheduledPostDto> SchedulePostAsync(CreateScheduledPostDto request)
        {
            var post = new ScheduledPostDto
            {
                Id = Guid.NewGuid(),
                CreatorId = request.CreatorId,
                Title = request.Title,
                Content = request.Content,
                ScheduledFor = request.ScheduledFor,
                Platform = request.Platform,
                MediaUrls = request.MediaUrls ?? new List<string>(),
                Tags = request.Tags ?? new List<string>(),
                Status = PostStatus.Scheduled,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _scheduledPosts.Add(post);
            return Task.FromResult(post);
        }

        public Task<ScheduledPostDto> GetScheduledPostAsync(Guid id)
        {
            var post = _scheduledPosts.FirstOrDefault(p => p.Id == id);
            return Task.FromResult(post);
        }

        public Task<ScheduledPostDto> UpdateScheduledPostAsync(Guid id, UpdateScheduledPostDto request)
        {
            var post = _scheduledPosts.FirstOrDefault(p => p.Id == id);
            if (post == null)
                return Task.FromResult<ScheduledPostDto>(null);

            if (request.Title != null)
                post.Title = request.Title;
            
            if (request.Content != null)
                post.Content = request.Content;
            
            if (request.ScheduledFor.HasValue)
                post.ScheduledFor = request.ScheduledFor.Value;
            
            if (request.MediaUrls != null)
                post.MediaUrls = request.MediaUrls;
            
            if (request.Tags != null)
                post.Tags = request.Tags;
            
            post.UpdatedAt = DateTime.UtcNow;
            
            return Task.FromResult(post);
        }

        public Task<List<ScheduledPostDto>> GetScheduledPostsInRangeDtoAsync(DateTime startDate, DateTime endDate)
        {
            var posts = _scheduledPosts
                .Where(p => p.ScheduledFor >= startDate && p.ScheduledFor <= endDate)
                .ToList();
            
            return Task.FromResult(posts);
        }

        Task<IEnumerable<Domain.Entities.ContentPost>> ISchedulerService.GetScheduledPostsInRangeAsync(DateTime startDate, DateTime endDate)
        {
            return Task.FromResult<IEnumerable<Domain.Entities.ContentPost>>(new List<Domain.Entities.ContentPost>());
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class MockDashboardService : IDashboardService
    {
        private readonly List<DashboardInsights> _insights = new List<DashboardInsights>();
        private readonly List<PerformanceMetrics> _metrics = new List<PerformanceMetrics>();
        private readonly List<ContentPost> _contentPosts = new List<ContentPost>();
        private readonly List<ContentPerformance> _contentPerformances = new List<ContentPerformance>();
        private readonly List<MicroSaaS.Domain.Entities.PostTimeRecommendation> _postTimeRecommendations = new List<MicroSaaS.Domain.Entities.PostTimeRecommendation>();

        public MockDashboardService()
        {
            // Inicializa alguns dados de exemplo para testes
            var creatorId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Adicionar métricas de exemplo
            for (int i = 0; i < 10; i++)
            {
                _metrics.Add(new PerformanceMetrics
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatorId = creatorId.ToString(),
                    Date = DateTime.UtcNow.AddDays(-i),
                    Platform = i % 2 == 0 ? SocialMediaPlatform.Instagram : SocialMediaPlatform.YouTube,
                    Followers = 5000 + (i * 50),
                    FollowersGrowth = i * 10,
                    TotalViews = 1500 + (i * 100),
                    TotalLikes = 750 + (i * 50),
                    TotalComments = 120 + (i * 10),
                    TotalShares = 60 + (i * 5),
                    EngagementRate = 4.8m - (i * 0.1m),
                    EstimatedRevenue = 350.00m + (i * 25.00m),
                    CreatedAt = DateTime.UtcNow.AddDays(-i),
                    UpdatedAt = DateTime.UtcNow.AddDays(-i)
                });
            }

            // Adicionar posts de exemplo
            for (int i = 0; i < 5; i++)
            {
                _contentPosts.Add(new ContentPost
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = $"Post de teste {i+1}",
                    Content = $"Conteúdo do post de teste {i+1}",
                    MediaUrl = $"https://example.com/media/{i+1}",
                    Platform = i % 3 == 0 ? SocialMediaPlatform.Instagram : 
                              i % 3 == 1 ? SocialMediaPlatform.YouTube : SocialMediaPlatform.TikTok,
                    Status = PostStatus.Published,
                    PublishedAt = DateTime.UtcNow.AddDays(-i),
                    Views = 1000 - (i * 90),
                    Likes = 500 - (i * 45),
                    Comments = 100 - (i * 9),
                    Shares = 50 - (i * 4),
                    EngagementRate = 5.0m - (i * 0.3m),
                    CreatedAt = DateTime.UtcNow.AddDays(-i - 1),
                    UpdatedAt = DateTime.UtcNow.AddDays(-i)
                });
            }

            // Adicionar recomendações de horários
            _postTimeRecommendations.Add(new MicroSaaS.Domain.Entities.PostTimeRecommendation
            {
                DayOfWeek = DayOfWeek.Monday,
                TimeOfDay = new TimeSpan(18, 0, 0),
                EngagementScore = 8.5
            });
            _postTimeRecommendations.Add(new MicroSaaS.Domain.Entities.PostTimeRecommendation
            {
                DayOfWeek = DayOfWeek.Wednesday,
                TimeOfDay = new TimeSpan(12, 0, 0),
                EngagementScore = 9.2
            });
            _postTimeRecommendations.Add(new MicroSaaS.Domain.Entities.PostTimeRecommendation
            {
                DayOfWeek = DayOfWeek.Friday,
                TimeOfDay = new TimeSpan(20, 0, 0),
                EngagementScore = 8.8
            });
        }

        public Task<ContentPerformance> AddContentPerformanceAsync(ContentPerformance performance)
        {
            performance.Id = Guid.NewGuid();
            performance.CollectedAt = DateTime.UtcNow;
            performance.CreatedAt = DateTime.UtcNow;
            performance.UpdatedAt = DateTime.UtcNow;
            
            _contentPerformances.Add(performance);
            return Task.FromResult(performance);
        }

        public Task<PerformanceMetrics> AddMetricsAsync(PerformanceMetrics metrics)
        {
            metrics.Id = Guid.NewGuid().ToString();
            metrics.CreatedAt = DateTime.UtcNow;
            metrics.UpdatedAt = DateTime.UtcNow;
            
            _metrics.Add(metrics);
            return Task.FromResult(metrics);
        }

        public Task<DashboardInsights> GenerateInsightsAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;
            var now = DateTime.UtcNow;
            
            var insight = new DashboardInsights
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                AverageEngagementRate = 4.5m,
                GrowthRate = 2.8m,
                TotalPosts = 125,
                TotalFollowers = 10000,
                TotalLikes = 5000,
                TotalComments = 1200,
                TotalShares = 800,
                TotalViews = 50000,
                TotalRevenue = 2500m,
                Date = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Platforms = new List<SocialMediaPlatform> { SocialMediaPlatform.Instagram },
                KeyInsights = new List<string> { "Vídeos têm melhor desempenho", "Melhor horário para postar é às 18:00" }
            };
            
            _insights.Add(insight);
            return Task.FromResult(insight);
        }

        public Task<decimal> GetAverageEngagementRateAsync(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
        {
            var performances = _contentPerformances
                .Where(p => p.CreatorId == creatorId && p.Platform == platform)
                .ToList();
            
            if (!performances.Any())
                return Task.FromResult(0m);
                
            var avgRate = performances.Average(p => p.EngagementRate);
            return Task.FromResult(avgRate);
        }

        public Task<List<MicroSaaS.Domain.Entities.PostTimeRecommendation>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
        {
            // Retorna as recomendações pré-definidas
            return Task.FromResult(_postTimeRecommendations);
        }

        public Task<PerformanceMetrics> GetDailyMetricsAsync(Guid creatorId, DateTime? date = null, SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
        {
            var targetDate = date ?? DateTime.UtcNow.Date;
            var metric = _metrics.FirstOrDefault(m => 
                m.CreatorId == creatorId.ToString() && 
                m.Date.Date == targetDate.Date && 
                m.Platform == platform);
                
            if (metric == null)
            {
                // Criar um exemplo se não encontrar
                metric = new PerformanceMetrics
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatorId = creatorId.ToString(),
                    Date = targetDate,
                    Platform = platform,
                    Followers = 5000,
                    FollowersGrowth = 50,
                    TotalViews = 1500,
                    TotalLikes = 750,
                    TotalComments = 120,
                    TotalShares = 60,
                    EngagementRate = 4.8m,
                    EstimatedRevenue = 350.00m,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _metrics.Add(metric);
            }
            
            return Task.FromResult(metric);
        }

        public Task<int> GetFollowerGrowthAsync(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Valor padrão para testes
            if (creatorId.ToString() == "11111111-1111-1111-1111-111111111111")
            {
                return Task.FromResult(120); // Crescimento especial para o criador de teste
            }
            
            return Task.FromResult(75); // Crescimento padrão
        }

        public Task<DashboardInsights> GetLatestInsightsAsync(Guid creatorId)
        {
            var insight = _insights.FirstOrDefault(i => i.CreatorId == creatorId);
            
            if (insight == null)
            {
                // Criar percepções padrão para testes
                insight = new DashboardInsights
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    AverageEngagementRate = 4.5m,
                    GrowthRate = 2.8m,
                    TotalPosts = 125,
                    TotalFollowers = 10000,
                    TotalLikes = 5000,
                    TotalComments = 1200,
                    TotalShares = 800,
                    TotalViews = 50000,
                    TotalRevenue = 2500m,
                    Date = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Platforms = new List<SocialMediaPlatform> { SocialMediaPlatform.Instagram },
                    KeyInsights = new List<string> { "Vídeos têm melhor desempenho", "Melhor horário para postar é às 18:00" }
                };
            }
            
            return Task.FromResult(insight);
        }

        public Task<IEnumerable<PerformanceMetrics>> GetMetricsAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null, SocialMediaPlatform? platform = null)
        {
            var metrics = _metrics
                .Where(m => m.CreatorId == creatorId.ToString())
                .ToList();

            if (platform.HasValue)
            {
                metrics = metrics.Where(m => m.Platform == platform.Value).ToList();
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                metrics = metrics.Where(m => m.Date >= startDate.Value && m.Date <= endDate.Value).ToList();
            }

            return Task.FromResult(metrics.AsEnumerable());
        }

        public Task<List<ContentRecommendation>> GetRecommendationsAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var recommendations = new List<ContentRecommendation>
            {
                new ContentRecommendation
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Aumentar frequência de postagem no Instagram",
                    Description = "Análise mostra que aumentar a frequência de postagem em 30% pode melhorar seu alcance",
                    Priority = RecommendationPriority.High,
                    Type = MicroSaaS.Shared.Enums.RecommendationType.PostingFrequency
                },
                new ContentRecommendation
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Criar conteúdo em formato carrossel",
                    Description = "Conteúdos em formato carrossel têm engajamento 25% maior",
                    Priority = RecommendationPriority.Medium,
                    Type = MicroSaaS.Shared.Enums.RecommendationType.ContentFormat
                }
            };

            return Task.FromResult(recommendations);
        }

        public Task<decimal> GetRevenueGrowthAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Retornar um valor fixo para testes
            return Task.FromResult(15.2m);
        }

        public Task<List<ContentPost>> GetTopContentAsync(Guid creatorId, int limit = 5)
        {
            var topPosts = _contentPosts
                .Where(p => p.CreatorId == creatorId)
                .OrderByDescending(p => p.EngagementRate)
                .Take(limit)
                .ToList();
                
            return Task.FromResult(topPosts);
        }
    }
} 