using MicroSaaS.Domain.Entities;
// using MicroSaaS.Shared.Models; // Remover referência direta aos modelos do Shared na interface
using MicroSaaS.Application.DTOs.Revenue; // Adicionar using para DTOs da Application
using MicroSaaS.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Interfaces.Services;

public interface IRevenueService
{
    // Conexão com AdSense
    Task<string> GetAdSenseAuthUrlAsync(Guid creatorId, string callbackUrl);
    Task<bool> ConnectAdSenseAsync(Guid creatorId, string authorizationCode);
    Task<bool> IntegrateGoogleAdSenseAsync(ContentCreator creator, string accessToken);
    
    // Relatórios de Receita - Atualizado para usar DTOs da Application
    Task<decimal> GetEstimatedRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    Task<RevenueSummaryDto> GetRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate); // Retorna RevenueSummaryDto
    Task<IEnumerable<PlatformRevenueDto>> GetRevenueByPlatformAsync(Guid creatorId, DateTime startDate, DateTime endDate); // Retorna IEnumerable<PlatformRevenueDto>
    Task<IEnumerable<DailyRevenueDto>> GetRevenueByDayAsync(Guid creatorId, DateTime startDate, DateTime endDate); // Retorna IEnumerable<DailyRevenueDto>

    Task<decimal> GetTotalRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    
    // Métodos adicionais
    Task<decimal> CalculateContentRevenueAsync(Guid contentId);
    Task<decimal> CalculateCreatorRevenueAsync(Guid creatorId);
    Task RefreshRevenueMetricsAsync();
    
    // Métricas de monetização avançadas
    Task<MonetizationMetricsDto> GetMonetizationMetricsAsync(Guid creatorId, DateTime startDate, DateTime endDate);
}

// REMOVER DEFINIÇÕES DE CLASSE DAQUI
/*
public class RevenueSummary
{
    // ... removido ...
}

public class PlatformRevenue
{
    // ... removido ...
}

public class DailyRevenue
{
    // ... removido ...
}

public class MonetizationMetricsDto
{
    // ... removido ... // TODO: Verificar se este DTO deve existir na Application
}

public class AdSenseMetricsDto
{
    // ... removido ... // TODO: Verificar se este DTO deve existir na Application
}
*/ 