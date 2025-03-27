using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using System;

namespace MicroSaaS.Infrastructure.Mappers;

public static class SocialMediaAccountMapper
{
    public static SocialMediaAccountEntity? ToEntity(this SocialMediaAccount account)
    {
        if (account == null) return null;

        return new SocialMediaAccountEntity
        {
            Id = account.Id.ToString(),
            Platform = account.Platform,
            AccessToken = account.AccessToken,
            RefreshToken = account.RefreshToken,
            TokenExpiry = account.TokenExpiresAt,
            CreatedAt = account.CreatedAt,
            UpdatedAt = account.UpdatedAt
        };
    }

    public static SocialMediaAccount? ToDomain(this SocialMediaAccountEntity entity)
    {
        if (entity == null) return null;

        return new SocialMediaAccount
        {
            Id = string.IsNullOrEmpty(entity.Id) ? Guid.Empty : Guid.Parse(entity.Id),
            Platform = entity.Platform,
            AccessToken = entity.AccessToken,
            RefreshToken = entity.RefreshToken,
            TokenExpiresAt = entity.TokenExpiry,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
} 