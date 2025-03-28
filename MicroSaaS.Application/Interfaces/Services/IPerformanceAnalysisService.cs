using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Application.DTOs.Performance;

namespace MicroSaaS.Application.Interfaces.Services;

public interface IPerformanceAnalysisService
{
    Task<PerformanceMetrics> GetDailyMetricsAsync(Guid creatorId, SocialMediaPlatform platform, DateTime date);
    Task<IEnumerable<PerformanceMetrics>> GetMetricsTimelineAsync(Guid creatorId, DateTime startDate, DateTime endDate, SocialMediaPlatform? platform = null);
    Task<DashboardInsights> GenerateInsightsAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    Task<List<ContentPost>> GetTopPerformingContentAsync(Guid creatorId, int limit = 5);
    Task<List<MicroSaaS.Domain.Entities.PostTimeRecommendation>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform);
    Task<decimal> CalculateAverageEngagementRateAsync(Guid creatorId, SocialMediaPlatform platform);
    Task<decimal> CalculateRevenueGrowthAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    Task<int> CalculateFollowerGrowthAsync(Guid creatorId, SocialMediaPlatform platform, DateTime startDate, DateTime endDate);
    Task<DashboardMetricsDto> GetDashboardMetricsAsync();
    Task<ContentMetricsDto> GetContentMetricsAsync(string contentId);
    Task<CreatorMetricsDto> GetCreatorMetricsAsync(string creatorId);
    Task RefreshMetricsAsync();
} 