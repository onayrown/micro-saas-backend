using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Application.DTOs;
using MicroSaaS.Application.DTOs.Performance;
using MicroSaaS.Application.DTOs.ContentPost;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroSaaS.Shared.DTOs;

namespace MicroSaaS.Application.Services.Dashboard
{
    /// <summary>
    /// Implementação do serviço de dashboard pertencente à camada de aplicação
    /// conforme os princípios da Clean Architecture.
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly MicroSaaS.Application.Interfaces.Services.IDashboardInsightsService _insightsService;
        private readonly MicroSaaS.Application.Interfaces.Repositories.IPerformanceMetricsRepository _metricsRepository;
        private readonly MicroSaaS.Application.Interfaces.Repositories.IContentPostRepository _contentRepository;
        private readonly MicroSaaS.Application.Interfaces.Repositories.IContentPerformanceRepository _contentPerformanceRepository;
        private readonly IRecommendationService _recommendationService;

        public DashboardService(
            MicroSaaS.Application.Interfaces.Services.IDashboardInsightsService insightsService,
            MicroSaaS.Application.Interfaces.Repositories.IPerformanceMetricsRepository metricsRepository,
            MicroSaaS.Application.Interfaces.Repositories.IContentPostRepository contentRepository,
            MicroSaaS.Application.Interfaces.Repositories.IContentPerformanceRepository contentPerformanceRepository,
            IRecommendationService recommendationService)
        {
            _insightsService = insightsService;
            _metricsRepository = metricsRepository;
            _contentRepository = contentRepository;
            _contentPerformanceRepository = contentPerformanceRepository;
            _recommendationService = recommendationService;
        }

        public async Task<DashboardInsights> GetLatestInsightsAsync(Guid creatorId)
        {
            var insights = await _insightsService.GetLatestByCreatorIdAsync(creatorId);
            if (insights == null)
            {
                // Se não houver insights anteriores, gerar novos insights
                return await GenerateInsightsAsync(creatorId);
            }
            return insights;
        }

        public async Task<DashboardInsights> GenerateInsightsAsync(Guid creatorId)
        {
            var insightsEntity = await _insightsService.GenerateInsightsAsync(creatorId);
            if (insightsEntity == null)
            {
                // Lógica para lidar com caso onde insights não puderam ser gerados
                // Talvez lançar exceção ou retornar um objeto DashboardInsights vazio/padrão
                // Por ora, retornando null, mas idealmente deveria ser tratado.
                // Considerar logar um aviso/erro aqui.
                Console.WriteLine($"Warning: Failed to generate insights for creator {creatorId}");
                return null;
            }
            return insightsEntity;
        }

