using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;

namespace MicroSaaS.Infrastructure.Mappers;

public static class ContentInsightMapper
{
    public static ContentInsight ToDomain(ContentInsightEntity entity)
    {
        if (entity == null) return null;

        return new ContentInsight
        {
            Type = entity.Type,
            Title = entity.Title,
            Description = entity.Description,
            Metrics = entity.Metrics,
            Trend = entity.Trend,
            ComparisonPeriod = entity.ComparisonPeriod,
            PercentageChange = entity.PercentageChange
        };
    }

    public static ContentInsightEntity ToEntity(ContentInsight domain)
    {
        if (domain == null) return null;

        return new ContentInsightEntity
        {
            Type = domain.Type,
            Title = domain.Title,
            Description = domain.Description,
            Metrics = domain.Metrics,
            Trend = domain.Trend,
            ComparisonPeriod = domain.ComparisonPeriod,
            PercentageChange = domain.PercentageChange
        };
    }
} 