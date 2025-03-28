using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Interfaces;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.AdapterRepositories
{
    public class DomainContentPostRepositoryAdapter : IContentPostRepository
    {
        private readonly MicroSaaS.Domain.Repositories.IContentPostRepository _domainRepository;

        public DomainContentPostRepositoryAdapter(MicroSaaS.Domain.Repositories.IContentPostRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public async Task<ContentPost> AddAsync(ContentPost post)
        {
            return await _domainRepository.AddAsync(post);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _domainRepository.DeleteAsync(id);
        }

        public async Task<List<ContentPost>> GetByCreatorIdAsync(Guid creatorId)
        {
            var result = await _domainRepository.GetByCreatorIdAsync(creatorId);
            return result.ToList();
        }

        public async Task<ContentPost> GetByIdAsync(Guid id)
        {
            return await _domainRepository.GetByIdAsync(id);
        }

        public async Task<List<ContentPost>> GetByPlatformAsync(SocialMediaPlatform platform)
        {
            var result = await _domainRepository.GetByPlatformAsync(platform);
            return result.ToList();
        }

        public async Task<ContentPost> UpdateAsync(ContentPost post)
        {
            return await _domainRepository.UpdateAsync(post);
        }
    }
} 