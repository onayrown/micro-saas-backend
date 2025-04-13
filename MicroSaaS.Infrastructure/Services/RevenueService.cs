using Microsoft.Extensions.Configuration;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.MongoDB;
using MicroSaaS.Infrastructure.Settings;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MicroSaaS.Infrastructure.Services;

public class RevenueService : IRevenueService
{
    private readonly IContentCreatorRepository _creatorRepository;
    private readonly IConfiguration _configuration;
    private readonly IMongoDbContext _context;
    private readonly HttpClient _httpClient;

    public RevenueService(
        IContentCreatorRepository creatorRepository,
        IConfiguration configuration,
        IMongoDbContext context,
        HttpClient httpClient)
    {
        _creatorRepository = creatorRepository;
        _configuration = configuration;
        _context = context;
        _httpClient = httpClient;
    }

    public async Task<Application.Interfaces.Services.RevenueSummary> GetRevenueSummaryAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado");

        var dailyRevenues = await GetDailyRevenueAsync(creatorId, startDate, endDate);
        var platformRevenues = await GetPlatformRevenueAsync(creatorId, startDate, endDate);
        var totalRevenue = await GetTotalRevenueAsync(creatorId, startDate, endDate);

        return new Application.Interfaces.Services.RevenueSummary
        {
            TotalRevenue = totalRevenue,
            EstimatedMonthlyRevenue = CalculateMRR(platformRevenues.ToList()),
            AverageRevenuePerView = totalRevenue / platformRevenues.Sum(p => p.Views)
        };
    }

    public async Task<IEnumerable<Application.Interfaces.Services.DailyRevenue>> GetDailyRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        return await GetRevenueByDayAsync(creatorId, startDate, endDate);
    }

    public async Task<IEnumerable<Application.Interfaces.Services.PlatformRevenue>> GetPlatformRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        return await GetRevenueByPlatformAsync(creatorId, startDate, endDate);
    }

    public async Task<decimal> GetTotalRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var filter = Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.And(
            Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Gte(p => p.Date, startDate),
            Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Lte(p => p.Date, endDate),
            Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Eq(p => p.CreatorId, creatorId)
        );

        var performances = await _context.PerformanceMetrics
            .Find(filter)
            .ToListAsync();
            
        return performances.Sum(p => p.EstimatedRevenue);
    }

    public async Task<string> GetAdSenseAuthUrlAsync(Guid creatorId, string callbackUrl)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado");

        // Obter as configurações do AdSense do appsettings.json
        var clientId = _configuration["Revenue:AdSense:ClientId"];
        if (string.IsNullOrEmpty(clientId))
            throw new InvalidOperationException("Client ID do Google AdSense não configurado");
        
        // Construir a URL de autenticação do OAuth2 para o Google AdSense
        var scopes = Uri.EscapeDataString("https://www.googleapis.com/auth/adsense.readonly");
        
        return $"https://accounts.google.com/o/oauth2/auth" +
               $"?client_id={clientId}" +
               $"&redirect_uri={Uri.EscapeDataString(callbackUrl)}" +
               $"&scope={scopes}" +
               $"&response_type=code" +
               $"&access_type=offline" +
               $"&prompt=consent" +
               $"&state={creatorId}";
    }

    public async Task<bool> ConnectAdSenseAsync(Guid creatorId, string authorizationCode)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado");

        // Obter as configurações do AdSense do appsettings.json
        var clientId = _configuration["Revenue:AdSense:ClientId"];
        var clientSecret = _configuration["Revenue:AdSense:ClientSecret"];
        
        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            throw new InvalidOperationException("Configurações do Google AdSense incompletas");
        
        try
        {
            // Trocar o código de autorização por um token de acesso
            var tokenEndpoint = "https://oauth2.googleapis.com/token";
            var callbackUrl = _configuration["Application:BaseUrl"] + "/api/revenue/adsense-callback";
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "code", authorizationCode },
                { "grant_type", "authorization_code" },
                { "redirect_uri", callbackUrl }
            });
            
            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();
                if (tokenResponse != null)
                {
                    // Atualizar o perfil do criador com as informações do AdSense
                    creator.AdSenseSettings = new MicroSaaS.Domain.Entities.AdSenseSettings
                    {
                        IsConnected = true,
                        AccessToken = tokenResponse.AccessToken,
                        RefreshToken = tokenResponse.RefreshToken,
                        TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                        ConnectedAt = DateTime.UtcNow
                    };
                    
                    await _creatorRepository.UpdateAsync(creator);
                    
                    // Buscar informações da conta do AdSense para validar
                    return await IntegrateGoogleAdSenseAsync(creator, tokenResponse.AccessToken);
                }
            }
            
            throw new Exception($"Falha ao trocar código por token. Status: {response.StatusCode}, Conteúdo: {await response.Content.ReadAsStringAsync()}");
        }
        catch (Exception ex)
        {
            // Logar o erro
            Console.WriteLine($"Erro ao conectar com Google AdSense: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> IntegrateGoogleAdSenseAsync(ContentCreator creator, string accessToken)
    {
        if (creator == null)
            throw new ArgumentNullException(nameof(creator));
        
        if (string.IsNullOrEmpty(accessToken))
            throw new ArgumentException("Token de acesso inválido", nameof(accessToken));
        
        try
        {
            // Endpoint para obter informações da conta do Google AdSense
            var accountInfoEndpoint = "https://www.googleapis.com/adsense/v2/accounts";
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            
            var response = await _httpClient.GetAsync(accountInfoEndpoint);
            if (response.IsSuccessStatusCode)
            {
                var accountInfo = await response.Content.ReadFromJsonAsync<AdSenseAccountsResponse>();
                
                if (accountInfo != null && accountInfo.Accounts.Count > 0)
                {
                    // Atualizar as informações da conta do AdSense no perfil do criador
                    creator.AdSenseSettings.AccountId = accountInfo.Accounts[0].Name;
                    creator.AdSenseSettings.DisplayName = accountInfo.Accounts[0].DisplayName;
                    creator.AdSenseSettings.LastUpdated = DateTime.UtcNow;
                    
                    await _creatorRepository.UpdateAsync(creator);
                    
                    // Iniciar a busca de dados de receita em background (em uma implementação real)
                    // Task.Run(() => RefreshAdSenseRevenueDataAsync(creator.Id));
                    
                    return true;
                }
            }
            
            throw new Exception($"Falha ao obter informações da conta do AdSense. Status: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            // Logar o erro
            Console.WriteLine($"Erro ao integrar com Google AdSense: {ex.Message}");
            return false;
        }
    }

    public async Task<decimal> GetEstimatedRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var filter = Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.And(
            Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Gte(p => p.Date, startDate),
            Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Lte(p => p.Date, endDate),
            Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Eq(p => p.CreatorId, creatorId)
        );

        var performances = await _context.PerformanceMetrics
            .Find(filter)
            .ToListAsync();
            
        return performances.Sum(p => p.EstimatedRevenue);
    }

    public async Task<Application.Interfaces.Services.RevenueSummary> GetRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var filter = Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.And(
            Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Gte(p => p.Date, startDate),
            Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Lte(p => p.Date, endDate),
            Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Eq(p => p.CreatorId, creatorId)
        );

        var performances = await _context.PerformanceMetrics
            .Find(filter)
            .ToListAsync();
            
        var totalRevenue = performances.Sum(p => p.EstimatedRevenue);
        var totalViews = performances.Sum(p => p.Views);
        
        var days = (endDate - startDate).TotalDays;
        var estimatedMonthly = (days > 0) ? (totalRevenue / (decimal)days * 30m) : 0m;
        
        return new Application.Interfaces.Services.RevenueSummary
        {
            TotalRevenue = totalRevenue,
            EstimatedMonthlyRevenue = estimatedMonthly,
            AverageRevenuePerView = totalViews > 0 ? totalRevenue / (decimal)totalViews : 0
        };
    }

    public async Task<List<Application.Interfaces.Services.PlatformRevenue>> GetRevenueByPlatformAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var filter = Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.And(
            Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Gte(p => p.Date, startDate),
            Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Lte(p => p.Date, endDate),
            Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Eq(p => p.CreatorId, creatorId)
        );
        
        var performances = await _context.PerformanceMetrics
            .Find(filter)
            .ToListAsync();

        return performances
            .GroupBy(p => p.Platform)
            .Select(g => new Application.Interfaces.Services.PlatformRevenue
            {
                Platform = g.Key.ToString(),
                Revenue = g.Sum(p => p.EstimatedRevenue),
                Views = (int)g.Sum(p => p.Views)
            }).ToList();
    }

    public async Task<List<Application.Interfaces.Services.DailyRevenue>> GetRevenueByDayAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var filter = Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.And(
            Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Gte(p => p.Date, startDate),
            Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Lte(p => p.Date, endDate),
            Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Eq(p => p.CreatorId, creatorId)
        );
        
        var performances = await _context.PerformanceMetrics
            .Find(filter)
            .ToListAsync();

        return performances
            .GroupBy(p => p.Date.Date)
            .Select(g => new Application.Interfaces.Services.DailyRevenue
            {
                Date = g.Key,
                Revenue = g.Sum(p => p.EstimatedRevenue)
            })
            .OrderBy(d => d.Date)
            .ToList();
    }

    private decimal CalculateMRR(IEnumerable<Application.Interfaces.Services.PlatformRevenue> platformRevenues)
    {
        // Calcular o MRR (Monthly Recurring Revenue) com base nas receitas por plataforma
        // Se não tivermos dados suficientes, fazemos uma estimativa baseada nos últimos 30 dias
        var totalLastMonth = platformRevenues.Sum(p => p.Revenue);
        
        // Aplicar um fator de correção de 0.8 a 1.2 baseado na tendência de crescimento
        // (em uma implementação real, seria calculada com base em dados históricos)
        var growthTrendFactor = 1.0m; // Neutro
        
        return totalLastMonth * growthTrendFactor;
    }

    private decimal CalculateARR(IEnumerable<Application.Interfaces.Services.PlatformRevenue> platformRevenues)
    {
        // Calcular o ARR (Annual Recurring Revenue)
        // Simplificado: MRR * 12
        return CalculateMRR(platformRevenues) * 12;
    }

    // Método para calcular métricas de monetização
    public async Task<Application.Interfaces.Services.MonetizationMetricsDto> GetMonetizationMetricsAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado");
            
        var totalRevenue = await GetTotalRevenueAsync(creatorId, startDate, endDate);
        var platformRevenues = await GetPlatformRevenueAsync(creatorId, startDate, endDate);
        
        // Calcular métricas básicas de monetização
        var metrics = new Application.Interfaces.Services.MonetizationMetricsDto
        {
            TotalRevenue = totalRevenue,
            EstimatedMonthlyRevenue = CalculateMRR(platformRevenues),
            EstimatedAnnualRevenue = CalculateARR(platformRevenues),
            RevenueByPlatform = platformRevenues.ToDictionary(p => p.Platform, p => p.Revenue),
            RevenuePerView = CalculateRevenuePerView(platformRevenues)
        };
        
        // Adicionar dados do AdSense se disponíveis
        if (creator.AdSenseSettings != null && creator.AdSenseSettings.IsConnected)
        {
            metrics.AdSenseMetrics = new Application.Interfaces.Services.AdSenseMetricsDto
            {
                EstimatedMonthlyRevenue = creator.AdSenseSettings.EstimatedMonthlyRevenue,
                TotalClicks = creator.AdSenseSettings.TotalClicks,
                TotalImpressions = creator.AdSenseSettings.TotalImpressions,
                Ctr = creator.AdSenseSettings.Ctr,
                Rpm = creator.AdSenseSettings.Rpm,
                LastUpdated = creator.AdSenseSettings.LastUpdated
            };
        }
        
        return metrics;
    }
    
    private decimal CalculateRevenuePerView(IEnumerable<Application.Interfaces.Services.PlatformRevenue> platformRevenues)
    {
        var totalViews = platformRevenues.Sum(p => p.Views);
        var totalRevenue = platformRevenues.Sum(p => p.Revenue);
        
        if (totalViews == 0)
            return 0;
            
        return totalRevenue / totalViews;
    }

    public async Task<decimal> CalculateContentRevenueAsync(Guid contentId)
    {
        var filter = Builders<MicroSaaS.Domain.Entities.ContentPerformance>.Filter.Eq(p => p.PostId, contentId);
        var performances = await _context.PerformanceMetrics
            .Find(filter)
            .ToListAsync();
        return performances.Sum(p => p.EstimatedRevenue);
    }

    public async Task<decimal> CalculateCreatorRevenueAsync(Guid creatorId)
    {
        var endDate = DateTime.UtcNow;
        var startDate = DateTime.MinValue;
        return await GetTotalRevenueAsync(creatorId, startDate, endDate);
    }

    public async Task RefreshRevenueMetricsAsync()
    {
        var creators = await _creatorRepository.GetAllAsync();
        foreach (var creator in creators)
        {
            if (creator.AdSenseSettings?.IsConnected == true && !string.IsNullOrEmpty(creator.AdSenseSettings.RefreshToken))
            {
                 await RefreshAdSenseTokenAsync(creator);
            }
        }
        await Task.CompletedTask;
    }

    private async Task RefreshAdSenseTokenAsync(ContentCreator creator)
    {
        if (creator?.AdSenseSettings == null || string.IsNullOrEmpty(creator.AdSenseSettings.RefreshToken))
            return;
            
        var clientId = _configuration["Revenue:AdSense:ClientId"];
        var clientSecret = _configuration["Revenue:AdSense:ClientSecret"];
        
        if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            return;
            
        try
        {
            var tokenEndpoint = "https://oauth2.googleapis.com/token";
            
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "refresh_token", creator.AdSenseSettings.RefreshToken },
                { "grant_type", "refresh_token" }
            });
            
            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();
                if (tokenResponse != null)
                {
                    // Atualizar o token
                    creator.AdSenseSettings.AccessToken = tokenResponse.AccessToken;
                    creator.AdSenseSettings.TokenExpiresAt = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
                    
                    await _creatorRepository.UpdateAsync(creator);
                }
            }
        }
        catch (Exception ex)
        {
            // Logar o erro
            Console.WriteLine($"Erro ao atualizar token do AdSense: {ex.Message}");
        }
    }

    // Classes auxiliares para deserialização das respostas do Google AdSense
    private class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;
        
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = string.Empty;
        
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = string.Empty;
    }
    
    private class AdSenseAccountsResponse
    {
        [JsonPropertyName("accounts")]
        public List<AdSenseAccount> Accounts { get; set; } = new List<AdSenseAccount>();
    }
    
    private class AdSenseAccount
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = string.Empty;
        
        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; } = string.Empty;
    }
    
    private class AdSenseReportResponse
    {
        [JsonPropertyName("rows")]
        public List<object> Rows { get; set; } = new List<object>();
        
        [JsonPropertyName("totals")]
        public List<object> Totals { get; set; } = new List<object>();
    }
} 