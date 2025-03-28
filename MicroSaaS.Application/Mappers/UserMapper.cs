using MicroSaaS.Application.DTOs.Auth;
using MicroSaaS.Domain.Entities;
using System;

namespace MicroSaaS.Application.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static User ToEntity(RegisterRequest request)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Username = request.Name,
            Email = request.Email,
            PasswordHash = string.Empty, // Será preenchido pelo serviço
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
            Username = userDto.Username,
            Email = userDto.Email,
            PasswordHash = string.Empty, // Será preenchido pelo serviço
            IsActive = userDto.IsActive,
            CreatedAt = userDto.CreatedAt,
            UpdatedAt = userDto.UpdatedAt
        };
    }
}


