using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Application.Interfaces.Services;

/// <summary>
/// Interface para o serviço de análise de conteúdo
/// </summary>
public interface IContentAnalysisService
{
    /// <summary>
    /// Analisa um conteúdo específico e provê insights detalhados
    /// </summary>
    Task<ContentInsightsDto> GetContentInsightsAsync(Guid contentId);
    
    /// <summary>
    /// Analisa padrões de alto desempenho no conteúdo de um criador
    /// </summary>
    Task<HighPerformancePatternDto> AnalyzeHighPerformancePatternsAsync(Guid creatorId, int topPostsCount = 20);
    
    /// <summary>
    /// Identifica padrões de audiência e comportamento
    /// </summary>
    Task<AudienceInsightsDto> GetAudienceInsightsAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Compara performance entre diferentes tipos de conteúdo
    /// </summary>
    Task<ContentComparisonDto> CompareContentTypesAsync(Guid creatorId, DateTime startDate, DateTime endDate);
    
    /// <summary>
    /// Prevê o desempenho de um conteúdo com base em suas características
    /// </summary>
    Task<ContentPredictionDto> PredictContentPerformanceAsync(ContentPredictionRequestDto request);
    
    /// <summary>
    /// Identifica fatores que mais contribuem para o engajamento
    /// </summary>
    Task<List<EngagementFactorDto>> IdentifyEngagementFactorsAsync(Guid creatorId);
    
    /// <summary>
    /// Analisa a sensibilidade da audiência a tipos específicos de conteúdo
    /// </summary>
    Task<AudienceSensitivityDto> AnalyzeAudienceSensitivityAsync(Guid creatorId);

    /// <summary>
    /// Gera recomendações de conteúdo personalizadas para um criador
    /// </summary>
    Task<ContentRecommendationsDto> GenerateContentRecommendationsAsync(Guid creatorId);
}

/// <summary>
/// DTO para insights detalhados sobre um conteúdo
/// </summary>
public class ContentInsightsDto
{
    public Guid ContentId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public double OverallScore { get; set; }
    public double EngagementScore { get; set; }
    public double ReachScore { get; set; }
    public double ConversionScore { get; set; }
    public double RetentionScore { get; set; }
    public double SentimentScore { get; set; }
    public Dictionary<string, double> PerformanceFactors { get; set; } = new();
    public List<string> StrengthPoints { get; set; } = new();
    public List<string> ImprovementSuggestions { get; set; } = new();
    public Dictionary<SocialMediaPlatform, double> PlatformPerformance { get; set; } = new();
    public ViralPotentialDto ViralPotential { get; set; } = new();
    public AudienceResponseDto AudienceResponse { get; set; } = new();
    public List<string> KeyAttributes { get; set; } = new();
    public List<CompetitorInsightDto> CompetitorInsights { get; set; } = new();
}

/// <summary>
/// DTO para potencial viral de um conteúdo
/// </summary>
public class ViralPotentialDto
{
    public double Score { get; set; }
    public string Assessment { get; set; } = string.Empty;
    public List<string> KeyFactors { get; set; } = new();
    public Dictionary<string, double> ShareProbabilities { get; set; } = new();
}

/// <summary>
/// DTO para resposta da audiência a um conteúdo
/// </summary>
public class AudienceResponseDto
{
    public double PositiveSentiment { get; set; }
    public double NegativeSentiment { get; set; }
    public double NeutralSentiment { get; set; }
    public List<string> CommonFeedback { get; set; } = new();
    public List<DemographicResponseDto> DemographicBreakdown { get; set; } = new();
}

/// <summary>
/// DTO para resposta demográfica
/// </summary>
public class DemographicResponseDto
{
    public string Demographic { get; set; } = string.Empty;
    public double EngagementRate { get; set; }
    public string ResponseType { get; set; } = string.Empty;
}

/// <summary>
/// DTO para insights sobre competidores
/// </summary>
public class CompetitorInsightDto
{
    public string CompetitorName { get; set; } = string.Empty;
    public double RelativePerformance { get; set; }
    public List<string> DifferentiatingFactors { get; set; } = new();
}

/// <summary>
/// DTO para padrões de alto desempenho
/// </summary>
public class HighPerformancePatternDto
{
    public Guid CreatorId { get; set; }
    public List<ContentPatternDto> IdentifiedPatterns { get; set; } = new();
    public List<TimingPatternDto> TimingPatterns { get; set; } = new();
    public List<TopicPatternDto> TopicPatterns { get; set; } = new();
    public List<FormatPatternDto> FormatPatterns { get; set; } = new();
    public List<StylePatternDto> StylePatterns { get; set; } = new();
    public Dictionary<string, double> AttributeCorrelations { get; set; } = new();
    public Dictionary<string, double> HighPerformingFormats { get; set; } = new();
}

/// <summary>
/// DTO para padrões de conteúdo
/// </summary>
public class ContentPatternDto
{
    public string PatternName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double ConfidenceScore { get; set; }
    public double AverageEngagement { get; set; }
    public List<string> ExampleContentIds { get; set; } = new();
    public List<string> Attributes { get; set; } = new();
}

