using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MicroSaaS.Infrastructure.Mappers;

public static class DashboardInsightsMapper
{
    public static DashboardInsights ToDomain(DashboardInsightsEntity entity)
    {
        if (entity == null) return null;

        return new DashboardInsights
        {
            Id = entity.Id,
            CreatorId = entity.CreatorId,
            Date = entity.Date,
            Insights = entity.Insights.Select(ContentInsightMapper.ToDomain).ToList(),
            Recommendations = entity.Recommendations.Select(ContentRecommendationMapper.ToDomain).ToList(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static DashboardInsightsEntity ToEntity(DashboardInsights domain)
    {
        if (domain == null) return null;

        return new DashboardInsightsEntity
        {
            Id = domain.Id,
            CreatorId = domain.CreatorId,
            Date = domain.Date,
            Insights = domain.Insights.Select(ContentInsightMapper.ToEntity).ToList(),
            Recommendations = domain.Recommendations.Select(ContentRecommendationMapper.ToEntity).ToList(),
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
} 