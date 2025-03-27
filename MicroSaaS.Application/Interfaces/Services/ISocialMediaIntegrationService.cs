using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;

namespace MicroSaaS.Application.Interfaces.Services;

public interface ISocialMediaIntegrationService
{
    // Autenticação
    Task<bool> AuthenticateAccountAsync(SocialMediaAccount account);
    string GetAuthorizationUrl(SocialMediaPlatform platform, string callbackUrl);
    Task<string> ExchangeCodeForToken(SocialMediaPlatform platform, string code);
    
    // Programação de Posts
    Task<bool> SchedulePostAsync(ContentPost post, string accessToken);
    
    // Análise de Desempenho
    Task<List<ContentPerformance>> GetPostPerformanceAsync(SocialMediaPlatform platform, string accessToken);
    Task<List<ContentPerformance>> GetContentInsightsAsync(Guid creatorId, SocialMediaPlatform platform, DateTime startDate, DateTime endDate);
    Task<List<PostTimeRecommendation>> GetBestPostTimesAsync(Guid creatorId, SocialMediaPlatform platform);
}
