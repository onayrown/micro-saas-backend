using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using System;

namespace MicroSaaS.Infrastructure.Mappers;

public static class ContentPerformanceMapper
{
    public static ContentPerformance ToDomain(ContentPerformanceEntity entity)
    {
        if (entity == null) return null;

        return new ContentPerformance
        {
            Id = entity.Id,
            PostId = entity.PostId,
            CreatorId = entity.CreatorId,
            Platform = entity.Platform,
            Date = entity.Date,
            CollectedAt = entity.CollectedAt,
            Views = entity.Views,
            Likes = entity.Likes,
            Comments = entity.Comments,
            Shares = entity.Shares,
            EngagementRate = entity.EngagementRate,
            EstimatedRevenue = entity.EstimatedRevenue,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static ContentPerformanceEntity ToEntity(ContentPerformance domain)
    {
        if (domain == null) return null;

        return new ContentPerformanceEntity
        {
            Id = domain.Id,
            PostId = domain.PostId,
            CreatorId = domain.CreatorId,
            Platform = domain.Platform,
            Date = domain.Date,
            CollectedAt = domain.CollectedAt,
            Views = domain.Views,
            Likes = domain.Likes,
            Comments = domain.Comments,
            Shares = domain.Shares,
            EngagementRate = domain.EngagementRate,
            EstimatedRevenue = domain.EstimatedRevenue,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
} 