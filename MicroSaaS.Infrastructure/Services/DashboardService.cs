using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Interfaces;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Services
{
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
                insights = await GenerateInsightsAsync(creatorId);
            }
            return insights;
        }

        public async Task<DashboardInsights> GenerateInsightsAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null)
        {
            return await _insightsService.GenerateInsightsAsync(creatorId);
        }

        public async Task<IEnumerable<PerformanceMetrics>> GetMetricsAsync(Guid creatorId, DateTime? startDate = null, DateTime? endDate = null, SocialMediaPlatform? platform = null)
        {
            var metrics = await _metricsRepository.GetByCreatorIdAsync(creatorId);

            // Aplicar filtros se fornecidos
            if (startDate.HasValue)
                metrics = metrics.Where(m => m.Date >= startDate.Value).ToList();

            if (endDate.HasValue)
                metrics = metrics.Where(m => m.Date <= endDate.Value).ToList();

            if (platform.HasValue)
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

        public async Task<List<PostTimeRecommendation>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
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

        public async Task<PerformanceMetrics> AddMetricsAsync(PerformanceMetrics metrics)
        {
            // Garantir que os timestamps estão definidos
            metrics.CreatedAt = DateTime.UtcNow;
            metrics.UpdatedAt = DateTime.UtcNow;

            return await _metricsRepository.AddAsync(metrics);
        }

        public async Task<ContentPerformance> AddContentPerformanceAsync(ContentPerformance performance)
        {
            // Garantir que o timestamp está definido
            performance.CollectedAt = DateTime.UtcNow;

            return await _contentPerformanceRepository.AddAsync(performance);
        }
    }
}