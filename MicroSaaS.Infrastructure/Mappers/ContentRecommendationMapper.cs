using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;

namespace MicroSaaS.Infrastructure.Mappers;

public static class ContentRecommendationMapper
{
    public static ContentRecommendation ToDomain(ContentRecommendationEntity entity)
    {
        if (entity == null) return null;

        return new ContentRecommendation
        {
            RecommendationType = entity.RecommendationType,
            Description = entity.Description,
            SuggestedTopics = entity.SuggestedTopics,
            Platform = entity.Platform,
            Priority = entity.Priority,
            PotentialImpact = entity.PotentialImpact
        };
    }

    public static ContentRecommendationEntity ToEntity(ContentRecommendation domain)
    {
        if (domain == null) return null;

        return new ContentRecommendationEntity
        {
            RecommendationType = domain.RecommendationType,
            Description = domain.Description,
            SuggestedTopics = domain.SuggestedTopics,
            Platform = domain.Platform,
            Priority = domain.Priority,
            PotentialImpact = domain.PotentialImpact,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
} 