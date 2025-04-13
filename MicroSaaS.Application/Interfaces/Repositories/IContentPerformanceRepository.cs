using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Application.Interfaces.Repositories;

public interface IContentPerformanceRepository
{
    Task<ContentPerformance> GetByIdAsync(Guid id);
    Task<IEnumerable<ContentPerformance>> GetAllAsync();
    Task<IEnumerable<ContentPerformance>> GetByPostIdAsync(string? postId);
    Task<IEnumerable<ContentPerformance>> GetByCreatorIdAsync(Guid creatorId);
    Task<IEnumerable<ContentPerformance>> GetByPlatformAsync(SocialMediaPlatform platform);
    Task<IEnumerable<ContentPerformance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<ContentPerformance>> GetTopPerformingByViewsAsync(Guid creatorId, int limit = 10);
    Task<IEnumerable<ContentPerformance>> GetTopPerformingByEngagementAsync(Guid creatorId, int limit = 10);
    Task<IEnumerable<ContentPerformance>> GetTopPerformingByRevenueAsync(Guid creatorId, int limit = 10);
    Task<ContentPerformance> AddAsync(ContentPerformance performance);
    Task<ContentPerformance> UpdateAsync(ContentPerformance performance);
    Task DeleteAsync(Guid id);
    Task<decimal> GetAverageEngagementRateAsync(Guid creatorId);
    Task<decimal> GetAverageEngagementRateByCreatorAsync(Guid creatorId);
    Task RefreshMetricsAsync();
} 