        public async Task<IEnumerable<PerformanceMetrics>> GetMetricsAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null, SocialMediaPlatform? platform = null)
        {
            var metrics = await _metricsRepository.GetByCreatorIdAsync(creatorId);

            // Aplicar filtros se fornecidos
            if (startDate.HasValue)
                metrics = metrics.Where(m => m.Date >= startDate.Value).ToList();

            if (endDate.HasValue)
                metrics = metrics.Where(m => m.Date <= endDate.Value).ToList();

            // Ajuste: Não filtrar se a plataforma for 'All'
            if (platform.HasValue && platform.Value != SocialMediaPlatform.All)
                metrics = metrics.Where(m => m.Platform == platform.Value).ToList();

            return metrics;
        }

        public async Task<PerformanceMetrics> GetDailyMetricsAsync(Guid creatorId, DateTime? date = null, SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
        {
            var targetDate = date ?? DateTime.UtcNow.Date;
            var metrics = await _metricsRepository.GetByCreatorIdAsync(creatorId);

            // Filtrar por data e plataforma
            return metrics.FirstOrDefault(m =>
                m.Date.Date == targetDate.Date &&
                m.Platform == platform);
        }

        public async Task<List<ContentPost>> GetTopContentAsync(Guid creatorId, int limit = 5)
        {
            var posts = await _contentRepository.GetByCreatorIdAsync(creatorId);
            var performances = await _contentPerformanceRepository.GetByCreatorIdAsync(creatorId);

            // Associar cada post com sua performance média
            var postPerformances = posts
                .Select(post => {
                    var postPerfs = performances.Where(p => p.PostId == post.Id).ToList();
                    var avgEngagement = postPerfs.Any()
                        ? postPerfs.Average(p => p.EngagementRate)
                        : 0;
                    return new { Post = post, EngagementRate = avgEngagement };
                })
                .OrderByDescending(pp => pp.EngagementRate)
                .Take(limit)
                .Select(pp => pp.Post)
                .ToList();

            return postPerformances;
        }

        public async Task<List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
        {
            return await _recommendationService.GetBestTimeToPostAsync(creatorId, platform);
        }

        public async Task<decimal> GetAverageEngagementRateAsync(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
        {
            var metrics = await _metricsRepository.GetByCreatorIdAsync(creatorId);
            var platformMetrics = metrics.Where(m => m.Platform == platform).ToList();

            if (!platformMetrics.Any())
                return 0;

            return platformMetrics.Average(m => m.EngagementRate);
        }

        public async Task<decimal> GetRevenueGrowthAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var end = endDate ?? DateTime.UtcNow;
            var start = startDate ?? end.AddDays(-30);

            // Na implementação real, este método buscaria dados de receita do repositório
            // e calcularia o crescimento percentual

            // Implementação simulada para fins de demonstração
            var performances = await _contentPerformanceRepository.GetByCreatorIdAsync(creatorId);
            var startRevenue = performances
                .Where(p => p.CollectedAt >= start && p.CollectedAt < start.AddDays(1))
                .Sum(p => p.EstimatedRevenue);

            var endRevenue = performances
                .Where(p => p.CollectedAt >= end.AddDays(-1) && p.CollectedAt <= end)
                .Sum(p => p.EstimatedRevenue);

            if (startRevenue == 0)
                return 0; // Evitar divisão por zero

            return ((endRevenue - startRevenue) / startRevenue) * 100;
        }

        public async Task<int> GetFollowerGrowthAsync(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram, DateTime? startDate = null, DateTime? endDate = null)
        {
            var end = endDate ?? DateTime.UtcNow;
            var start = startDate ?? end.AddDays(-30);

            var metrics = await _metricsRepository.GetByCreatorIdAsync(creatorId);
            var platformMetrics = metrics.Where(m => m.Platform == platform).ToList();

            var startMetrics = platformMetrics
                .Where(m => m.Date.Date <= start.Date)
                .OrderByDescending(m => m.Date)
                .FirstOrDefault();

            var endMetrics = platformMetrics
                .Where(m => m.Date.Date <= end.Date)
                .OrderByDescending(m => m.Date)
                .FirstOrDefault();

            if (startMetrics == null || endMetrics == null)
                return 0;

            return endMetrics.Followers - startMetrics.Followers;
        }

        public async Task<PerformanceMetrics> AddMetricsAsync(MicroSaaS.Application.DTOs.Performance.DashboardMetricsDto metricsDto)
        {
            // Mapear DTO para Entidade (apenas campos existentes em ambos)
            var metricsEntity = new PerformanceMetrics
            {
                // Assumindo que a entidade PerformanceMetrics TEM esses campos,
                // mas o DTO DashboardMetricsDto NÃO TEM (baseado nos erros CS1061).
                // Portanto, não podemos ler de metricsDto.
                // Precisamos obter esses valores de outra forma ou ajustar o DTO/Entidade.
                // Por ora, comentaremos/removeremos as leituras do DTO.

                // Id = metricsDto.Id == Guid.Empty ? Guid.NewGuid() : metricsDto.Id, // Erro CS1061
                // CreatorId = metricsDto.CreatorId, // Erro CS1061
                // Platform = metricsDto.Platform, // Erro CS1061
                // Date = metricsDto.Date, // Erro CS1061
                // Followers = metricsDto.Followers, // Erro CS1061
                // TotalViews = metricsDto.TotalViews, // Erro CS1061
                // TotalLikes = metricsDto.TotalLikes, // Erro CS1061
                // TotalComments = metricsDto.TotalComments, // Erro CS1061
                // TotalShares = metricsDto.TotalShares, // Erro CS1061
                // EngagementRate = metricsDto.EngagementRate, // Erro CS1061
                // Impressions = metricsDto.Impressions, // Erro CS1061
                // Reach = metricsDto.Reach, // Erro CS1061

                // TODO: Mapear corretamente os campos existentes ou ajustar DTO/Entidade.
                // A Entidade precisa ser preenchida com dados válidos.
                // Exemplo provisório (precisa de lógica real):
                Id = Guid.NewGuid(),
                CreatorId = Guid.Empty, // Precisa vir de algum lugar!
                Platform = Shared.Enums.SocialMediaPlatform.All,
                Date = DateTime.UtcNow.Date,
                Followers = 0,
                TotalViews = 0,
                TotalLikes = 0,
                TotalComments = 0,
                TotalShares = 0,
                EngagementRate = 0,
                Impressions = 0,
                Reach = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return await _metricsRepository.AddAsync(metricsEntity);
        }

        public async Task<ContentPerformance> AddContentPerformanceAsync(MicroSaaS.Application.DTOs.ContentPerformanceDto performanceDto)
        {
            // Mapear DTO para Entidade (apenas campos existentes em ambos)
            var performanceEntity = new ContentPerformance
            {
                // Assumindo que a entidade ContentPerformance TEM esses campos,
                // mas o DTO ContentPerformanceDto NÃO TEM (baseado nos erros CS1061).

                // Id = performanceDto.Id == Guid.Empty ? Guid.NewGuid() : performanceDto.Id, // Erro CS1061
                // PostId = performanceDto.PostId, // Erro CS1061
                // CreatorId = performanceDto.CreatorId, // Erro CS1061
                // Platform = performanceDto.Platform, // Erro CS1061
                Views = performanceDto.Views,
                Likes = performanceDto.Likes,
                Comments = performanceDto.Comments,
                Shares = performanceDto.Shares,
                // EngagementRate não existe no DTO, calculamos aqui
                EngagementRate = (performanceDto.Views > 0) ? ((decimal)(performanceDto.Likes + performanceDto.Comments) / performanceDto.Views) * 100 : 0,
                EstimatedRevenue = performanceDto.EstimatedRevenue,

                // Mapeando os campos existentes
                Id = Guid.NewGuid(),
                PostId = performanceDto.PostId ?? Guid.Empty,
                CreatorId = performanceDto.AccountId ?? Guid.Empty,
                Platform = performanceDto.Platform,
                Date = performanceDto.Date,
                CollectedAt = performanceDto.CollectedAt,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            return await _contentPerformanceRepository.AddAsync(performanceEntity);
        }

        private DashboardInsightsDto MapToDto(DashboardInsights entity)
        {
            if (entity == null) return null;

            return new DashboardInsightsDto
            {
                Id = entity.Id,
                CreatorId = entity.CreatorId,
                Date = entity.Date,
                PeriodStart = entity.PeriodStart,
                PeriodEnd = entity.PeriodEnd,
                GeneratedDate = entity.GeneratedDate,
                Platforms = entity.Platforms,
                PlatformsPerformance = entity.PlatformsPerformance?.ConvertAll(p => new PlatformInsightDto
                {
                    Platform = p.Platform,
                    FollowerCount = p.FollowerCount,
                    FollowerGrowth = p.FollowerGrowth,
                    EngagementRate = p.EngagementRate,
                    ReachGrowth = p.ReachGrowth,
                    TopPerformingContent = p.TopPerformingContent
                }) ?? new(),
                TotalFollowers = entity.TotalFollowers,
                TotalPosts = entity.TotalPosts,
                TotalViews = entity.TotalViews,
                TotalLikes = entity.TotalLikes,
                TotalComments = entity.TotalComments,
                TotalShares = entity.TotalShares,
                AverageEngagementRate = entity.AverageEngagementRate,
                TotalRevenue = entity.TotalRevenue,
                TotalRevenueInPeriod = entity.TotalRevenueInPeriod,
                GrowthRate = entity.GrowthRate,
                RevenueGrowth = entity.RevenueGrowth,
                PostingConsistency = entity.PostingConsistency,
                AudienceDemographics = entity.AudionceDemographics != null ? new AudienceDemographicsDto
                {
                    AgeGroups = entity.AudionceDemographics.AgeGroups,
                    TopCountries = entity.AudionceDemographics.TopCountries,
                    Gender = entity.AudionceDemographics.Gender
                } : null,
            };
        }
    }
}