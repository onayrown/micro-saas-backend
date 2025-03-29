using MicroSaaS.Application.DTOs;
using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Application.Interfaces.Repositories;
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

namespace MicroSaaS.IntegrationTests
{
    public class MockUserRepository : IUserRepository
    {
        public Task<User?> GetByIdAsync(Guid id)
        {
            return Task.FromResult<User?>(new User
            {
                Id = id,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            if (email == "test@example.com")
            {
                return Task.FromResult<User?>(new User
                {
                    Id = Guid.NewGuid(),
                    Username = "testuser",
                    Email = email,
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

        public Task<User> AddAsync(User user)
        {
            return Task.FromResult(user);
        }

        public Task<User> UpdateAsync(User user)
        {
            return Task.FromResult(user);
        }

        public Task DeleteAsync(Guid id)
        {
            return Task.CompletedTask;
        }
    }

    public class MockSocialMediaAccountRepository : ISocialMediaAccountRepository
    {
        public Task<SocialMediaAccount?> GetByIdAsync(Guid id)
        {
            return Task.FromResult<SocialMediaAccount?>(null);
        }

        public Task<IEnumerable<SocialMediaAccount>> GetByCreatorIdAsync(Guid creatorId)
        {
            return Task.FromResult<IEnumerable<SocialMediaAccount>>(new List<SocialMediaAccount>());
        }

        public Task<IEnumerable<SocialMediaAccount>> GetByPlatformAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            return Task.FromResult<IEnumerable<SocialMediaAccount>>(new List<SocialMediaAccount>());
        }

        public Task<SocialMediaAccount> AddAsync(SocialMediaAccount account)
        {
            return Task.FromResult(account);
        }

        public Task UpdateAsync(SocialMediaAccount account)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id)
        {
            return Task.CompletedTask;
        }

        public Task UpdateTokenAsync(Guid id, string accessToken, string refreshToken, DateTime expiresAt)
        {
            return Task.CompletedTask;
        }

        public Task<int> GetTotalFollowersAsync()
        {
            return Task.FromResult(1000);
        }

        public Task<int> GetTotalFollowersByCreatorAsync(Guid creatorId)
        {
            return Task.FromResult(500);
        }

        public Task RefreshSocialMediaMetricsAsync()
        {
            return Task.CompletedTask;
        }
    }

    public class MockTokenService : ITokenService
    {
        public string GenerateToken(User user)
        {
            return "test.jwt.token";
        }

        public bool ValidateToken(string token)
        {
            return true;
        }

        public Task<bool> ValidateTokenAsync(string token)
        {
            return Task.FromResult(true);
        }

        public Guid GetUserIdFromToken(string token)
        {
            return Guid.NewGuid();
        }

        public string GetUserEmailFromToken(string token)
        {
            return "test@example.com";
        }

        public Task<string> RefreshTokenAsync(string token)
        {
            return Task.FromResult("new.jwt.token");
        }

        public Task RevokeTokenAsync(string token)
        {
            return Task.CompletedTask;
        }
    }

    public class MockAuthService : IAuthService
    {
        public Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            if (request.Email == "test@example.com" && request.Password == "Test@123")
            {
                return Task.FromResult(new AuthResponse
                {
                    Token = "test.jwt.token",
                    User = new UserDto
                    {
                        Id = Guid.NewGuid(),
                        Username = "testuser",
                        Email = request.Email,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    }
                });
            }
            return Task.FromResult<AuthResponse>(null!);
        }

        public Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            return Task.FromResult(new AuthResponse
            {
                Token = "test.jwt.token",
                User = new UserDto
                {
                    Id = Guid.NewGuid(),
                    Username = request.Name,
                    Email = request.Email,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            });
        }

        public Task<AuthResponse> RefreshTokenAsync(string token)
        {
            return Task.FromResult(new AuthResponse
            {
                Token = "new.jwt.token",
                User = new UserDto
                {
                    Id = Guid.NewGuid(),
                    Username = "testuser",
                    Email = "test@example.com",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            });
        }

        public Task RevokeTokenAsync(string token)
        {
            return Task.CompletedTask;
        }

        public Task<bool> ValidateUserCredentialsAsync(string email, string password)
        {
            return Task.FromResult(email == "test@example.com" && password == "Test@123");
        }
    }

    public class MockSocialMediaIntegrationService : ISocialMediaIntegrationService
    {
        public Task<string> GetAuthUrlAsync(SocialMediaPlatform platform)
        {
            return Task.FromResult("https://auth.example.com");
        }

        public Task<SocialMediaAccount> HandleAuthCallbackAsync(SocialMediaPlatform platform, string code)
        {
            return Task.FromResult(new SocialMediaAccount
            {
                Id = Guid.NewGuid(),
                CreatorId = Guid.NewGuid(),
                Platform = platform,
                Username = "testuser",
                AccessToken = "test.access.token",
                RefreshToken = "test.refresh.token",
                TokenExpiresAt = DateTime.UtcNow.AddHours(1),
                IsActive = true
            });
        }

        public Task<bool> ValidateTokenAsync(SocialMediaAccount account)
        {
            return Task.FromResult(true);
        }

        public Task RefreshTokenAsync(SocialMediaAccount account)
        {
            return Task.CompletedTask;
        }

        public Task ConnectAccountAsync(SocialMediaAccount account)
        {
            return Task.CompletedTask;
        }

        public Task DisconnectAccountAsync(SocialMediaAccount account)
        {
            return Task.CompletedTask;
        }

        public Task<Dictionary<string, int>> GetAccountStatsAsync(SocialMediaAccount account)
        {
            return Task.FromResult(new Dictionary<string, int>());
        }

        public Task PostContentAsync(ContentPost post)
        {
            return Task.CompletedTask;
        }

        public Task SchedulePostAsync(ContentPost post, DateTime scheduledTime)
        {
            return Task.CompletedTask;
        }

        public Task CancelScheduledPostAsync(string postId)
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ContentPost>> GetScheduledPostsAsync(Guid creatorId)
        {
            return Task.FromResult<IEnumerable<ContentPost>>(new List<ContentPost>());
        }

        public Task<IEnumerable<ContentPost>> GetPublishedPostsAsync(Guid creatorId)
        {
            return Task.FromResult<IEnumerable<ContentPost>>(new List<ContentPost>());
        }

        public Task<IEnumerable<ContentPerformanceDto>> GetPostPerformanceAsync(string postId)
        {
            return Task.FromResult<IEnumerable<ContentPerformanceDto>>(new List<ContentPerformanceDto>());
        }

        public Task<IEnumerable<ContentPerformanceDto>> GetAccountPerformanceAsync(Guid accountId, DateTime startDate, DateTime endDate)
        {
            return Task.FromResult<IEnumerable<ContentPerformanceDto>>(new List<ContentPerformanceDto>());
        }

        public Task<Dictionary<string, decimal>> GetRevenueMetricsAsync(Guid accountId, DateTime startDate, DateTime endDate)
        {
            return Task.FromResult(new Dictionary<string, decimal>());
        }

        public Task<IEnumerable<MicroSaaS.Shared.Models.PostTimeRecommendation>> GetBestPostingTimesAsync(Guid accountId)
        {
            return Task.FromResult<IEnumerable<MicroSaaS.Shared.Models.PostTimeRecommendation>>(new List<MicroSaaS.Shared.Models.PostTimeRecommendation>());
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

        public Task<List<ContentRecommendation>> GetContentRecommendationsAsync(Guid creatorId)
        {
            var recommendations = new List<ContentRecommendation>
            {
                new ContentRecommendation
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Diversifique seus tópicos",
                    Description = "Baseado na análise do seu conteúdo, recomendamos expandir para tópicos relacionados para aumentar o alcance.",
                    Type = MicroSaaS.Shared.Enums.RecommendationType.ContentTopic,
                    SuggestedTopics = new List<string> { "Dicas práticas", "Tutoriais rápidos", "Histórias de sucesso" },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Priority = 1,
                    PotentialImpact = "Aumento potencial de 20% no alcance"
                },
                new ContentRecommendation
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = "Experimente vídeos curtos",
                    Description = "Vídeos de 60-90 segundos estão gerando maior engajamento em sua niche. Considere adaptar parte do seu conteúdo para este formato.",
                    Type = MicroSaaS.Shared.Enums.RecommendationType.ContentFormat,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Priority = 2,
                    PotentialImpact = "Aumento potencial de 35% no engajamento"
                }
            };
            
            return Task.FromResult(recommendations);
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
                    ExampleContentIds = new List<string> { "post123", "post456" },
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
                ExampleContentIds = new List<string> { "post123", "post456" },
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
                TopResponsiveTopics = new List<string> { "topic1", "topic2" },
                TopResponsiveFormats = new List<string> { "format1", "format2" },
                BestTimeOfDay = new List<TimeSpan> { new TimeSpan(12, 0, 0), new TimeSpan(18, 0, 0) },
                BestDaysOfWeek = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Friday },
                ConfidenceScore = 0.8m,
                LastUpdated = DateTime.UtcNow,
                SensitivityTopics = new List<MicroSaaS.Shared.DTOs.TopicSensitivityDto> 
                {
                    new MicroSaaS.Shared.DTOs.TopicSensitivityDto 
                    {
                        Topic = "Tópico sensível",
                        SensitivityLevel = 7,
                        RecommendedApproach = "Abordagem neutra"
                    }
                },
                OverallSensitivity = 7.0,
                Analysis = "Análise de exemplo para testes",
                RecommendedContentApproach = "Abordagem recomendada para testes"
            });
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
        public Task StartAsync()
        {
            return Task.CompletedTask;
        }

        public Task StopAsync()
        {
            return Task.CompletedTask;
        }

        public Task<ContentPost> SchedulePostAsync(ContentPost post)
        {
            post.Status = PostStatus.Scheduled;
            post.UpdatedAt = DateTime.UtcNow;
            return Task.FromResult(post);
        }

        public Task CancelScheduledPostAsync(Guid postId)
        {
            return Task.CompletedTask;
        }

        public Task ProcessScheduledPostsAsync()
        {
            return Task.CompletedTask;
        }

        public Task<IEnumerable<ContentPost>> GetScheduledPostsInRangeAsync(DateTime startDate, DateTime endDate)
        {
            var posts = new List<ContentPost>
            {
                new ContentPost
                {
                    Id = Guid.NewGuid(),
                    Title = "Scheduled Post 1",
                    Content = "Content for scheduled post 1",
                    Status = PostStatus.Scheduled,
                    ScheduledFor = DateTime.UtcNow.AddHours(2),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new ContentPost
                {
                    Id = Guid.NewGuid(),
                    Title = "Scheduled Post 2",
                    Content = "Content for scheduled post 2",
                    Status = PostStatus.Scheduled,
                    ScheduledFor = DateTime.UtcNow.AddHours(5),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };
            
            return Task.FromResult<IEnumerable<ContentPost>>(posts);
        }

        public Task SendUpcomingPostNotificationsAsync(int hoursAhead = 1)
        {
            return Task.CompletedTask;
        }

        // Implementação da interface IHostedService
        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            return StartAsync();
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            return StopAsync();
        }
    }
} 