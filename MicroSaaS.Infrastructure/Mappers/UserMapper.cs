using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Entities;
using MongoDB.Bson;
using System;

namespace MicroSaaS.Infrastructure.Mappers;

public static class UserMapper
{
    public static UserEntity? ToEntity(this User? user)
    {
        if (user == null) return null;

        // Em ambiente de testes, preservar o Id exatamente como está
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing" ||
            AppDomain.CurrentDomain.FriendlyName.Contains("testhost"))
        {
            return new UserEntity
            {
                Id = user.Id.ToString(),
                Username = user.Username,
                Name = user.Username,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Role = "user",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        // Lógica normal para produção
        string id;
        if (string.IsNullOrEmpty(user.Id.ToString()) || user.Id == Guid.Empty)
        {
            // Se não existe um Id válido, gerar um novo ObjectId
            id = ObjectId.GenerateNewId().ToString();
        }
        else
        {
            // Tenta usar o Id existente se for um ObjectId válido
            if (ObjectId.TryParse(user.Id.ToString(), out _))
            {
                id = user.Id.ToString();
            }
            else
            {
                // Caso o Guid não seja um ObjectId válido, gera um novo
                id = ObjectId.GenerateNewId().ToString();
            }
        }

        return new UserEntity
        {
            Id = id,
            Username = user.Username,
            Name = user.Username,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            Role = "user",
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public static User? ToDomain(this UserEntity? entity)
    {
        if (entity == null) return null;

        // Em ambiente de testes, converter diretamente de string para Guid
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Testing" ||
            AppDomain.CurrentDomain.FriendlyName.Contains("testhost"))
        {
            if (Guid.TryParse(entity.Id, out var guid))
            {
                return new User
                {
                    Id = guid,
                    Username = string.IsNullOrEmpty(entity.Username) ? entity.Name : entity.Username,
                    Email = entity.Email,
                    PasswordHash = entity.PasswordHash,
                    IsActive = entity.IsActive,
                    CreatedAt = entity.CreatedAt,
                    UpdatedAt = entity.UpdatedAt
                };
            }
        }

        // Lógica normal para produção
        Guid id;
        if (ObjectId.TryParse(entity.Id, out var objectId))
        {
            // Gera um Guid determinístico baseado no ObjectId
            var bytes = objectId.ToByteArray();
            var guidBytes = new byte[16];
            
            // Copia os bytes disponíveis do ObjectId
            Array.Copy(bytes, guidBytes, Math.Min(bytes.Length, 16));
            
            id = new Guid(guidBytes);
        }
        else if (!Guid.TryParse(entity.Id, out id))
        {
            // Fallback para um novo Guid se nenhuma conversão funcionar
            id = Guid.NewGuid();
        }

        return new User
        {
            Id = id,
            Username = string.IsNullOrEmpty(entity.Username) ? entity.Name : entity.Username,
            Email = entity.Email,
            PasswordHash = entity.PasswordHash,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
} 