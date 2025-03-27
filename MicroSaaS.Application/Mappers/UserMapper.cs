using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Domain.Entities;

namespace MicroSaaS.Application.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            IsActive = user.IsActive,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static User ToEntity(RegisterRequest request)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // DTO para Domínio
    public static User? ToDomain(this UserDto userDto)
    {
        if (userDto == null) return null;

        return new User
        {
            Id = userDto.Id,
            Name = userDto.Name,
            Email = userDto.Email,
            CreatedAt = userDto.CreatedAt,
            LastLoginAt = userDto.LastLoginAt,
            IsActive = userDto.IsActive
        };
    }
}


