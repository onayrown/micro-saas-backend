using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using System;

namespace MicroSaaS.Infrastructure.Mappers;

public static class SocialMediaAccountMapper
{
    public static SocialMediaAccount? ToDomain(SocialMediaAccountEntity? entity)
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
            IsActive = entity.IsActive,
            FollowersCount = entity.FollowersCount,
            ProfileUrl = entity.ProfileUrl,
            ProfileImageUrl = entity.ProfileImageUrl,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static SocialMediaAccountEntity? ToEntity(SocialMediaAccount? domain)
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
            IsActive = domain.IsActive,
            FollowersCount = domain.FollowersCount,
            ProfileUrl = domain.ProfileUrl,
            ProfileImageUrl = domain.ProfileImageUrl,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }
} 