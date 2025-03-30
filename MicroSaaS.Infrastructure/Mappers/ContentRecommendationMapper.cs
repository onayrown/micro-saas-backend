using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using System;

namespace MicroSaaS.Infrastructure.Mappers;

public static class ContentRecommendationMapper
{
    public static ContentRecommendation ToDomain(ContentRecommendationEntity entity)
    {
        if (entity == null) return null;

        return new ContentRecommendation
        {
            Id = entity.Id,
            CreatorId = entity.CreatorId,
            Title = entity.Title,
            Description = entity.Description,
            Type = entity.Type,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            RecommendationType = entity.RecommendationType,
            SuggestedTopics = entity.SuggestedTopics,
            Platform = entity.Platform,
            Priority = entity.Priority,
            PotentialImpact = entity.PotentialImpact,
            RecommendedAction = entity.RecommendedAction
        };
    }

    public static ContentRecommendationEntity ToEntity(ContentRecommendation domain)
    {
        if (domain == null) return null;

        return new ContentRecommendationEntity
        {
            Id = domain.Id,
            CreatorId = domain.CreatorId,
            Title = domain.Title,
            Description = domain.Description,
            Type = domain.Type,
            CreatedAt = domain.CreatedAt != default ? domain.CreatedAt : DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            RecommendationType = domain.RecommendationType,
            SuggestedTopics = domain.SuggestedTopics,
            Platform = domain.Platform,
            Priority = domain.Priority,
            PotentialImpact = domain.PotentialImpact,
            RecommendedAction = domain.RecommendedAction
        };
    }
} 