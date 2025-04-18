using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Application.DTOs.Performance;
using MicroSaaS.Application.DTOs;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.IntegrationTests.Mocks
{
    public class MockDashboardService : IDashboardService
    {
        // Listas em memória para simular dados
        private readonly List<DashboardInsights> _insights = new List<DashboardInsights>();
        private readonly List<PerformanceMetrics> _metrics = new List<PerformanceMetrics>();
        private readonly List<ContentPost> _contentPosts = new List<ContentPost>();
        private readonly List<ContentPerformance> _contentPerformances = new List<ContentPerformance>();

        // ID Fixo para testes
        private static readonly Guid FIXED_CREATOR_ID = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Propriedades para controlar o comportamento do mock
        public DashboardInsights? InsightsToReturn { get; set; }
        public IEnumerable<PerformanceMetrics>? MetricsToReturn { get; set; }
        public PerformanceMetrics? DailyMetricToReturn { get; set; }
        public List<ContentPost>? TopContentToReturn { get; set; }
        public List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>? BestTimeToPostToReturn { get; set; } // Usar DTO do Shared
        public decimal AvgEngagementRateToReturn { get; set; }
        public decimal RevenueGrowthToReturn { get; set; }
        public int FollowerGrowthToReturn { get; set; }
        public PerformanceMetrics? AddedMetricsToReturn { get; set; }
        public ContentPerformance? AddedContentPerformanceToReturn { get; set; }

        public MockDashboardService()
        {
            // Inicializar com dados mockados se as listas estiverem vazias
            if (!_metrics.Any())
            {
                InitializeMockData();
            }
        }

        private void InitializeMockData()
        {
            // Inicializa PerformanceMetrics com propriedades corretas
            _metrics.Add(new PerformanceMetrics
            {
                Id = Guid.NewGuid(),
                CreatorId = FIXED_CREATOR_ID,
                Platform = SocialMediaPlatform.Instagram,
                Date = DateTime.UtcNow.Date.AddDays(-1),
                Followers = 1000, // Usar Followers
                EngagementRate = 5.5m,
                Views = 5000, // Usar Views ou TotalViews
                TotalViews = 5000,
                EstimatedRevenue = 150.75m // Usar EstimatedRevenue
            });
             _metrics.Add(new PerformanceMetrics
            {
                Id = Guid.NewGuid(),
                CreatorId = FIXED_CREATOR_ID,
                Platform = SocialMediaPlatform.YouTube,
                Date = DateTime.UtcNow.Date.AddDays(-1),
                Followers = 5000,
                EngagementRate = 8.0m,
                Views = 20000,
                TotalViews = 20000,
                EstimatedRevenue = 450.00m
            });

            // Inicializa ContentPosts
            _contentPosts.Add(new ContentPost
            {
                Id = Guid.NewGuid(), CreatorId = FIXED_CREATOR_ID,
                Title = "Mock Post 1", Content = "Content for mock post 1",
                Platform = SocialMediaPlatform.Instagram,
                Status = PostStatus.Published, PublishedAt = DateTime.UtcNow.AddDays(-2)
            });
             _contentPosts.Add(new ContentPost
            {
                Id = Guid.NewGuid(), CreatorId = FIXED_CREATOR_ID,
                Title = "Mock Post 2", Content = "Content for mock post 2",
                Platform = SocialMediaPlatform.YouTube,
                Status = PostStatus.Published, PublishedAt = DateTime.UtcNow.AddDays(-3)
            });

             // Inicializa ContentPerformance
             if(_contentPosts.Count > 0)
             {
                _contentPerformances.Add(new ContentPerformance
                {
                    Id = Guid.NewGuid(), PostId = _contentPosts[0].Id, CreatorId = FIXED_CREATOR_ID, Platform = _contentPosts[0].Platform,
                    Likes = 150, Comments = 20, Shares = 5, Views = 1200,
                    Date = DateTime.UtcNow.AddDays(-1)
                });
             }

             // Inicializa DashboardInsights com propriedades corretas
             if(_contentPosts.Count > 0)
             {
                _insights.Add(new DashboardInsights
                {
                    Id = Guid.NewGuid(),
                    CreatorId = FIXED_CREATOR_ID,
                    GeneratedAt = DateTime.UtcNow,
                    Date = DateTime.UtcNow.Date,
                    PeriodStart = DateTime.UtcNow.AddDays(-7),
                    PeriodEnd = DateTime.UtcNow,
                    GrowthRate = 1.5m,
                    KeyInsights = new List<string> { "Insight 1", "Insight 2" },
                    ActionRecommendations = new List<string> { "Post more videos", "Engage with comments" },
                    Platforms = new List<SocialMediaPlatform> { SocialMediaPlatform.Instagram, SocialMediaPlatform.YouTube },
                    TotalFollowers = _metrics.Sum(m => m.Followers)
                });
             }
        }

        public Task<ContentPerformance> AddContentPerformanceAsync(MicroSaaS.Application.DTOs.ContentPerformanceDto performanceDto)
        {
            // A interface IDashboardService espera retornar Task<ContentPerformance> (Entidade)
            // O código anterior retornava Task<ContentPerformanceDto>
            // Mapear DTO para Entidade
            var entity = new ContentPerformance
            {
                 Id = Guid.NewGuid(), // Gerar novo ID para a entidade
                 PostId = performanceDto.PostId ?? Guid.Empty, // Usar PostId do DTO
                 CreatorId = Guid.Empty, // TODO: Obter CreatorId (não está no DTO atual)
                 Platform = performanceDto.Platform,
                 Date = performanceDto.Date,
                 CollectedAt = performanceDto.CollectedAt,
                 Views = performanceDto.Views,
                 Likes = performanceDto.Likes,
                 Comments = performanceDto.Comments,
                 Shares = performanceDto.Shares,
                 EstimatedRevenue = performanceDto.EstimatedRevenue,
                 // EngagementRate precisa ser calculado ou vir de outro lugar
                 CreatedAt = DateTime.UtcNow,
                 UpdatedAt = DateTime.UtcNow
            };

            // Adicionar a entidade à lista interna (se necessário)
             _contentPerformances.Add(entity);

            // Retornar a entidade mockada ou a entidade recém-criada
            return Task.FromResult(AddedContentPerformanceToReturn ?? entity);
        }

        public Task<PerformanceMetrics> AddMetricsAsync(DashboardMetricsDto metricsDto)
        {
            // A interface IDashboardService espera retornar Task<PerformanceMetrics> (Entidade)
            // O código anterior retornava Task<DashboardMetricsDto>
             // Mapear DTO para Entidade
            var entity = new PerformanceMetrics
            {
                 Id = Guid.NewGuid(), // Gerar novo ID para a entidade
                 // CreatorId = metricsDto.CreatorId, // Não existe CreatorId em DashboardMetricsDto
                 // Date = metricsDto.Date ?? DateTime.UtcNow.Date, // Não existe Date em DashboardMetricsDto
                 // Platform = metricsDto.Platform ?? SocialMediaPlatform.All, // Não existe Platform em DashboardMetricsDto
                 Followers = metricsDto.TotalCreators, // Mapeamento exemplo, ajustar conforme necessário
                 // FollowersGrowth = metricsDto.FollowersGrowth ?? 0, // Não existe
                 TotalViews = metricsDto.TotalPosts, // Mapeamento exemplo
                 // TotalLikes = metricsDto.TotalLikes ?? 0, // Não existe
                 // TotalComments = metricsDto.TotalComments ?? 0, // Não existe
                 // TotalShares = metricsDto.TotalShares ?? 0, // Não existe
                 EngagementRate = metricsDto.AverageEngagementRate // Mapeamento exemplo
                 // EstimatedRevenue = metricsDto.EstimatedRevenue ?? 0m // Não existe
                 // Adicionar outras propriedades da entidade
            };

            // Adicionar a entidade à lista interna (se necessário)
            _metrics.Add(entity);

            // Retornar a entidade mockada ou a entidade recém-criada
            return Task.FromResult(AddedMetricsToReturn ?? entity);
        }

        public Task<DashboardInsights> GenerateInsightsAsync(Guid creatorId)
        {
            var insights = _insights.FirstOrDefault(i => i.CreatorId == creatorId);
            if (insights == null)
            {
                 insights = new DashboardInsights
                 {
                     Id = Guid.NewGuid(),
                     CreatorId = creatorId,
                     GeneratedAt = DateTime.UtcNow,
                     Date = DateTime.UtcNow.Date,
                     PeriodStart = DateTime.UtcNow.AddDays(-7),
                     PeriodEnd = DateTime.UtcNow,
                     KeyInsights = new List<string> { "Mock Insight 1" },
                     ActionRecommendations = new List<string> { "Mock Action 1" }
                 };
            }
            return Task.FromResult(insights);
        }

        public Task<decimal> GetAverageEngagementRateAsync(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
        {
            var avgRate = _metrics.Where(m => m.CreatorId == creatorId && m.Platform == platform)
                                  .Average(m => (decimal?)m.EngagementRate) ?? 0m;
            return Task.FromResult(Math.Round(avgRate, 2));
        }

        public Task<List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
        {
            return Task.FromResult(BestTimeToPostToReturn ?? new List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>());
        }

        public Task<PerformanceMetrics> GetDailyMetricsAsync(Guid creatorId, DateTime? date = null, SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
        {
            var targetDate = date?.Date ?? DateTime.UtcNow.Date;
            var metric = _metrics.FirstOrDefault(m => m.CreatorId == creatorId && m.Platform == platform && m.Date.Date == targetDate);
            if (metric == null)
            {
                // Retorna métrica vazia ou padrão se não encontrada
                return Task.FromResult(new PerformanceMetrics { CreatorId = creatorId, Platform = platform, Date = targetDate });
            }
            return Task.FromResult(metric);
        }

        public Task<int> GetFollowerGrowthAsync(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram, DateTime? startDate = null, DateTime? endDate = null)
        {
            // Usar FollowersGrowth se disponível, senão calcular com Followers
            var startMetric = _metrics.Where(m => m.CreatorId == creatorId && m.Platform == platform && m.Date.Date >= (startDate?.Date ?? DateTime.MinValue))
                                         .OrderBy(m => m.Date)
                                         .FirstOrDefault();
            var endMetric = _metrics.Where(m => m.CreatorId == creatorId && m.Platform == platform && m.Date.Date <= (endDate?.Date ?? DateTime.MaxValue))
                                       .OrderByDescending(m => m.Date)
                                       .FirstOrDefault();

            if(startMetric != null && endMetric != null)
            {
                // Tentar usar FollowersGrowth se preenchido
                 if (startMetric.FollowersGrowth != 0 || endMetric.FollowersGrowth != 0)
                 {
                     // Simplificado: assumir que FollowersGrowth representa o crescimento no dia.
                     // Uma lógica real poderia somar os FollowersGrowth do período.
                     return Task.FromResult(endMetric.FollowersGrowth);
                 }
                 // Calcular com base na diferença de Followers
                 return Task.FromResult(endMetric.Followers - startMetric.Followers);
            }
            return Task.FromResult(0);
        }

        public Task<DashboardInsights> GetLatestInsightsAsync(Guid creatorId)
        {
            var latestInsight = _insights.Where(i => i.CreatorId == creatorId)
                                         .OrderByDescending(i => i.GeneratedAt)
                                         .FirstOrDefault();
             if (latestInsight == null)
            {
                 latestInsight = new DashboardInsights
                 {
                     CreatorId = creatorId,
                     GeneratedAt = DateTime.UtcNow,
                     Date = DateTime.UtcNow.Date,
                     KeyInsights = new List<string>{ "No insights found, generated mock." }
                };
            }
            return Task.FromResult(latestInsight);
        }

        public Task<IEnumerable<PerformanceMetrics>> GetMetricsAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null, SocialMediaPlatform? platform = null)
        {
            var query = _metrics.Where(m => m.CreatorId == creatorId);
            if (startDate.HasValue) query = query.Where(m => m.Date.Date >= startDate.Value.Date);
            if (endDate.HasValue) query = query.Where(m => m.Date.Date <= endDate.Value.Date);
            if (platform.HasValue) query = query.Where(m => m.Platform == platform.Value);

            return Task.FromResult<IEnumerable<PerformanceMetrics>>(query.OrderBy(m => m.Date).ToList());
        }

        public Task<decimal> GetRevenueGrowthAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var startRevenue = _metrics.Where(m => m.CreatorId == creatorId && m.Date.Date >= (startDate?.Date ?? DateTime.MinValue))
                                        .OrderBy(m => m.Date)
                                        .FirstOrDefault()?.EstimatedRevenue ?? 0m;
             var endRevenue = _metrics.Where(m => m.CreatorId == creatorId && m.Date.Date <= (endDate?.Date ?? DateTime.MaxValue))
                                       .OrderByDescending(m => m.Date)
                                       .FirstOrDefault()?.EstimatedRevenue ?? startRevenue;

            // Calcula o crescimento percentual, evitando divisão por zero
            if (startRevenue == 0)
            {
                 return Task.FromResult(endRevenue > 0 ? 100.0m : 0m); // Crescimento infinito ou 0
            }
            var growth = ((endRevenue - startRevenue) / startRevenue) * 100;
            return Task.FromResult(Math.Round(growth, 2));
        }

        public Task<List<ContentPost>> GetTopContentAsync(Guid creatorId, int limit = 5)
        {
            // Ordena por uma métrica simulada (ex: Likes da performance) e retorna o top N
             var topContent = _contentPosts
                .Where(p => p.CreatorId == creatorId && p.Status == PostStatus.Published)
                .Select(p => new
                {
                    Post = p,
                    Performance = _contentPerformances.FirstOrDefault(cp => cp.PostId == p.Id)
                })
                .OrderByDescending(x => x.Performance?.Likes ?? 0) // Ordena por likes (exemplo)
                .Take(limit)
                .Select(x => x.Post)
                .ToList();

            return Task.FromResult(topContent);
        }
    }
}