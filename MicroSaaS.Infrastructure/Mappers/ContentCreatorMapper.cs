using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MicroSaaS.Infrastructure.Mappers;

public static class ContentCreatorMapper
{
    public static ContentCreator? ToDomain(ContentCreatorEntity? entity)
    {
        if (entity == null) return null;

        return new ContentCreator
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Name = entity.Name,
            Email = entity.Email,
            Username = entity.Username,
            Bio = entity.Bio,
            ProfileImageUrl = entity.ProfileImageUrl,
            WebsiteUrl = entity.WebsiteUrl,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static ContentCreatorEntity? ToEntity(ContentCreator? domain)
    {
        if (domain == null) return null;

        return new ContentCreatorEntity
        {
            Id = domain.Id,
            UserId = domain.UserId,
            Name = domain.Name,
            Email = domain.Email,
            Username = domain.Username,
            Bio = domain.Bio,
            ProfileImageUrl = domain.ProfileImageUrl,
            WebsiteUrl = domain.WebsiteUrl,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
}

public static class SubscriptionPlanMapper
{
    public static SubscriptionPlan? ToDomain(SubscriptionPlanEntity? entity)
    {
        if (entity == null) return null;

        return new SubscriptionPlan
        {
            Id = Guid.Parse(entity.Id),
            Name = entity.Name,
            Price = entity.Price,
            MaxPosts = entity.MaxPosts,
            IsFreePlan = entity.IsFreePlan
        };
    }

    public static SubscriptionPlanEntity? ToEntity(SubscriptionPlan? domain)
    {
        if (domain == null) return null;

        return new SubscriptionPlanEntity
        {
            Id = domain.Id.ToString(),
            Name = domain.Name,
            Price = domain.Price,
            MaxPosts = domain.MaxPosts,
            IsFreePlan = domain.IsFreePlan
        };
    }
} 