using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Application.Interfaces.Repositories;

public interface ISocialMediaAccountRepository
{
    Task<SocialMediaAccount?> GetByIdAsync(Guid id);
    Task<IEnumerable<SocialMediaAccount>> GetByCreatorIdAsync(Guid creatorId);
    Task<IEnumerable<SocialMediaAccount>> GetByPlatformAsync(Guid creatorId, SocialMediaPlatform platform);
    Task<SocialMediaAccount> AddAsync(SocialMediaAccount account);
    Task UpdateAsync(SocialMediaAccount account);
    Task DeleteAsync(Guid id);
    Task UpdateTokenAsync(Guid id, string accessToken, string refreshToken, DateTime expiresAt);
    Task<int> GetTotalFollowersAsync();
    Task<int> GetTotalFollowersByCreatorAsync(Guid creatorId);
    Task RefreshSocialMediaMetricsAsync();
} 