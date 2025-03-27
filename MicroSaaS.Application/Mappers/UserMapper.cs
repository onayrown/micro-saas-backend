using MicroSaaS.Application.DTOs;
using MicroSaaS.Domain.Entities;

namespace MicroSaaS.Application.Mappers;

public static class UserMapper
{
    // Domínio para DTO
    public static UserDto ToDto(this User user)
    {
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive
        };
    }

    // DTO para Domínio
    public static User ToDomain(this UserDto userDto)
    {
        if (userDto == null) return null;

        return new User
        {
            Id = userDto.Id,
            Username = userDto.Username,
            Email = userDto.Email,
            PasswordHash = userDto.PasswordHash,
            CreatedAt = userDto.CreatedAt,
            LastLoginAt = userDto.LastLoginAt,
            IsActive = userDto.IsActive
        };
    }
}


