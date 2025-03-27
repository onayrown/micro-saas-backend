using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using System;

namespace MicroSaaS.Infrastructure.Mappers;

public static class SubscriptionPlanMapper
{
    public static SubscriptionPlanEntity? ToEntity(this SubscriptionPlan plan)
    {
        if (plan == null) return null;

        return new SubscriptionPlanEntity
        {
            Id = plan.Id.ToString(),
            Name = plan.Name,
            Price = plan.Price,
            MaxPosts = plan.MaxPosts,
            IsFreePlan = plan.IsFreePlan,
            Features = plan.Features,
            CreatedAt = plan.CreatedAt,
            UpdatedAt = plan.UpdatedAt
        };
    }

    public static SubscriptionPlan? ToDomain(this SubscriptionPlanEntity entity)
    {
        if (entity == null) return null;

        return new SubscriptionPlan
        {
            Id = string.IsNullOrEmpty(entity.Id) ? Guid.Empty : Guid.Parse(entity.Id),
            Name = entity.Name,
            Price = entity.Price,
            MaxPosts = entity.MaxPosts,
            IsFreePlan = entity.IsFreePlan,
            Features = entity.Features,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
} 