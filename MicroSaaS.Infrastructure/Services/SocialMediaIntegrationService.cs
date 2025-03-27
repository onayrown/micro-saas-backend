using Microsoft.Extensions.Configuration;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using MicroSaaS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Linq;

namespace MicroSaaS.Infrastructure.Services;

public class SocialMediaIntegrationService : ISocialMediaIntegrationService
{
    private readonly IContentCreatorRepository _creatorRepository;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public SocialMediaIntegrationService(
        IContentCreatorRepository creatorRepository,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _creatorRepository = creatorRepository;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    // Autenticação
    public async Task<string> GetAuthUrlAsync(SocialMediaPlatform platform)
    {
        var clientId = GetClientId(platform);
        var scope = GetScope(platform);
        var baseUrl = GetAuthBaseUrl(platform);
        var callbackUrl = GetCallbackUrl(platform);

        return $"{baseUrl}?client_id={clientId}&redirect_uri={Uri.EscapeDataString(callbackUrl)}&scope={Uri.EscapeDataString(scope)}&response_type=code";
    }

    public async Task<SocialMediaAccount> HandleAuthCallbackAsync(SocialMediaPlatform platform, string code)
    {
        var clientId = GetClientId(platform);
        var clientSecret = GetClientSecret(platform);
        var callbackUrl = GetCallbackUrl(platform);
        var tokenUrl = GetTokenUrl(platform);

        var tokenResponse = await ExchangeAuthCodeForTokenAsync(tokenUrl, clientId, clientSecret, code, callbackUrl);
        
        // Em uma implementação real, você buscaria os detalhes da conta da API da plataforma
        var account = new SocialMediaAccount
        {
            Id = Guid.NewGuid(),
            Platform = platform,
            Username = $"user_{platform.ToString().ToLower()}",
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return account;
    }

    public async Task<bool> ValidateTokenAsync(SocialMediaAccount account)
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account));

        // Verificar se o token expirou
        if (account.TokenExpiresAt <= DateTime.UtcNow)
            return false;

