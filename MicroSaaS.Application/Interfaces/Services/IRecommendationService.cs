using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Interfaces.Services;

public interface IRecommendationService
{
    // Recomendações de tempo de postagem
    Task<List<PostTimeRecommendation>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform);
    Task<Dictionary<SocialMediaPlatform, List<PostTimeRecommendation>>> GetBestTimeToPostAllPlatformsAsync(Guid creatorId);
    
    // Recomendações de conteúdo
    Task<List<ContentRecommendation>> GetContentRecommendationsAsync(Guid creatorId);
    Task<List<ContentRecommendation>> GetTopicRecommendationsAsync(Guid creatorId);
    Task<List<ContentRecommendation>> GetFormatRecommendationsAsync(Guid creatorId);
    
    // Recomendações de hashtags
    Task<List<string>> GetHashtagRecommendationsAsync(Guid creatorId, string contentDescription, SocialMediaPlatform platform);
    
    // Identificação de tendências
    Task<List<TrendTopic>> GetTrendingTopicsAsync(SocialMediaPlatform platform);
    Task<List<TrendTopic>> GetNicheTrendingTopicsAsync(Guid creatorId);
    
    // Outras recomendações estratégicas
    Task<List<ContentRecommendation>> GetMonetizationRecommendationsAsync(Guid creatorId);
    Task<List<ContentRecommendation>> GetAudienceGrowthRecommendationsAsync(Guid creatorId);
    Task<List<ContentRecommendation>> GetEngagementImprovementRecommendationsAsync(Guid creatorId);
    
    // Análise de conteúdo
    Task<ContentAnalysis> AnalyzeContentPerformanceAsync(Guid contentId);
    
    // Atualização de recomendações
    Task RefreshRecommendationsAsync(Guid creatorId);
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