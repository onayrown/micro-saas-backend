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
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

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

        // URL específica para Instagram
        if (platform == SocialMediaPlatform.Instagram)
        {
            return $"{baseUrl}?client_id={clientId}&redirect_uri={Uri.EscapeDataString(callbackUrl)}&scope={Uri.EscapeDataString(scope)}&response_type=code";
        }
        
        // URL específica para YouTube
        if (platform == SocialMediaPlatform.YouTube)
        {
            return $"{baseUrl}?client_id={clientId}&redirect_uri={Uri.EscapeDataString(callbackUrl)}&scope={Uri.EscapeDataString(scope)}&response_type=code&access_type=offline&prompt=consent";
        }
        
        // URL específica para TikTok
        if (platform == SocialMediaPlatform.TikTok)
        {
            return $"{baseUrl}?client_key={clientId}&redirect_uri={Uri.EscapeDataString(callbackUrl)}&scope={Uri.EscapeDataString(scope)}&response_type=code&state={Guid.NewGuid()}";
        }
        
        // URL genérica para outras plataformas
        return $"{baseUrl}?client_id={clientId}&redirect_uri={Uri.EscapeDataString(callbackUrl)}&scope={Uri.EscapeDataString(scope)}&response_type=code";
    }

    public async Task<SocialMediaAccount> HandleAuthCallbackAsync(SocialMediaPlatform platform, string code)
    {
        var clientId = GetClientId(platform);
        var clientSecret = GetClientSecret(platform);
        var callbackUrl = GetCallbackUrl(platform);
        var tokenUrl = GetTokenUrl(platform);

        var tokenResponse = await ExchangeAuthCodeForTokenAsync(tokenUrl, clientId, clientSecret, code, callbackUrl, platform);
        
        // Criar conta com dados básicos
        var account = new SocialMediaAccount
        {
            Id = Guid.NewGuid(),
            Platform = platform,
            AccessToken = tokenResponse.AccessToken,
            RefreshToken = tokenResponse.RefreshToken,
            TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Obter informações de perfil específicas para Instagram
        if (platform == SocialMediaPlatform.Instagram)
        {
            try
            {
                var profileData = await GetInstagramProfileAsync(tokenResponse.AccessToken);
                account.Username = profileData.Username;
                account.ProfileUrl = profileData.ProfileUrl;
                account.ProfileImageUrl = profileData.ProfileImageUrl;
                account.FollowersCount = profileData.FollowersCount;
            }
            catch (Exception ex)
            {
                // Logar o erro, mas continuar com informações básicas
                Console.WriteLine($"Erro ao obter perfil do Instagram: {ex.Message}");
                account.Username = $"instagram_user_{DateTime.UtcNow.Ticks}";
            }
        }
        // Obter informações de perfil específicas para YouTube
        else if (platform == SocialMediaPlatform.YouTube)
        {
            try
            {
                var profileData = await GetYouTubeProfileAsync(tokenResponse.AccessToken);
                account.Username = profileData.Username;
                account.ProfileUrl = profileData.ProfileUrl;
                account.ProfileImageUrl = profileData.ProfileImageUrl;
                account.FollowersCount = profileData.FollowersCount;
            }
            catch (Exception ex)
            {
                // Logar o erro, mas continuar com informações básicas
                Console.WriteLine($"Erro ao obter perfil do YouTube: {ex.Message}");
                account.Username = $"youtube_user_{DateTime.UtcNow.Ticks}";
            }
        }
        // Obter informações de perfil específicas para TikTok
        else if (platform == SocialMediaPlatform.TikTok)
        {
            try
            {
                var profileData = await GetTikTokProfileAsync(tokenResponse.AccessToken, tokenResponse.OpenId);
                account.Username = profileData.Username;
                account.ProfileUrl = profileData.ProfileUrl;
                account.ProfileImageUrl = profileData.ProfileImageUrl;
                account.FollowersCount = profileData.FollowersCount;
            }
            catch (Exception ex)
            {
                // Logar o erro, mas continuar com informações básicas
                Console.WriteLine($"Erro ao obter perfil do TikTok: {ex.Message}");
                account.Username = $"tiktok_user_{DateTime.UtcNow.Ticks}";
            }
        }
        else
        {
            // Dados fictícios para outras plataformas até implementação completa
            account.Username = $"user_{platform.ToString().ToLower()}";
        }

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

        // Lógica específica para Instagram
        if (platform == SocialMediaPlatform.Instagram)
        {
            try
            {
                // O Instagram usa o endpoint de token com grant_type=refresh_token
                var requestData = new Dictionary<string, string>
                {
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "grant_type", "refresh_token" },
                    { "refresh_token", account.RefreshToken }
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, refreshTokenUrl)
                {
                    Content = new FormUrlEncodedContent(requestData)
                };

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var tokenData = await response.Content.ReadFromJsonAsync<TokenResponse>();
                    if (tokenData != null)
                    {
                        account.AccessToken = tokenData.AccessToken;
                        if (!string.IsNullOrEmpty(tokenData.RefreshToken))
                        {
                            account.RefreshToken = tokenData.RefreshToken;
                        }
                        account.TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenData.ExpiresIn);
                        account.UpdatedAt = DateTime.UtcNow;
                        return;
                    }
                }
                
                throw new Exception($"Falha ao atualizar token do Instagram: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar token do Instagram: {ex.Message}", ex);
            }
        }
        // Lógica específica para YouTube
        else if (platform == SocialMediaPlatform.YouTube)
        {
            try
            {
                // O YouTube usa o endpoint de token com grant_type=refresh_token
                var requestData = new Dictionary<string, string>
                {
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "grant_type", "refresh_token" },
                    { "refresh_token", account.RefreshToken }
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, refreshTokenUrl)
                {
                    Content = new FormUrlEncodedContent(requestData)
                };

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var tokenData = await response.Content.ReadFromJsonAsync<TokenResponse>();
                    if (tokenData != null)
                    {
                        account.AccessToken = tokenData.AccessToken;
                        // YouTube geralmente não retorna um novo refresh_token
                        account.TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenData.ExpiresIn);
                        account.UpdatedAt = DateTime.UtcNow;
                        return;
                    }
                }
                
                throw new Exception($"Falha ao atualizar token do YouTube: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar token do YouTube: {ex.Message}", ex);
            }
        }
        // Lógica específica para TikTok
        else if (platform == SocialMediaPlatform.TikTok)
        {
            try
            {
                // O TikTok usa o endpoint de token com grant_type=refresh_token
                var requestData = new Dictionary<string, string>
                {
                    { "client_key", clientId },
                    { "client_secret", clientSecret },
                    { "grant_type", "refresh_token" },
                    { "refresh_token", account.RefreshToken }
                };

                using var request = new HttpRequestMessage(HttpMethod.Post, refreshTokenUrl)
                {
                    Content = new FormUrlEncodedContent(requestData)
                };

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var tokenData = await response.Content.ReadFromJsonAsync<TikTokTokenResponse>();
                    if (tokenData != null && tokenData.Data != null)
                    {
                        account.AccessToken = tokenData.Data.AccessToken;
                        if (!string.IsNullOrEmpty(tokenData.Data.RefreshToken))
                        {
                            account.RefreshToken = tokenData.Data.RefreshToken;
                        }
                        account.TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenData.Data.ExpiresIn);
                        account.UpdatedAt = DateTime.UtcNow;
                        return;
                    }
                }
                
                throw new Exception($"Falha ao atualizar token do TikTok: {response.StatusCode}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao atualizar token do TikTok: {ex.Message}", ex);
            }
        }
        
        // Implementação genérica para outras plataformas
        var newTokenResponse = new TokenResponse
        {
            AccessToken = $"new_access_token_{Guid.NewGuid()}",
            RefreshToken = account.RefreshToken,
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

    public async Task<IEnumerable<MicroSaaS.Shared.Models.PostTimeRecommendation>> GetBestPostingTimesAsync(Guid accountId)
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException("ID da conta é obrigatório", nameof(accountId));

        // Em uma implementação real, você analisaria dados históricos para determinar os melhores horários
        var recommendations = new List<MicroSaaS.Shared.Models.PostTimeRecommendation>();
        
        // Gerar recomendações para diferentes dias da semana
        foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
        {
            // Adicionar algumas recomendações para manhã, tarde e noite
            recommendations.Add(new MicroSaaS.Shared.Models.PostTimeRecommendation
            {
                Id = Guid.NewGuid(),
                DayOfWeek = day,
                TimeOfDay = new TimeSpan(9, 0, 0), // 9:00 AM
                EngagementScore = (decimal)(0.6 + Random.Shared.NextDouble() * 0.4)
            });
            
            recommendations.Add(new MicroSaaS.Shared.Models.PostTimeRecommendation
            {
                Id = Guid.NewGuid(),
                DayOfWeek = day,
                TimeOfDay = new TimeSpan(12, 0, 0), // 12:00 PM
                EngagementScore = (decimal)(0.5 + Random.Shared.NextDouble() * 0.5)
            });
            
            recommendations.Add(new MicroSaaS.Shared.Models.PostTimeRecommendation
            {
                Id = Guid.NewGuid(),
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

    private async Task<TokenResponse> ExchangeAuthCodeForTokenAsync(string tokenUrl, string clientId, string clientSecret, string code, string redirectUri, SocialMediaPlatform platform)
    {
        FormUrlEncodedContent formContent;
        
        if (platform == SocialMediaPlatform.TikTok)
        {
            formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_key", clientId },
                { "client_secret", clientSecret },
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUri }
            });
        }
        else
        {
            formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "code", code },
                { "grant_type", "authorization_code" },
                { "redirect_uri", redirectUri }
            });
        }

        var response = await _httpClient.PostAsync(tokenUrl, formContent);
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            
            // TikTok usa um formato diferente para a resposta
            if (platform == SocialMediaPlatform.TikTok)
            {
                var tikTokResponse = JsonSerializer.Deserialize<TikTokTokenResponse>(content);
                if (tikTokResponse?.Data != null)
                {
                    return new TokenResponse
                    {
                        AccessToken = tikTokResponse.Data.AccessToken,
                        RefreshToken = tikTokResponse.Data.RefreshToken,
                        ExpiresIn = tikTokResponse.Data.ExpiresIn,
                        OpenId = tikTokResponse.Data.OpenId
                    };
                }
            }
            else
            {
                return JsonSerializer.Deserialize<TokenResponse>(content) ?? new TokenResponse();
            }
        }
        
        throw new Exception($"Falha ao trocar código por token. Status: {response.StatusCode}, Conteúdo: {await response.Content.ReadAsStringAsync()}");
    }

    // Método auxiliar para obter informações do perfil do Instagram
    private async Task<InstagramProfile> GetInstagramProfileAsync(string accessToken)
    {
        // Endpoint para obter informações do usuário do Instagram
        string userEndpoint = "https://graph.instagram.com/me?fields=id,username,account_type,media_count&access_token=" + accessToken;
        
        var response = await _httpClient.GetAsync(userEndpoint);
        if (response.IsSuccessStatusCode)
        {
            var userInfo = await response.Content.ReadFromJsonAsync<InstagramUserInfo>();
            
            // Criar um perfil com as informações disponíveis
            return new InstagramProfile
            {
                Username = userInfo?.Username ?? "instagram_user",
                ProfileUrl = $"https://www.instagram.com/{userInfo?.Username ?? "unknown"}/",
                ProfileImageUrl = "https://placekitten.com/200/200", // Placeholder até implementarmos a obtenção da imagem real
                FollowersCount = new Random().Next(100, 10000) // Placeholder até implementarmos a obtenção real
            };
        }
        
        throw new Exception($"Falha ao obter perfil do Instagram: {response.StatusCode}");
    }
    
    // Método auxiliar para obter informações do perfil do YouTube
    private async Task<YouTubeProfile> GetYouTubeProfileAsync(string accessToken)
    {
        // Endpoint para obter informações do canal do YouTube
        string channelEndpoint = "https://www.googleapis.com/youtube/v3/channels?part=snippet,statistics&mine=true";
        
        using var request = new HttpRequestMessage(HttpMethod.Get, channelEndpoint);
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var channelData = await response.Content.ReadFromJsonAsync<YouTubeChannelResponse>();
            var channel = channelData?.Items?.FirstOrDefault();
            
            if (channel != null)
            {
                return new YouTubeProfile
                {
                    Username = channel.Snippet?.Title ?? "youtube_user",
                    ProfileUrl = $"https://www.youtube.com/channel/{channel.Id}",
                    ProfileImageUrl = channel.Snippet?.Thumbnails?.Default?.Url ?? "https://placekitten.com/200/200",
                    FollowersCount = int.Parse(channel.Statistics?.SubscriberCount ?? "0")
                };
            }
        }
        
        throw new Exception($"Falha ao obter perfil do YouTube: {response.StatusCode}");
    }

    // Método auxiliar para obter informações do perfil do TikTok
    private async Task<TikTokProfile> GetTikTokProfileAsync(string accessToken, string openId)
    {
        // Endpoint para obter informações do usuário do TikTok
        string userInfoEndpoint = "https://open.tiktokapis.com/v2/user/info/";
        
        var fields = new List<string> { "open_id", "union_id", "avatar_url", "display_name", "follower_count" };
        
        using var request = new HttpRequestMessage(HttpMethod.Post, userInfoEndpoint);
        request.Headers.Add("Authorization", $"Bearer {accessToken}");
        
        var requestContent = new
        {
            fields = fields
        };
        
        // Corrigindo a criação do StringContent para usar apenas dois parâmetros
        var jsonContent = JsonSerializer.Serialize(requestContent);
        request.Content = new StringContent(jsonContent, Encoding.UTF8);
        request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        
        var response = await _httpClient.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var userInfo = await response.Content.ReadFromJsonAsync<TikTokUserInfoResponse>();
            
            if (userInfo?.Data != null)
            {
                return new TikTokProfile
                {
                    Username = userInfo.Data.DisplayName ?? "tiktok_user",
                    ProfileUrl = $"https://www.tiktok.com/@{userInfo.Data.DisplayName ?? "unknown"}",
                    ProfileImageUrl = userInfo.Data.AvatarUrl ?? "https://placekitten.com/200/200",
                    FollowersCount = userInfo.Data.FollowerCount
                };
            }
        }
        
        throw new Exception($"Falha ao obter perfil do TikTok: {response.StatusCode}");
    }

    // Classes auxiliares para Instagram
    private class InstagramUserInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string AccountType { get; set; } = string.Empty;
        public int MediaCount { get; set; }
    }

    private class InstagramProfile
    {
        public string Username { get; set; } = string.Empty;
        public string ProfileUrl { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public int FollowersCount { get; set; }
    }
    
    // Classes auxiliares para YouTube
    private class YouTubeProfile
    {
        public string Username { get; set; } = string.Empty;
        public string ProfileUrl { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public int FollowersCount { get; set; }
    }
    
    private class YouTubeChannelResponse
    {
        public List<YouTubeChannel> Items { get; set; } = new List<YouTubeChannel>();
    }
    
    private class YouTubeChannel
    {
        public string Id { get; set; } = string.Empty;
        public YouTubeSnippet? Snippet { get; set; }
        public YouTubeStatistics? Statistics { get; set; }
    }
    
    private class YouTubeSnippet
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public YouTubeThumbnails? Thumbnails { get; set; }
    }
    
    private class YouTubeThumbnails
    {
        public YouTubeThumbnail? Default { get; set; }
        public YouTubeThumbnail? Medium { get; set; }
        public YouTubeThumbnail? High { get; set; }
    }
    
    private class YouTubeThumbnail
    {
        public string Url { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
    }
    
    private class YouTubeStatistics
    {
        public string ViewCount { get; set; } = string.Empty;
        public string SubscriberCount { get; set; } = string.Empty;
        public string VideoCount { get; set; } = string.Empty;
    }

    // Classes auxiliares para TikTok
    private class TikTokProfile
    {
        public string Username { get; set; } = string.Empty;
        public string ProfileUrl { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public int FollowersCount { get; set; }
    }
    
    private class TikTokTokenResponse
    {
        public TikTokTokenData? Data { get; set; }
    }
    
    private class TikTokTokenData
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
        
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
        
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        
        [JsonPropertyName("open_id")]
        public string OpenId { get; set; } = string.Empty;
        
        [JsonPropertyName("scope")]
        public string Scope { get; set; } = string.Empty;
    }
    
    private class TikTokUserInfoResponse
    {
        public TikTokUserData? Data { get; set; }
    }
    
    private class TikTokUserData
    {
        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }
        
        [JsonPropertyName("avatar_url")]
        public string? AvatarUrl { get; set; }
        
        [JsonPropertyName("follower_count")]
        public int FollowerCount { get; set; }
        
        [JsonPropertyName("open_id")]
        public string? OpenId { get; set; }
    }

    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        
        [JsonPropertyName("open_id")]
        public string OpenId { get; set; } = string.Empty;
    }
} 