using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using System;
using System.Collections.Generic;

namespace MicroSaaS.Infrastructure.Mappers;

public static class PerformanceMetricsMapper
{
    public static PerformanceMetricsEntity ToEntity(this PerformanceMetrics metrics)
    {
        if (metrics == null) return null;

        return new PerformanceMetricsEntity
        {
            Id = metrics.Id.ToString(),
            CreatorId = metrics.CreatorId.ToString(),
            Platform = metrics.Platform,
            Date = metrics.Date,
            Followers = metrics.Followers,
            FollowersGrowth = metrics.FollowersGrowth,
            TotalViews = metrics.TotalViews,
            TotalLikes = metrics.TotalLikes,
            TotalComments = metrics.TotalComments,
            TotalShares = metrics.TotalShares,
            EngagementRate = metrics.EngagementRate,
            EstimatedRevenue = metrics.EstimatedRevenue,
            TopPerformingContentIds = metrics.TopPerformingContentIds,
            CreatedAt = metrics.CreatedAt,
            UpdatedAt = metrics.UpdatedAt
        };
    }

    public static PerformanceMetrics ToDomain(this PerformanceMetricsEntity entity)
    {
        if (entity == null) return null;

        return new PerformanceMetrics
        {
            Id = Guid.Parse(entity.Id),
            CreatorId = Guid.Parse(entity.CreatorId),
            Platform = entity.Platform,
            Date = entity.Date,
            Followers = entity.Followers,
            FollowersGrowth = entity.FollowersGrowth,
            TotalViews = entity.TotalViews,
            TotalLikes = entity.TotalLikes,
            TotalComments = entity.TotalComments,
            TotalShares = entity.TotalShares,
            EngagementRate = entity.EngagementRate,
            EstimatedRevenue = entity.EstimatedRevenue,
            TopPerformingContentIds = entity.TopPerformingContentIds,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
} 