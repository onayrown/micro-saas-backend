using Microsoft.Extensions.DependencyInjection;
using MicroSaaS.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Adicionar ContentAnalysisService - implementação real estará na camada de Infra
        services.AddScoped<IContentAnalysisService, FallbackContentAnalysisService>();
        
        return services;
    }
}

/// <summary>
/// Implementação temporária de fallback para quando a implementação real não está disponível
/// </summary>
public class FallbackContentAnalysisService : IContentAnalysisService
{
    public Task<ContentInsightsDto> GetContentInsightsAsync(Guid contentId)
    {
        throw new NotImplementedException("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure.");
    }

    public Task<HighPerformancePatternDto> AnalyzeHighPerformancePatternsAsync(Guid creatorId, int topPostsCount = 20)
    {
        throw new NotImplementedException("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure.");
    }

    public Task<AudienceSensitivityDto> AnalyzeAudienceSensitivityAsync(Guid creatorId)
    {
        throw new NotImplementedException("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure.");
    }
    
    public Task<ContentRecommendationsDto> GenerateContentRecommendationsAsync(Guid creatorId)
    {
        throw new NotImplementedException("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure.");
    }

    public Task<AudienceInsightsDto> GetAudienceInsightsAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure.");
    }

    public Task<ContentComparisonDto> CompareContentTypesAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure.");
    }

    public Task<ContentPredictionDto> PredictContentPerformanceAsync(ContentPredictionRequestDto request)
    {
        throw new NotImplementedException("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure.");
    }

    public Task<List<EngagementFactorDto>> IdentifyEngagementFactorsAsync(Guid creatorId)
    {
        throw new NotImplementedException("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure.");
    }
} 