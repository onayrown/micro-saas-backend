using Microsoft.Extensions.DependencyInjection;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Shared.Results;
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
    public Task<Result<ContentInsightsDto>> GetContentInsightsAsync(Guid contentId)
    {
        return Task.FromResult(Result<ContentInsightsDto>.Fail("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure."));
    }

    public Task<Result<HighPerformancePatternDto>> AnalyzeHighPerformancePatternsAsync(Guid creatorId, int topPostsCount = 20)
    {
        return Task.FromResult(Result<HighPerformancePatternDto>.Fail("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure."));
    }

    public Task<Result<AudienceSensitivityDto>> AnalyzeAudienceSensitivityAsync(Guid creatorId)
    {
        return Task.FromResult(Result<AudienceSensitivityDto>.Fail("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure."));
    }
    
    public Task<Result<ContentRecommendationsDto>> GenerateContentRecommendationsAsync(Guid creatorId)
    {
        return Task.FromResult(Result<ContentRecommendationsDto>.Fail("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure."));
    }

    public Task<Result<AudienceInsightsDto>> GetAudienceInsightsAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        return Task.FromResult(Result<AudienceInsightsDto>.Fail("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure."));
    }

    public Task<Result<ContentComparisonDto>> CompareContentTypesAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        return Task.FromResult(Result<ContentComparisonDto>.Fail("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure."));
    }

    public Task<Result<ContentPredictionDto>> PredictContentPerformanceAsync(ContentPredictionRequestDto request)
    {
        return Task.FromResult(Result<ContentPredictionDto>.Fail("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure."));
    }

    public Task<Result<List<EngagementFactorDto>>> IdentifyEngagementFactorsAsync(Guid creatorId)
    {
        return Task.FromResult(Result<List<EngagementFactorDto>>.Fail("Esta é uma implementação de fallback. Use a implementação real na camada Infrastructure."));
    }
} 