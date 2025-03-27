using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using System;

namespace MicroSaaS.Infrastructure.Mappers;

public static class UserMapper
{
    public static UserEntity ToEntity(this User user)
    {
        if (user == null) return null;

        return new UserEntity
        {
            Id = user.Id.ToString(),
            Name = user.Name,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive
        };
    }

    public static User ToDomain(this UserEntity entity)
    {
        if (entity == null) return null;

        return new User
        {
            Id = Guid.Parse(entity.Id),
            Name = entity.Name,
            Email = entity.Email,
            PasswordHash = entity.PasswordHash,
            CreatedAt = entity.CreatedAt,
            LastLoginAt = entity.LastLoginAt,
            IsActive = entity.IsActive
        };
    }
} 