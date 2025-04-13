using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Interfaces.Repositories;

public interface IDashboardInsightsRepository
{
    Task<DashboardInsights?> GetByIdAsync(Guid id);
    Task<IEnumerable<DashboardInsights>> GetAllAsync();
    Task<IEnumerable<DashboardInsights>> GetByCreatorIdAsync(Guid creatorId);
    Task<DashboardInsights?> GetLatestByCreatorIdAsync(Guid creatorId);
    Task<DashboardInsights?> GetByCreatorAndPeriodAsync(Guid creatorId, DateTime periodStart, DateTime periodEnd);
    Task<DashboardInsights> AddAsync(DashboardInsights insights);
    Task<DashboardInsights?> UpdateAsync(DashboardInsights insights);
    Task DeleteAsync(Guid id);
    Task<List<DashboardInsights>> GetByCreatorIdAndDateRangeAsync(Guid creatorId, DateTime startDate, DateTime endDate, InsightType type);
    Task<List<DashboardInsights>> GetByCreatorIdAsync(Guid creatorId, InsightType type);
} 