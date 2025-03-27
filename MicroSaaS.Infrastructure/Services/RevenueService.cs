using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Services;

public class RevenueService : IRevenueService
{
    private readonly IConfiguration _configuration;
    private readonly IContentCreatorRepository _creatorRepository;

    public RevenueService(
        IConfiguration configuration,
        IContentCreatorRepository creatorRepository)
    {
        _configuration = configuration;
        _creatorRepository = creatorRepository;
    }

    public Task<string> GetAdSenseAuthUrlAsync(Guid creatorId, string callbackUrl)
    {
        // Gerar URL para autenticação com o Google AdSense
        var clientId = _configuration["Revenue:AdSense:ClientId"];
        var scope = "https://www.googleapis.com/auth/adsense.readonly";
        var authUrl = $"https://accounts.google.com/o/oauth2/auth" +
                     $"?client_id={clientId}" +
                     $"&redirect_uri={Uri.EscapeDataString(callbackUrl)}" +
                     $"&scope={Uri.EscapeDataString(scope)}" +
                     $"&state={creatorId}" +
                     $"&response_type=code" +
                     $"&access_type=offline";

        return Task.FromResult(authUrl);
    }

    public async Task<bool> ConnectAdSenseAsync(Guid creatorId, string authorizationCode)
    {
        // Na implementação real, trocaríamos o código por um token de acesso e atualizaríamos o criador
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            return false;

        // Simular um token de acesso
        var accessToken = $"mock_adsense_token_{Guid.NewGuid()}";
        
        // Integrar com AdSense
        return await IntegrateGoogleAdSenseAsync(creator, accessToken);
    }

    public async Task<bool> IntegrateGoogleAdSenseAsync(ContentCreator creator, string accessToken)
    {
        // Na implementação real, armazenamos o token e associamos a conta ao criador
        
        // Salvar as alterações
        await _creatorRepository.UpdateAsync(creator);
        
        return true;
    }

    public Task<decimal> GetEstimatedRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        // Implementação simulada - gerar um valor aleatório para demonstração
        var days = (endDate - startDate).Days + 1;
        var revenue = Math.Round((decimal)(Random.Shared.NextDouble() * 100 * days), 2);
        
        return Task.FromResult(revenue);
    }

    public async Task<RevenueSummary> GetRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        // Implementação simulada - em um ambiente real, isso buscaria dados do AdSense API
        var totalRevenue = await GetEstimatedRevenueAsync(creatorId, startDate, endDate);
        var daysInPeriod = (endDate - startDate).Days + 1;
        
        return new RevenueSummary
        {
            TotalRevenue = totalRevenue,
            EstimatedMonthlyRevenue = totalRevenue * 30 / daysInPeriod,
            AverageRevenuePerView = Math.Round(0.01m * (decimal)Random.Shared.NextDouble(), 4)
        };
    }

    public async Task<List<PlatformRevenue>> GetRevenueByPlatformAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        // Implementação simulada - gerar dados aleatórios para demonstração
        var totalRevenue = await GetEstimatedRevenueAsync(creatorId, startDate, endDate);
        
        // Distribuir a receita entre as plataformas
        var platformRevenues = new List<PlatformRevenue>
        {
            new PlatformRevenue
            {
                Platform = "YouTube",
                Revenue = Math.Round(totalRevenue * 0.6m, 2),
                Views = Random.Shared.Next(1000, 100000)
            },
            new PlatformRevenue
            {
                Platform = "Instagram",
                Revenue = Math.Round(totalRevenue * 0.3m, 2),
                Views = Random.Shared.Next(5000, 50000)
            },
            new PlatformRevenue
            {
                Platform = "TikTok",
                Revenue = Math.Round(totalRevenue * 0.1m, 2),
                Views = Random.Shared.Next(10000, 200000)
            }
        };
        
        return platformRevenues;
    }

    public async Task<List<DailyRevenue>> GetRevenueByDayAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        // Implementação simulada - gerar dados aleatórios para demonstração
        var totalRevenue = await GetEstimatedRevenueAsync(creatorId, startDate, endDate);
        var daysInPeriod = (endDate - startDate).Days + 1;
        var avgDailyRevenue = totalRevenue / daysInPeriod;
        
        var dailyRevenues = new List<DailyRevenue>();
        var currentDate = startDate;
        
        while (currentDate <= endDate)
        {
            // Adicionar alguma variação para tornar os dados mais realistas
            var variation = (decimal)(Random.Shared.NextDouble() * 0.5 + 0.75); // Entre 0.75 e 1.25
            var dailyRevenue = Math.Round(avgDailyRevenue * variation, 2);
            
            dailyRevenues.Add(new DailyRevenue
            {
                Date = currentDate,
                Revenue = dailyRevenue,
                Views = Random.Shared.Next(1000, 10000)
            });
            
            currentDate = currentDate.AddDays(1);
        }
        
        return dailyRevenues;
    }
} 