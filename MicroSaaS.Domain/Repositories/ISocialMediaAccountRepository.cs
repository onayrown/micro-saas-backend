using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Domain.Repositories
{
    public interface ISocialMediaAccountRepository
    {
        Task<SocialMediaAccount> GetByIdAsync(Guid id);
        Task<IEnumerable<SocialMediaAccount>> GetAllAsync();
        Task<IEnumerable<SocialMediaAccount>> GetByCreatorIdAsync(Guid creatorId);
        Task<SocialMediaAccount> GetByCreatorIdAndPlatformAsync(Guid creatorId, SocialMediaPlatform platform);
        Task<SocialMediaAccount> AddAsync(SocialMediaAccount account);
        Task<SocialMediaAccount> UpdateAsync(SocialMediaAccount account);
        Task<bool> DeleteAsync(Guid id);
        Task UpdateTokenAsync(Guid id, string accessToken, string refreshToken, DateTime expiresAt);
    }
} 