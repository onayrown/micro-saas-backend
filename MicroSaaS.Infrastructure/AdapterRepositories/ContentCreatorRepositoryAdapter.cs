using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.AdapterRepositories
{
    public class ContentCreatorRepositoryAdapter : Application.Interfaces.Repositories.IContentCreatorRepository
    {
        private readonly Domain.Repositories.IContentCreatorRepository _domainRepository;

        public ContentCreatorRepositoryAdapter(Domain.Repositories.IContentCreatorRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public async Task<ContentCreator> GetByIdAsync(Guid id)
        {
            return await _domainRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<ContentCreator>> GetAllAsync()
        {
            return await _domainRepository.GetAllAsync();
        }

        public async Task<ContentCreator> AddAsync(ContentCreator creator)
        {
            return await _domainRepository.AddAsync(creator);
        }

        public async Task<ContentCreator> UpdateAsync(ContentCreator creator)
        {
            return await _domainRepository.UpdateAsync(creator);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _domainRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<ContentCreator>> GetByUserIdAsync(Guid userId)
        {
            var creator = await _domainRepository.GetByUserIdAsync(userId);
            return creator != null ? new List<ContentCreator> { creator } : new List<ContentCreator>();
        }

        public async Task<int> CountAsync()
        {
            var creators = await _domainRepository.GetAllAsync();
            return creators.Count();
        }
    }
} 