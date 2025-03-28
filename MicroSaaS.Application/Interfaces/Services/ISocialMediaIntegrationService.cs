using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using MicroSaaS.Application.DTOs;

namespace MicroSaaS.Application.Interfaces.Services;

public interface ISocialMediaIntegrationService
{
    // Autenticação
    Task<string> GetAuthUrlAsync(SocialMediaPlatform platform);
    Task<SocialMediaAccount> HandleAuthCallbackAsync(SocialMediaPlatform platform, string code);
    Task<bool> ValidateTokenAsync(SocialMediaAccount account);
    Task RefreshTokenAsync(SocialMediaAccount account);

    // Gerenciamento de Contas
    Task ConnectAccountAsync(SocialMediaAccount account);
    Task DisconnectAccountAsync(SocialMediaAccount account);
    Task<Dictionary<string, int>> GetAccountStatsAsync(SocialMediaAccount account);

    // Gerenciamento de Posts
    Task PostContentAsync(ContentPost post);
    Task SchedulePostAsync(ContentPost post, DateTime scheduledTime);
    Task CancelScheduledPostAsync(string postId);
    Task<IEnumerable<ContentPost>> GetScheduledPostsAsync(Guid creatorId);
    Task<IEnumerable<ContentPost>> GetPublishedPostsAsync(Guid creatorId);

    // Análise de Performance
    Task<IEnumerable<ContentPerformanceDto>> GetPostPerformanceAsync(string postId);
    Task<IEnumerable<ContentPerformanceDto>> GetAccountPerformanceAsync(Guid accountId, DateTime startDate, DateTime endDate);
    Task<Dictionary<string, decimal>> GetRevenueMetricsAsync(Guid accountId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<MicroSaaS.Shared.Models.PostTimeRecommendation>> GetBestPostingTimesAsync(Guid accountId);
}
