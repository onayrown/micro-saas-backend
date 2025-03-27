using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using System;
using System.Linq;

namespace MicroSaaS.Infrastructure.Mappers;

public static class ContentCreatorMapper
{
    public static ContentCreatorEntity? ToEntity(this ContentCreator creator)
    {
        if (creator == null) return null;

        return new ContentCreatorEntity
        {
            Id = creator.Id.ToString(),
            Name = creator.Name,
            Email = creator.Email,
            Username = creator.Username,
            Bio = creator.Bio,
            Niche = creator.Niche,
            ContentType = creator.ContentType,
            SubscriptionPlan = creator.SubscriptionPlan?.ToEntity(),
            SocialMediaAccounts = creator.SocialMediaAccounts?.Select(a => a.ToEntity()).ToList() ?? new List<SocialMediaAccountEntity>(),
            CreatedAt = creator.CreatedAt,
            UpdatedAt = creator.UpdatedAt
        };
    }

    public static ContentCreator? ToDomain(this ContentCreatorEntity entity)
    {
        if (entity == null) return null;

        return new ContentCreator
        {
            Id = string.IsNullOrEmpty(entity.Id) ? Guid.Empty : Guid.Parse(entity.Id),
            Name = entity.Name,
            Email = entity.Email,
            Username = entity.Username,
            Bio = entity.Bio,
            Niche = entity.Niche,
            ContentType = entity.ContentType,
            SubscriptionPlan = entity.SubscriptionPlan?.ToDomain(),
            SocialMediaAccounts = entity.SocialMediaAccounts?.Select(a => a.ToDomain()).ToList() ?? new List<SocialMediaAccount>(),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
} 