/// <summary>
/// DTO para padrões de tempo
/// </summary>
public class TimingPatternDto
{
    public List<DayOfWeek> BestDays { get; set; } = new();
    public List<TimeSpan> BestTimes { get; set; } = new();
    public Dictionary<SocialMediaPlatform, List<BestTimeSlotDto>> PlatformSpecificTimes { get; set; } = new();
    public double ConfidenceScore { get; set; }
}

/// <summary>
/// DTO para melhor slot de tempo
/// </summary>
public class BestTimeSlotDto
{
    public DayOfWeek Day { get; set; }
    public TimeSpan Time { get; set; }
    public double EngagementScore { get; set; }
    public string Rationale { get; set; } = string.Empty;
}

/// <summary>
/// DTO para padrões de tópico
/// </summary>
public class TopicPatternDto
{
    public string TopicName { get; set; } = string.Empty;
    public double EngagementScore { get; set; }
    public double GrowthTrend { get; set; }
    public List<string> RelatedTopics { get; set; } = new();
    public List<string> Keywords { get; set; } = new();
}

/// <summary>
/// DTO para padrões de formato
/// </summary>
public class FormatPatternDto
{
    public string FormatName { get; set; } = string.Empty;
    public double EngagementScore { get; set; }
    public List<SocialMediaPlatform> BestPlatforms { get; set; } = new();
    public string OptimalDuration { get; set; } = string.Empty;
    public List<string> BestPractices { get; set; } = new();
}

/// <summary>
/// DTO para padrões de estilo
/// </summary>
public class StylePatternDto
{
    public string StyleName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public double AudienceReception { get; set; }
    public List<string> KeyCharacteristics { get; set; } = new();
}

/// <summary>
/// DTO para insights de audiência
/// </summary>
public class AudienceInsightsDto
{
    public Guid CreatorId { get; set; }
    public int TotalAudienceSize { get; set; }
    public double GrowthRate { get; set; }
    public Dictionary<string, double> DemographicBreakdown { get; set; } = new();
    public List<AudienceSegmentDto> KeySegments { get; set; } = new();
    public Dictionary<string, double> InterestDistribution { get; set; } = new();
    public Dictionary<SocialMediaPlatform, double> PlatformEngagement { get; set; } = new();
    public List<string> EngagementPatterns { get; set; } = new();
    public Dictionary<string, double> ContentPreferences { get; set; } = new();
    public LoyaltyMetricsDto LoyaltyMetrics { get; set; } = new();
}

/// <summary>
/// DTO para segmentos de audiência
/// </summary>
public class AudienceSegmentDto
{
    public string SegmentName { get; set; } = string.Empty;
    public double Percentage { get; set; }
    public List<string> KeyCharacteristics { get; set; } = new();
    public List<string> PreferredContent { get; set; } = new();
    public double EngagementRate { get; set; }
    public double GrowthRate { get; set; }
}

/// <summary>
/// DTO para métricas de lealdade
/// </summary>
public class LoyaltyMetricsDto
{
    public double ReturnRate { get; set; }
    public double AverageEngagementFrequency { get; set; }
    public double ContentConsumptionRate { get; set; }
    public double AdvocacyScore { get; set; }
    public List<string> LoyaltyFactors { get; set; } = new();
}

/// <summary>
/// DTO para comparação de conteúdo
/// </summary>
public class ContentComparisonDto
{
    public Guid CreatorId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Dictionary<string, ContentTypePerformanceDto> ContentTypePerformance { get; set; } = new();
    public Dictionary<SocialMediaPlatform, List<ContentTypePerformanceDto>> PlatformSpecificPerformance { get; set; } = new();
    public List<CrossPlatformInsightDto> CrossPlatformInsights { get; set; } = new();
    public Dictionary<string, double> AttributePerformance { get; set; } = new();
}

/// <summary>
/// DTO para performance de tipo de conteúdo
/// </summary>
public class ContentTypePerformanceDto
{
    public string ContentType { get; set; } = string.Empty;
    public int Count { get; set; }
    public double AverageEngagement { get; set; }
    public double AverageReach { get; set; }
    public double AverageConversion { get; set; }
    public double AverageRetention { get; set; }
    public double GrowthTrend { get; set; }
    public List<string> TopPerformingContentIds { get; set; } = new();
    public List<string> KeySuccessFactors { get; set; } = new();
}

/// <summary>
/// DTO para insights de cross-platform
/// </summary>
public class CrossPlatformInsightDto
{
    public string InsightName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<SocialMediaPlatform> RelatedPlatforms { get; set; } = new();
    public double ConfidenceScore { get; set; }
    public List<string> ActionableRecommendations { get; set; } = new();
}

/// <summary>
/// DTO para pedido de previsão de conteúdo
/// </summary>
public class ContentPredictionRequestDto
{
    public Guid CreatorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ContentType Type { get; set; }
    public SocialMediaPlatform TargetPlatform { get; set; }
    public List<string> Tags { get; set; } = new();
    public int EstimatedDurationSeconds { get; set; }
    public bool IncludesCallToAction { get; set; }
    public DayOfWeek PostDay { get; set; }
    public TimeSpan PostTime { get; set; }
    public Dictionary<string, string> AdditionalAttributes { get; set; } = new();
}

