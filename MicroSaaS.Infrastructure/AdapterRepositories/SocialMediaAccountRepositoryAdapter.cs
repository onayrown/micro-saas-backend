using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Repositories;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MicroSaaS.Infrastructure.AdapterRepositories
{
    public class SocialMediaAccountRepositoryAdapter : Application.Interfaces.Repositories.ISocialMediaAccountRepository
    {
        private readonly Domain.Repositories.ISocialMediaAccountRepository _domainRepository;

        public SocialMediaAccountRepositoryAdapter(Domain.Repositories.ISocialMediaAccountRepository domainRepository)
        {
            _domainRepository = domainRepository;
        }

        public async Task<SocialMediaAccount?> GetByIdAsync(Guid id)
        {
            return await _domainRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<SocialMediaAccount>> GetByCreatorIdAsync(Guid creatorId)
        {
            return await _domainRepository.GetByCreatorIdAsync(creatorId);
        }

        public async Task<IEnumerable<SocialMediaAccount>> GetByPlatformAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            var account = await _domainRepository.GetByCreatorIdAndPlatformAsync(creatorId, platform);
            return account != null ? new List<SocialMediaAccount> { account } : new List<SocialMediaAccount>();
        }

        public async Task<SocialMediaAccount> AddAsync(SocialMediaAccount account)
        {
            return await _domainRepository.AddAsync(account);
        }

        public async Task UpdateAsync(SocialMediaAccount account)
        {
            await _domainRepository.UpdateAsync(account);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _domainRepository.DeleteAsync(id);
        }

        public async Task UpdateTokenAsync(Guid id, string accessToken, string refreshToken, DateTime expiresAt)
        {
            var account = await _domainRepository.GetByIdAsync(id);
            if (account != null)
            {
                account.AccessToken = accessToken;
                account.RefreshToken = refreshToken;
                account.TokenExpiresAt = expiresAt;
                await _domainRepository.UpdateAsync(account);
            }
        }

        public async Task<int> GetTotalFollowersAsync()
        {
            var accounts = await _domainRepository.GetAllAsync();
            return accounts.Sum(a => a.FollowersCount);
        }

        public async Task<int> GetTotalFollowersByCreatorAsync(Guid creatorId)
        {
            var accounts = await _domainRepository.GetByCreatorIdAsync(creatorId);
            return accounts.Sum(a => a.FollowersCount);
        }

        public async Task RefreshSocialMediaMetricsAsync()
        {
            var accounts = await _domainRepository.GetAllAsync();
            foreach (var account in accounts)
            {
                // Em uma implementação real, você atualizaria as métricas da API da plataforma
                // Por enquanto, apenas atualizamos o timestamp
                account.UpdatedAt = DateTime.UtcNow;
                await _domainRepository.UpdateAsync(account);
            }
        }
    }
} 