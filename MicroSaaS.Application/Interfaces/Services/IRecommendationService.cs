using MicroSaaS.Domain.Entities;
// using MicroSaaS.Application.DTOs.Recommendation;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Interfaces.Services;

public interface IRecommendationService
{
    // Recomendações de tempo de postagem
    Task<List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform);
    Task<Dictionary<SocialMediaPlatform, List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>>> GetBestTimeToPostAllPlatformsAsync(Guid creatorId);
    
    // Recomendações de conteúdo
    Task<List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>> GetContentRecommendationsAsync(Guid creatorId);
    Task<List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>> GetTopicRecommendationsAsync(Guid creatorId);
    Task<List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>> GetFormatRecommendationsAsync(Guid creatorId);
    
    // Recomendações de hashtags
    Task<List<string>> GetHashtagRecommendationsAsync(Guid creatorId, string contentDescription, SocialMediaPlatform platform);
    
    // Identificação de tendências
    Task<List<MicroSaaS.Shared.DTOs.TrendTopicDto>> GetTrendingTopicsAsync(SocialMediaPlatform platform);
    Task<List<MicroSaaS.Shared.DTOs.TrendTopicDto>> GetNicheTrendingTopicsAsync(Guid creatorId);
    
    // Outras recomendações estratégicas
    Task<List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>> GetMonetizationRecommendationsAsync(Guid creatorId);
    Task<List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>> GetAudienceGrowthRecommendationsAsync(Guid creatorId);
    Task<List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>> GetEngagementImprovementRecommendationsAsync(Guid creatorId);
    
    // Análise de conteúdo
    Task<MicroSaaS.Shared.DTOs.ContentAnalysisDto> AnalyzeContentPerformanceAsync(Guid contentId);
    
    // Atualização de recomendações
    Task RefreshRecommendationsAsync(Guid creatorId);

    // Novos métodos para DTOs
    Task<List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>> GetBestPostingTimesAsync(Guid creatorId, SocialMediaPlatform platform);
    Task<List<MicroSaaS.Shared.DTOs.ContentRecommendationDto>> GetContentRecommendationsAsync(Guid creatorId, SocialMediaPlatform platform);
    Task<List<MicroSaaS.Shared.DTOs.GrowthRecommendationDto>> GetGrowthRecommendationsAsync(Guid creatorId, SocialMediaPlatform platform);
    Task<MicroSaaS.Shared.DTOs.ContentRecommendationDto> GenerateCustomRecommendationAsync(CustomRecommendationRequestDto request);
    Task<MicroSaaS.Shared.DTOs.AudienceSensitivityDto> GetAudienceSensitivityAnalysisAsync(Guid creatorId, SocialMediaPlatform platform);
    Task<MicroSaaS.Shared.DTOs.GrowthRecommendationDto> GenerateCustomGrowthRecommendationAsync(CustomRecommendationRequestDto request);
}

public class TrendTopic
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double PopularityScore { get; set; }
    public DateTime DiscoveredAt { get; set; } = DateTime.UtcNow;
    public List<string> RelatedHashtags { get; set; } = new();
    public SocialMediaPlatform Platform { get; set; }
}

public class ContentAnalysis
{
    public Guid ContentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public double EngagementScore { get; set; }
    public double ReachScore { get; set; }
    public double ConversionScore { get; set; }
    public Dictionary<string, double> PerformanceFactors { get; set; } = new();
    public List<string> StrengthPoints { get; set; } = new();
    public List<string> ImprovementSuggestions { get; set; } = new();
    public Dictionary<SocialMediaPlatform, double> PlatformPerformance { get; set; } = new();
} 