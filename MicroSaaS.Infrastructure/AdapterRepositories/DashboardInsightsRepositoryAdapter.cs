using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.AdapterRepositories
{
    public class DashboardInsightsRepositoryAdapter : IDashboardInsightsRepository
    {
        private readonly MicroSaaS.Application.Interfaces.Repositories.IDashboardInsightsRepository _applicationRepository;

        public DashboardInsightsRepositoryAdapter(MicroSaaS.Application.Interfaces.Repositories.IDashboardInsightsRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<DashboardInsights> AddAsync(DashboardInsights insights)
        {
            return await _applicationRepository.AddAsync(insights);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _applicationRepository.DeleteAsync(id);
        }

        public async Task<List<DashboardInsights>> GetByCreatorIdAsync(Guid creatorId)
        {
            var results = await _applicationRepository.GetByCreatorIdAsync(creatorId);
            return results.ToList();
        }

        public async Task<DashboardInsights> GetByIdAsync(Guid id)
        {
            return await _applicationRepository.GetByIdAsync(id);
        }

        public async Task<DashboardInsights> GetLatestByCreatorIdAsync(Guid creatorId)
        {
            return await _applicationRepository.GetLatestByCreatorIdAsync(creatorId);
        }

        public async Task<DashboardInsights> UpdateAsync(DashboardInsights insights)
        {
            return await _applicationRepository.UpdateAsync(insights);
        }
    }
} 