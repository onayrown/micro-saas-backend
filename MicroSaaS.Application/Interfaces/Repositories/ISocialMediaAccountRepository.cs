using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Interfaces.Repositories;

public interface ISocialMediaAccountRepository
{
    Task<SocialMediaAccount?> GetByIdAsync(Guid id);
    Task<IEnumerable<SocialMediaAccount>> GetByCreatorIdAsync(Guid creatorId);
    Task<IEnumerable<SocialMediaAccount>> GetByPlatformAsync(Guid creatorId, SocialMediaPlatform platform);
    Task<SocialMediaAccount> AddAsync(SocialMediaAccount account);
    Task<SocialMediaAccount> UpdateAsync(SocialMediaAccount account);
    Task<bool> DeleteAsync(Guid id);
    Task UpdateTokenAsync(Guid id, string accessToken, string refreshToken, DateTime expiresAt);
    Task<int> GetTotalFollowersAsync();
    Task<int> GetTotalFollowersByCreatorAsync(Guid creatorId);
    Task RefreshSocialMediaMetricsAsync();
    
    /// <summary>
    /// Verifica se um criador existe com base no ID fornecido
    /// </summary>
    /// <param name="creatorId">ID do criador a verificar</param>
    /// <returns>True se o criador existir, False caso contrário</returns>
    Task<bool> CreatorExistsAsync(Guid creatorId);
} 