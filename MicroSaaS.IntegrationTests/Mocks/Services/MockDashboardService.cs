using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.IntegrationTests.Mocks.Services
{
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
            InitializeMetrics(creatorId);

            // Adicionar posts de exemplo
            InitializeContentPosts(creatorId);

            // Adicionar recomendações de horários
            InitializePostTimeRecommendations();
        }

        private void InitializeMetrics(Guid creatorId)
        {
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
        }

        private void InitializeContentPosts(Guid creatorId)
        {
            for (int i = 0; i < 5; i++)
            {
                _contentPosts.Add(new ContentPost
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = $"Post de teste {i + 1}",
                    Content = $"Conteúdo do post de teste {i + 1}",
                    MediaUrl = $"https://example.com/media/{i + 1}",
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
        }

        private void InitializePostTimeRecommendations()
        {
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
                insight = CreateDefaultInsights(creatorId);
                _insights.Add(insight);
            }
            
            return Task.FromResult(insight);
        }

        private DashboardInsights CreateDefaultInsights(Guid creatorId)
        {
            return new DashboardInsights
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

        public Task<IEnumerable<PerformanceMetrics>> GetMetricsAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null, SocialMediaPlatform? platform = null)
        {
            var query = _metrics.Where(m => m.CreatorId == creatorId);
            
            if (startDate.HasValue)
                query = query.Where(m => m.Date >= startDate.Value);
                
            if (endDate.HasValue)
                query = query.Where(m => m.Date <= endDate.Value);
                
            if (platform.HasValue)
                query = query.Where(m => m.Platform == platform.Value);
                
            return Task.FromResult(query.AsEnumerable());
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