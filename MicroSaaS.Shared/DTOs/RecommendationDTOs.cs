using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Shared.DTOs
{
    public class BestTimeSlotDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; set; }

        [JsonPropertyName("platform")]
        public SocialMediaPlatform Platform { get; set; }

        [JsonPropertyName("dayOfWeek")]
        public DayOfWeek DayOfWeek { get; set; }

        [JsonPropertyName("timeOfDay")]
        public TimeSpan TimeOfDay { get; set; }

        [JsonPropertyName("engagementScore")]
        public decimal EngagementScore { get; set; }

        [JsonPropertyName("confidenceScore")]
        public decimal ConfidenceScore { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("hour")]
        public int Hour { get; set; }

        [JsonPropertyName("engagementPotential")]
        public int EngagementPotential { get; set; }

        [JsonPropertyName("recommendationStrength")]
        public string RecommendationStrength { get; set; } = string.Empty;
    }

    // Enum RecommendationType movido para MicroSaaS.Shared.Enums.RecommendationType

    public enum GrowthCategory
    {
        Engagement,
        Reach,
        Followers,
        ContentQuality,
        Monetization
    }

    public class ContentRecommendationDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public MicroSaaS.Shared.Enums.RecommendationType Type { get; set; }

        [JsonPropertyName("platform")]
        public SocialMediaPlatform Platform { get; set; }

        [JsonPropertyName("confidenceScore")]
        public decimal ConfidenceScore { get; set; }

        [JsonPropertyName("score")]
        public decimal Score { get; set; }

        [JsonPropertyName("exampleContentIds")]
        public List<Guid> ExampleContentIds { get; set; } = new List<Guid>();

        [JsonPropertyName("suggestedHashtags")]
        public List<string> SuggestedHashtags { get; set; } = new List<string>();

        [JsonPropertyName("suggestedKeywords")]
        public List<string> SuggestedKeywords { get; set; } = new List<string>();

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }

    public class GrowthRecommendationDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public GrowthCategory Category { get; set; }

        [JsonPropertyName("platform")]
        public SocialMediaPlatform Platform { get; set; }

        [JsonPropertyName("implementationSteps")]
        public List<string> ImplementationSteps { get; set; } = new List<string>();

        [JsonPropertyName("expectedOutcome")]
        public string ExpectedOutcome { get; set; } = string.Empty;

        [JsonPropertyName("difficulty")]
        public int Difficulty { get; set; } // 1-5 scale

        [JsonPropertyName("timeToImplement")]
        public string TimeToImplement { get; set; } = string.Empty;

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }

    public class ContentRecommendationsDto
    {
        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; set; }

        [JsonPropertyName("recommendations")]
        public List<ContentRecommendationDto> Recommendations { get; set; } = new List<ContentRecommendationDto>();

        [JsonPropertyName("generatedAt")]
        public DateTime GeneratedAt { get; set; }
    }

    public class TopicSensitivityDto
    {
        [JsonPropertyName("topic")]
        public string Topic { get; set; } = string.Empty;

        [JsonPropertyName("sensitivityLevel")]
        public int SensitivityLevel { get; set; }

        [JsonPropertyName("recommendedApproach")]
        public string RecommendedApproach { get; set; } = string.Empty;
    }

    public class AudienceSensitivityDto
    {
        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; set; }

        [JsonPropertyName("platform")]
        public string Platform { get; set; } = string.Empty;

        [JsonPropertyName("topResponsiveTopics")]
        public List<string> TopResponsiveTopics { get; set; } = new List<string>();

        [JsonPropertyName("topResponsiveFormats")]
        public List<string> TopResponsiveFormats { get; set; } = new List<string>();

        [JsonPropertyName("bestTimeOfDay")]
        public List<TimeSpan> BestTimeOfDay { get; set; } = new List<TimeSpan>();

        [JsonPropertyName("bestDaysOfWeek")]
        public List<DayOfWeek> BestDaysOfWeek { get; set; } = new List<DayOfWeek>();

        [JsonPropertyName("confidenceScore")]
        public decimal ConfidenceScore { get; set; }

        [JsonPropertyName("lastUpdated")]
        public DateTime LastUpdated { get; set; }

        [JsonPropertyName("sensitivityTopics")]
        public List<TopicSensitivityDto> SensitivityTopics { get; set; } = new List<TopicSensitivityDto>();

        [JsonPropertyName("overallSensitivity")]
        public double OverallSensitivity { get; set; }

        [JsonPropertyName("analysis")]
        public string Analysis { get; set; } = string.Empty;

        [JsonPropertyName("recommendedContentApproach")]
        public string RecommendedContentApproach { get; set; } = string.Empty;
    }

    public class RecommendationDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("type")]
        public MicroSaaS.Shared.Enums.RecommendationType Type { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("score")]
        public decimal Score { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }
    }

    public class CustomRecommendationRequestDto
    {
        [JsonPropertyName("creatorId")]
        public Guid CreatorId { get; set; }

        [JsonPropertyName("platform")]
        public SocialMediaPlatform Platform { get; set; }

        [JsonPropertyName("recommendationType")]
        public MicroSaaS.Shared.Enums.RecommendationType RecommendationType { get; set; }

        [JsonPropertyName("specificTopic")]
        public string? SpecificTopic { get; set; }

        [JsonPropertyName("targetAudience")]
        public string? TargetAudience { get; set; }

        [JsonPropertyName("contentGoal")]
        public string? ContentGoal { get; set; }

        [JsonPropertyName("preferredContentLength")]
        public string? PreferredContentLength { get; set; }
    }

    public class TrendTopicDto
    {
        [JsonPropertyName("topic")]
        public string Topic { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("volume")]
        public double Volume { get; set; }

        [JsonPropertyName("platform")]
        public SocialMediaPlatform Platform { get; set; }

        [JsonPropertyName("relatedHashtags")]
        public List<string> RelatedHashtags { get; set; } = new();
    }

    public class ContentAnalysisDto
    {
        [JsonPropertyName("contentId")]
        public Guid ContentId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("engagementScore")]
        public double EngagementScore { get; set; }

        [JsonPropertyName("reachScore")]
        public double ReachScore { get; set; }

        [JsonPropertyName("strengthPoints")]
        public List<string> StrengthPoints { get; set; } = new();

        [JsonPropertyName("improvementSuggestions")]
        public List<string> ImprovementSuggestions { get; set; } = new();

        [JsonPropertyName("analysisDate")]
        public DateTime AnalysisDate { get; set; }
    }
}