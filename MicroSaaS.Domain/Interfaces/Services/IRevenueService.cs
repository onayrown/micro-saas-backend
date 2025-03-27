using MicroSaaS.Shared.Models;

namespace MicroSaaS.Domain.Interfaces.Services;

public interface IRevenueService
{
    Task<RevenueSummary> GetRevenueSummaryAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<DailyRevenue>> GetDailyRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<PlatformRevenue>> GetPlatformRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate);
} 