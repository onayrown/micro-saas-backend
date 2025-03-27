using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using System;

namespace MicroSaaS.Infrastructure.Mappers;

public static class ContentCreatorMapper
{
    public static ContentCreatorEntity ToEntity(this ContentCreator creator)
    {
        if (creator == null) return null;

        return new ContentCreatorEntity
        {
            Id = creator.Id.ToString(),
            Name = creator.Name,
            Email = creator.Email,
            Username = creator.Username
        };
    }

    public static ContentCreator ToDomain(this ContentCreatorEntity entity)
    {
        if (entity == null) return null;

        return new ContentCreator
        {
            Id = string.IsNullOrEmpty(entity.Id) ? Guid.Empty : Guid.Parse(entity.Id),
            Name = entity.Name,
            Email = entity.Email,
            Username = entity.Username,
            SocialMediaAccounts = new List<SocialMediaAccount>(),
            SubscriptionPlan = null
        };
    }
} 