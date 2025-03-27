using MicroSaaS.Domain.Entities;

namespace MicroSaaS.Domain.Interfaces.Services;

public interface ISocialMediaIntegrationService
{
    Task<bool> ConnectPlatformAsync(Guid creatorId, string platform, string accessToken);
    Task<bool> DisconnectPlatformAsync(Guid creatorId, string platform);
    Task<IEnumerable<SocialMediaAccount>> GetConnectedPlatformsAsync(Guid creatorId);
    Task<bool> IsPlatformConnectedAsync(Guid creatorId, string platform);
    Task<bool> RefreshTokenAsync(Guid creatorId, string platform);
} 