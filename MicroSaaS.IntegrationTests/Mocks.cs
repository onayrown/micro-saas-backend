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
using System.Linq;

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
                    Priority = RecommendationPriority.High,
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
                    Priority = RecommendationPriority.Medium,
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
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
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
            metrics.Id = Guid.NewGuid();
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
                GeneratedDate = now,
                Platforms = new List<SocialMediaPlatform> { SocialMediaPlatform.Instagram, SocialMediaPlatform.YouTube, SocialMediaPlatform.TikTok },
                PeriodStart = start,
                PeriodEnd = end,
                GrowthRate = 4.2m,
                TotalRevenueInPeriod = 2800.00m,
                ComparisonWithPreviousPeriod = 15.2m,
                TopContentInsights = new List<ContentInsight>
                {
                    new ContentInsight
                    {
                        Id = Guid.NewGuid(),
                        Title = "Vídeo de tutorial tem alto engajamento",
                        Type = InsightType.HighEngagement,
                        Description = "Seus tutoriais estão gerando 2x mais engajamento que outros conteúdos",
                        RecommendedAction = "Postar mais vídeos tutoriais"
                    }
                },
                Recommendations = new List<ContentRecommendation>
                {
                    new ContentRecommendation
                    {
                        Id = Guid.NewGuid(),
                        Title = "Aumentar frequência no Instagram",
                        Description = "Aumente a frequência de publicações no Instagram para melhorar o alcance",
                        Priority = RecommendationPriority.High,
                        Type = MicroSaaS.Shared.Enums.RecommendationType.PostingFrequency
                    }
                },
                BestTimeToPost = _postTimeRecommendations,
                CreatedAt = now,
                UpdatedAt = now,
                Date = now.Date,
                TotalFollowers = 5500,
                TotalPosts = 140,
                TotalViews = 75000,
                TotalLikes = 22000,
                TotalComments = 4500,
                TotalShares = 2200,
                AverageEngagementRate = 4.8m,
                TotalRevenue = 3200.00m,
                Type = InsightType.Normal
            };
            
            _insights.Add(insight);
            return Task.FromResult(insight);
        }

        public Task<decimal> GetAverageEngagementRateAsync(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
        {
            var metrics = _metrics.Where(m => m.CreatorId == creatorId && m.Platform == platform).ToList();
            if (!metrics.Any())
            {
                return Task.FromResult(4.5m); // Valor padrão para testes
            }
            
            var avgEngagementRate = metrics.Average(m => m.EngagementRate);
            return Task.FromResult(avgEngagementRate);
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
                m.CreatorId == creatorId && 
                m.Date.Date == targetDate.Date && 
                m.Platform == platform);
                
            if (metric == null)
            {
                // Criar um exemplo se não encontrar
                metric = new PerformanceMetrics
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
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
            // Retornar um valor fixo para testes
            return Task.FromResult(180);
        }

        public Task<DashboardInsights> GetLatestInsightsAsync(Guid creatorId)
        {
            var insight = _insights.FirstOrDefault(i => i.CreatorId == creatorId);
            if (insight == null)
            {
                // Criar insights de exemplo se não existirem
                return GenerateInsightsAsync(creatorId);
            }
            
            return Task.FromResult(insight);
        }

        public Task<IEnumerable<PerformanceMetrics>> GetMetricsAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null, SocialMediaPlatform? platform = null)
        {
            // Filtrando
            var result = _metrics.Where(m => m.CreatorId == creatorId).AsEnumerable();
            
            if (startDate.HasValue)
                result = result.Where(m => m.Date >= startDate.Value);
                
            if (endDate.HasValue)
                result = result.Where(m => m.Date <= endDate.Value);
                
            if (platform.HasValue)
                result = result.Where(m => m.Platform == platform.Value);
            
            return Task.FromResult(result);
        }

        public Task<List<ContentRecommendation>> GetRecommendationsAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var recommendations = new List<ContentRecommendation>
            {
                new ContentRecommendation
                {
                    Id = Guid.NewGuid(),
                    Title = "Aumentar frequência de postagem no Instagram",
                    Description = "Análise mostra que aumentar a frequência de postagem em 30% pode melhorar seu alcance",
                    Priority = RecommendationPriority.High,
                    Type = MicroSaaS.Shared.Enums.RecommendationType.PostingFrequency
                },
                new ContentRecommendation
                {
                    Id = Guid.NewGuid(),
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