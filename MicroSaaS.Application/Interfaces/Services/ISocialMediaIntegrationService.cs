using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Application.Interfaces.Services;

public interface ISocialMediaIntegrationService
{
    Task<bool> AuthenticateAccountAsync(SocialMediaAccount account);
    Task<List<ContentPerformance>> GetPostPerformanceAsync(SocialMediaPlatform platform, string accessToken);
    Task<bool> SchedulePostAsync(ContentPost post, string accessToken);
}
