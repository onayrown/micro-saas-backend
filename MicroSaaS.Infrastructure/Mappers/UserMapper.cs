using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using System;

namespace MicroSaaS.Infrastructure.Mappers;

public static class UserMapper
{
    public static UserEntity? ToEntity(this User? user)
    {
        if (user == null) return null;

        return new UserEntity
        {
            Id = user.Id.ToString(),
            Username = user.Username,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static User? ToDomain(this UserEntity? entity)
    {
        if (entity == null) return null;

        return new User
        {
            Id = Guid.Parse(entity.Id),
            Username = entity.Username,
            Email = entity.Email,
            PasswordHash = entity.PasswordHash,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
} 