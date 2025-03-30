using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MicroSaaS.Domain.Entities;

public class ContentRecommendation
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public RecommendationType Type { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public RecommendationType RecommendationType { get; set; }
    public List<string> SuggestedTopics { get; set; } = new();
    public SocialMediaPlatform Platform { get; set; }
    public RecommendationPriority Priority { get; set; }
    public string PotentialImpact { get; set; } = string.Empty;
    public string RecommendedAction { get; set; } = string.Empty;
} 