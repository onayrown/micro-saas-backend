using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using System;

namespace MicroSaaS.Infrastructure.Mappers;

public static class SocialMediaAccountMapper
{
    public static SocialMediaAccount ToDomain(SocialMediaAccountEntity entity)
    {
        if (entity == null) return null;

        return new SocialMediaAccount
        {
            Id = entity.Id,
            CreatorId = entity.CreatorId,
            Platform = entity.Platform,
            Username = entity.Username,
            AccessToken = entity.AccessToken,
            RefreshToken = entity.RefreshToken,
            TokenExpiresAt = entity.TokenExpiresAt,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static SocialMediaAccountEntity ToEntity(SocialMediaAccount domain)
    {
        if (domain == null) return null;

        return new SocialMediaAccountEntity
        {
            Id = domain.Id,
            CreatorId = domain.CreatorId,
            Platform = domain.Platform,
            Username = domain.Username,
            AccessToken = domain.AccessToken,
            RefreshToken = domain.RefreshToken,
            TokenExpiresAt = domain.TokenExpiresAt,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
} 