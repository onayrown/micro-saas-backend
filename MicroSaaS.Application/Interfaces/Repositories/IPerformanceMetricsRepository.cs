using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Application.Interfaces.Repositories;

public interface IPerformanceMetricsRepository
{
    Task<PerformanceMetrics> GetByIdAsync(Guid id);
    Task<IEnumerable<PerformanceMetrics>> GetAllAsync();
    Task<IEnumerable<PerformanceMetrics>> GetByCreatorIdAsync(Guid creatorId);
    Task<IEnumerable<PerformanceMetrics>> GetByCreatorAndPlatformAsync(Guid creatorId, SocialMediaPlatform platform);
    Task<IEnumerable<PerformanceMetrics>> GetByDateRangeAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    Task<PerformanceMetrics> GetByCreatorAndDateAsync(Guid creatorId, DateTime date, SocialMediaPlatform platform);
    Task<PerformanceMetrics> AddAsync(PerformanceMetrics metrics);
    Task<PerformanceMetrics> UpdateAsync(PerformanceMetrics metrics);
    Task DeleteAsync(Guid id);
    Task<decimal> GetAverageEngagementRateAsync();
    Task<decimal> GetAverageEngagementRateByCreatorAsync(Guid creatorId);
    Task RefreshMetricsAsync();
} 