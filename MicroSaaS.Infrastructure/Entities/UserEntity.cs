using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MicroSaaS.Infrastructure.Entities;

public class UserEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string? Id { get; set; }

    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; }
}