        // Em uma implementação real, você faria uma chamada à API da plataforma para validar o token
        return true;
    }

    public async Task RefreshTokenAsync(SocialMediaAccount account)
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account));

        var platform = account.Platform;
        var clientId = GetClientId(platform);
        var clientSecret = GetClientSecret(platform);
        var refreshTokenUrl = GetRefreshTokenUrl(platform);

        // Em uma implementação real, você faria uma chamada à API da plataforma para atualizar o token
        var newTokenResponse = new TokenResponse
        {
            AccessToken = $"new_access_token_{Guid.NewGuid()}",
            RefreshToken = account.RefreshToken, // Algumas plataformas retornam um novo refresh_token
            ExpiresIn = 3600 // 1 hora
        };

        account.AccessToken = newTokenResponse.AccessToken;
        if (!string.IsNullOrEmpty(newTokenResponse.RefreshToken))
        {
            account.RefreshToken = newTokenResponse.RefreshToken;
        }
        account.TokenExpiresAt = DateTime.UtcNow.AddSeconds(newTokenResponse.ExpiresIn);
        account.UpdatedAt = DateTime.UtcNow;
    }

    // Gerenciamento de Contas
    public async Task ConnectAccountAsync(SocialMediaAccount account)
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account));

        var creator = await _creatorRepository.GetByIdAsync(account.CreatorId);
        if (creator == null)
            throw new Exception("Criador não encontrado");

        // Em uma implementação real, você salvaria a conta no repositório
        account.IsActive = true;
        account.UpdatedAt = DateTime.UtcNow;
    }

    public async Task DisconnectAccountAsync(SocialMediaAccount account)
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account));

        var creator = await _creatorRepository.GetByIdAsync(account.CreatorId);
        if (creator == null)
            throw new Exception("Criador não encontrado");

        // Em uma implementação real, você revogaria o token na API da plataforma
        account.IsActive = false;
        account.UpdatedAt = DateTime.UtcNow;
    }

    public async Task<Dictionary<string, int>> GetAccountStatsAsync(SocialMediaAccount account)
    {
        if (account == null)
            throw new ArgumentNullException(nameof(account));

        var creator = await _creatorRepository.GetByIdAsync(account.CreatorId);
        if (creator == null)
            throw new Exception("Criador não encontrado");

        // Em uma implementação real, você obteria estatísticas da API da plataforma
        return new Dictionary<string, int>
        {
            { "followers", Random.Shared.Next(1000, 10000) },
            { "following", Random.Shared.Next(100, 1000) },
            { "posts", Random.Shared.Next(50, 500) }
        };
    }

    // Gerenciamento de Posts
    public async Task PostContentAsync(ContentPost post)
    {
        if (post == null)
            throw new ArgumentNullException(nameof(post));

        var creator = await _creatorRepository.GetByIdAsync(post.CreatorId);
        if (creator == null)
            throw new Exception("Criador não encontrado");

        // Em uma implementação real, você postaria o conteúdo na API da plataforma
        post.Status = PostStatus.Published;
        post.PublishedAt = DateTime.UtcNow;
        post.UpdatedAt = DateTime.UtcNow;
    }

    public async Task SchedulePostAsync(ContentPost post, DateTime scheduledTime)
    {
        if (post == null)
            throw new ArgumentNullException(nameof(post));

        var creator = await _creatorRepository.GetByIdAsync(post.CreatorId);
        if (creator == null)
            throw new Exception("Criador não encontrado");

        // Em uma implementação real, você agendaria o post na API da plataforma
        post.Status = PostStatus.Scheduled;
        post.ScheduledFor = scheduledTime;
        post.UpdatedAt = DateTime.UtcNow;
    }

    public async Task CancelScheduledPostAsync(string postId)
    {
        if (string.IsNullOrEmpty(postId))
            throw new ArgumentException("ID do post é obrigatório", nameof(postId));

        // Em uma implementação real, você cancelaria o agendamento na API da plataforma
    }

    public async Task<IEnumerable<ContentPost>> GetScheduledPostsAsync(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new Exception("Criador não encontrado");

        // Em uma implementação real, você obteria os posts agendados da API da plataforma
        return new List<ContentPost>();
    }

    public async Task<IEnumerable<ContentPost>> GetPublishedPostsAsync(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new Exception("Criador não encontrado");

        // Em uma implementação real, você obteria os posts publicados da API da plataforma
        return new List<ContentPost>();
    }

    // Análise de Performance
    public async Task<IEnumerable<ContentPerformanceDto>> GetPostPerformanceAsync(string postId)
    {
        if (string.IsNullOrEmpty(postId))
            throw new ArgumentException("ID do post é obrigatório", nameof(postId));

        // Em uma implementação real, você obteria o desempenho do post da API da plataforma
        var performances = new List<ContentPerformanceDto>
        {
            new ContentPerformanceDto
            {
                Id = Guid.NewGuid(),
                PostId = postId,
                Platform = SocialMediaPlatform.Instagram,
                Views = Random.Shared.Next(100, 10000),
                Likes = Random.Shared.Next(10, 1000),
                Comments = Random.Shared.Next(5, 200),
                Shares = Random.Shared.Next(1, 100),
                Date = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 30)),
                EstimatedRevenue = Random.Shared.Next(100, 5000) / 100.0m,
                CollectedAt = DateTime.UtcNow
            }
        };

        return performances;
    }

    public async Task<IEnumerable<ContentPerformanceDto>> GetAccountPerformanceAsync(Guid accountId, DateTime startDate, DateTime endDate)
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException("ID da conta é obrigatório", nameof(accountId));

        // Em uma implementação real, você obteria o desempenho da conta da API da plataforma
        var performances = new List<ContentPerformanceDto>();
        
        // Simular dados de desempenho para o período especificado
        var currentDate = startDate;
        while (currentDate <= endDate)
        {
            performances.Add(new ContentPerformanceDto
            {
                Id = Guid.NewGuid(),
                PostId = $"post_{Guid.NewGuid()}",
                AccountId = accountId,
                Platform = SocialMediaPlatform.Instagram,
                Views = Random.Shared.Next(1000, 100000),
                Likes = Random.Shared.Next(100, 10000),
                Comments = Random.Shared.Next(10, 1000),
                Shares = Random.Shared.Next(5, 500),
                Date = currentDate,
                EstimatedRevenue = Random.Shared.Next(100, 5000) / 100.0m,
                CollectedAt = DateTime.UtcNow
            });
            
            currentDate = currentDate.AddDays(1);
        }

        return performances;
    }

    public async Task<Dictionary<string, decimal>> GetRevenueMetricsAsync(Guid accountId, DateTime startDate, DateTime endDate)
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException("ID da conta é obrigatório", nameof(accountId));

        // Em uma implementação real, você obteria métricas de receita da API da plataforma
        return new Dictionary<string, decimal>
        {
            { "estimated_revenue", Random.Shared.Next(100, 10000) / 100.0m },
            { "ad_impressions", Random.Shared.Next(10000, 1000000) },
            { "cpm", Random.Shared.Next(100, 1000) / 100.0m }
        };
    }

    public async Task<IEnumerable<PostTimeRecommendation>> GetBestPostingTimesAsync(Guid accountId)
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException("ID da conta é obrigatório", nameof(accountId));

        // Em uma implementação real, você analisaria dados históricos para determinar os melhores horários
        var recommendations = new List<PostTimeRecommendation>();
        
        // Gerar recomendações para diferentes dias da semana
        foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
        {
            // Adicionar algumas recomendações para manhã, tarde e noite
            recommendations.Add(new PostTimeRecommendation
            {
                DayOfWeek = day,
                TimeOfDay = new TimeSpan(9, 0, 0), // 9:00 AM
                EngagementScore = (decimal)(0.6 + Random.Shared.NextDouble() * 0.4)
            });
            
            recommendations.Add(new PostTimeRecommendation
            {
                DayOfWeek = day,
                TimeOfDay = new TimeSpan(12, 0, 0), // 12:00 PM
                EngagementScore = (decimal)(0.5 + Random.Shared.NextDouble() * 0.5)
            });
            
            recommendations.Add(new PostTimeRecommendation
            {
                DayOfWeek = day,
                TimeOfDay = new TimeSpan(18, 0, 0), // 6:00 PM
                EngagementScore = (decimal)(0.7 + Random.Shared.NextDouble() * 0.3)
            });
        }
        
        // Ordenar por pontuação de engajamento (descendente)
        return recommendations.OrderByDescending(r => r.EngagementScore);
    }

    // Implementações de métodos da interface Domain
    public async Task<bool> ConnectPlatformAsync(Guid creatorId, string platform, string accessToken)
    {
        if (creatorId == Guid.Empty)
            throw new ArgumentException("ID do criador é obrigatório", nameof(creatorId));
        
        if (string.IsNullOrEmpty(platform))
            throw new ArgumentException("Plataforma é obrigatória", nameof(platform));
        
        if (string.IsNullOrEmpty(accessToken))
            throw new ArgumentException("Token de acesso é obrigatório", nameof(accessToken));

        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new Exception("Criador não encontrado");

        // Em uma implementação real, você validaria o token e conectaria à plataforma
        return true;
    }

    public async Task<bool> DisconnectPlatformAsync(Guid creatorId, string platform)
    {
        if (creatorId == Guid.Empty)
            throw new ArgumentException("ID do criador é obrigatório", nameof(creatorId));
        
        if (string.IsNullOrEmpty(platform))
            throw new ArgumentException("Plataforma é obrigatória", nameof(platform));

        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new Exception("Criador não encontrado");

        // Em uma implementação real, você revogaria o token e desconectaria da plataforma
        return true;
    }

    public async Task<IEnumerable<SocialMediaAccount>> GetConnectedPlatformsAsync(Guid creatorId)
    {
        if (creatorId == Guid.Empty)
            throw new ArgumentException("ID do criador é obrigatório", nameof(creatorId));

        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new Exception("Criador não encontrado");

        // Em uma implementação real, você buscaria as plataformas conectadas do repositório
        return new List<SocialMediaAccount>();
    }

    public async Task<bool> IsPlatformConnectedAsync(Guid creatorId, string platform)
    {
        if (creatorId == Guid.Empty)
            throw new ArgumentException("ID do criador é obrigatório", nameof(creatorId));
        
        if (string.IsNullOrEmpty(platform))
            throw new ArgumentException("Plataforma é obrigatória", nameof(platform));

        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new Exception("Criador não encontrado");

        // Em uma implementação real, você verificaria se a plataforma está conectada
        return Random.Shared.Next(2) == 1; // 50% de chance
    }

    public async Task<bool> RefreshTokenAsync(Guid creatorId, string platform)
    {
        if (creatorId == Guid.Empty)
            throw new ArgumentException("ID do criador é obrigatório", nameof(creatorId));
        
        if (string.IsNullOrEmpty(platform))
            throw new ArgumentException("Plataforma é obrigatória", nameof(platform));

        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new Exception("Criador não encontrado");

        // Em uma implementação real, você atualizaria o token da plataforma
        return true;
    }

    // Métodos auxiliares
    private string GetClientId(SocialMediaPlatform platform)
    {
        return platform switch
        {
            SocialMediaPlatform.Instagram => _configuration["SocialMedia:Instagram:ClientId"] ?? "default_instagram_client_id",
            SocialMediaPlatform.YouTube => _configuration["SocialMedia:YouTube:ClientId"] ?? "default_youtube_client_id",
            SocialMediaPlatform.TikTok => _configuration["SocialMedia:TikTok:ClientId"] ?? "default_tiktok_client_id",
            _ => throw new ArgumentOutOfRangeException(nameof(platform))
        };
    }

    private string GetClientSecret(SocialMediaPlatform platform)
    {
        return platform switch
        {
            SocialMediaPlatform.Instagram => _configuration["SocialMedia:Instagram:ClientSecret"] ?? "default_instagram_client_secret",
            SocialMediaPlatform.YouTube => _configuration["SocialMedia:YouTube:ClientSecret"] ?? "default_youtube_client_secret",
            SocialMediaPlatform.TikTok => _configuration["SocialMedia:TikTok:ClientSecret"] ?? "default_tiktok_client_secret",
            _ => throw new ArgumentOutOfRangeException(nameof(platform))
        };
    }

    private string GetScope(SocialMediaPlatform platform)
    {
        return platform switch
        {
            SocialMediaPlatform.Instagram => "user_profile,user_media",
            SocialMediaPlatform.YouTube => "https://www.googleapis.com/auth/youtube",
            SocialMediaPlatform.TikTok => "user.info.basic,video.publish",
            _ => throw new ArgumentOutOfRangeException(nameof(platform))
        };
    }

    private string GetAuthBaseUrl(SocialMediaPlatform platform)
    {
        return platform switch
        {
            SocialMediaPlatform.Instagram => "https://api.instagram.com/oauth/authorize",
            SocialMediaPlatform.YouTube => "https://accounts.google.com/o/oauth2/auth",
            SocialMediaPlatform.TikTok => "https://open-api.tiktok.com/platform/oauth/authorize",
            _ => throw new ArgumentOutOfRangeException(nameof(platform))
        };
    }

    private string GetTokenUrl(SocialMediaPlatform platform)
    {
        return platform switch
        {
            SocialMediaPlatform.Instagram => "https://api.instagram.com/oauth/access_token",
            SocialMediaPlatform.YouTube => "https://oauth2.googleapis.com/token",
            SocialMediaPlatform.TikTok => "https://open-api.tiktok.com/oauth/access_token/",
            _ => throw new ArgumentOutOfRangeException(nameof(platform))
        };
    }

    private string GetRefreshTokenUrl(SocialMediaPlatform platform)
    {
        return platform switch
        {
            SocialMediaPlatform.Instagram => "https://api.instagram.com/oauth/refresh_access_token",
            SocialMediaPlatform.YouTube => "https://oauth2.googleapis.com/token",
            SocialMediaPlatform.TikTok => "https://open-api.tiktok.com/oauth/refresh_token/",
            _ => throw new ArgumentOutOfRangeException(nameof(platform))
        };
    }

    private string GetCallbackUrl(SocialMediaPlatform platform)
    {
        var baseUrl = _configuration["Application:BaseUrl"] ?? "https://localhost:5001";
        return $"{baseUrl}/api/auth/callback/{platform.ToString().ToLower()}";
    }

    private async Task<TokenResponse> ExchangeAuthCodeForTokenAsync(
        string tokenUrl, string clientId, string clientSecret, string code, string redirectUri)
    {
        // Em uma implementação real, você faria uma chamada HTTP para trocar o código pelo token
        // Por simplicidade, estamos retornando um token simulado
        return new TokenResponse
        {
            AccessToken = $"access_token_{Guid.NewGuid()}",
            RefreshToken = $"refresh_token_{Guid.NewGuid()}",
            ExpiresIn = 3600 // 1 hora
        };
    }

    private class TokenResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
    }
} 