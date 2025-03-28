using MicroSaaS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Domain.Interfaces;

public interface IDashboardInsightsRepository
{
    Task<DashboardInsights> GetByIdAsync(Guid id);
    Task<List<DashboardInsights>> GetByCreatorIdAsync(Guid creatorId);
    Task<DashboardInsights> GetLatestByCreatorIdAsync(Guid creatorId);
    Task<DashboardInsights> AddAsync(DashboardInsights insights);
    Task<DashboardInsights> UpdateAsync(DashboardInsights insights);
    Task DeleteAsync(Guid id);
} 