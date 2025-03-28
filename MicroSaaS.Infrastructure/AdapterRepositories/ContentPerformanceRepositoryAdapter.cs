using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.AdapterRepositories
{
    public class ContentPerformanceRepositoryAdapter : IContentPerformanceRepository
    {
        private readonly MicroSaaS.Application.Interfaces.Repositories.IContentPerformanceRepository _applicationRepository;

        public ContentPerformanceRepositoryAdapter(MicroSaaS.Application.Interfaces.Repositories.IContentPerformanceRepository applicationRepository)
        {
            _applicationRepository = applicationRepository;
        }

        public async Task<ContentPerformance> AddAsync(ContentPerformance performance)
        {
            return await _applicationRepository.AddAsync(performance);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _applicationRepository.DeleteAsync(id);
        }

        public async Task<List<ContentPerformance>> GetByCreatorIdAsync(Guid creatorId)
        {
            var results = await _applicationRepository.GetByCreatorIdAsync(creatorId);
            return results.ToList();
        }

        public async Task<ContentPerformance> GetByIdAsync(Guid id)
        {
            return await _applicationRepository.GetByIdAsync(id);
        }

        public async Task<List<ContentPerformance>> GetByPostIdAsync(Guid postId)
        {
            var results = await _applicationRepository.GetByPostIdAsync(postId.ToString());
            return results.ToList();
        }

        public async Task<ContentPerformance> UpdateAsync(ContentPerformance performance)
        {
            return await _applicationRepository.UpdateAsync(performance);
        }
    }
} 