/// <summary>
/// DTO para previsão de conteúdo
/// </summary>
public class ContentPredictionDto
{
    public Guid RequestId { get; set; }
    public ContentPredictionRequestDto Request { get; set; } = new();
    public double PredictedEngagementScore { get; set; }
    public double PredictedReachScore { get; set; }
    public double PredictedViralPotential { get; set; }
    public Dictionary<string, double> MetricPredictions { get; set; } = new();
    public Dictionary<string, double> FactorConfidenceScores { get; set; } = new();
    public List<string> OptimizationSuggestions { get; set; } = new();
    public PredictedAudienceResponseDto PredictedAudience { get; set; } = new();
    public double ConfidenceScore { get; set; }
}

/// <summary>
/// DTO para resposta de audiência prevista
/// </summary>
public class PredictedAudienceResponseDto
{
    public Dictionary<string, double> DemographicAppeal { get; set; } = new();
    public Dictionary<string, double> SentimentDistribution { get; set; } = new();
    public List<string> LikelyFeedback { get; set; } = new();
    public double RetentionProbability { get; set; }
}

/// <summary>
/// DTO para fatores de engajamento
/// </summary>
public class EngagementFactorDto
{
    public string FactorName { get; set; } = string.Empty;
    public double Importance { get; set; }
    public string Description { get; set; } = string.Empty;
    public Dictionary<string, double> SubFactors { get; set; } = new();
    public List<string> OptimizationTips { get; set; } = new();
    public double ConfidenceScore { get; set; }
}

/// <summary>
/// DTO para sensibilidade de audiência
/// </summary>
public class AudienceSensitivityDto
{
    public Guid CreatorId { get; set; }
    public Dictionary<string, double> ContentTypeSensitivity { get; set; } = new();
    public Dictionary<string, double> TopicSensitivity { get; set; } = new();
    public Dictionary<string, double> StyleSensitivity { get; set; } = new();
    public Dictionary<string, double> TimingSensitivity { get; set; } = new();
    public List<AudiencePreferenceDto> TopPreferences { get; set; } = new();
    public List<AudienceAversion> TopAversions { get; set; } = new();
    public double OverallSensitivityScore { get; set; }
}

/// <summary>
/// DTO para preferência de audiência
/// </summary>
public class AudiencePreferenceDto
{
    public string PreferenceName { get; set; } = string.Empty;
    public double StrengthScore { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> Examples { get; set; } = new();
}

/// <summary>
/// DTO para aversão de audiência
/// </summary>
public class AudienceAversion
{
    public string AversionName { get; set; } = string.Empty;
    public double StrengthScore { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<string> Examples { get; set; } = new();
}

/// <summary>
/// Recomendações de conteúdo para um criador
/// </summary>
public class ContentRecommendationsDto
{
    public List<TopicRecommendationDto> ContentTopics { get; set; } = new();
    public List<FormatRecommendationDto> ContentFormats { get; set; } = new();
    public List<string> ContentStrategies { get; set; } = new();
    public List<string> EngagementTactics { get; set; } = new();
    public List<string> MonetizationOpportunities { get; set; } = new();
}

/// <summary>
/// Recomendação de tópico de conteúdo
/// </summary>
public class TopicRecommendationDto
{
    public string Topic { get; set; } = string.Empty;
    public double RelevanceScore { get; set; }
    public string PotentialReach { get; set; } = string.Empty;
    public string RecommendationReason { get; set; } = string.Empty;
}

/// <summary>
/// Recomendação de formato de conteúdo
/// </summary>
public class FormatRecommendationDto
{
    public string Format { get; set; } = string.Empty;
    public double PerformanceScore { get; set; }
    public string IdealLength { get; set; } = string.Empty;
    public List<string> BestPractices { get; set; } = new();
}

/// <summary>
/// Tendências de mercado para criadores de conteúdo
/// </summary>
public class MarketTrendsDto
{
    public List<TrendItemDto> GeneralTrends { get; set; } = new();
    public List<TrendItemDto> NicheTrends { get; set; } = new();
    public List<string> EmergingTopics { get; set; } = new();
    public Dictionary<string, double> CategoryGrowth { get; set; } = new();
}

/// <summary>
/// Item de tendência de mercado
/// </summary>
public class TrendItemDto
{
    public string Topic { get; set; } = string.Empty;
    public double GrowthRate { get; set; }
    public string TimeFrame { get; set; } = string.Empty;
    public List<string> Demographics { get; set; } = new();
}

/// <summary>
/// Dados de audiência de um criador
/// </summary>
public class AudienceDataDto
{
    public Dictionary<string, double> DemographicBreakdown { get; set; } = new();
    public Dictionary<string, double> InterestCategories { get; set; } = new();
    public Dictionary<string, double> EngagementMetrics { get; set; } = new();
    public Dictionary<SocialMediaPlatform, double> PlatformPreferences { get; set; } = new();
} 