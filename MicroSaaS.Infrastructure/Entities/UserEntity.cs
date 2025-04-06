using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MicroSaaS.Infrastructure.Entities;

[BsonIgnoreExtraElements] // Ignora campos extras no documento que não estão mapeados na classe
public class UserEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)] // Usa ObjectId para compatibilidade com documentos existentes
    public string Id { get; set; } = string.Empty;

    [BsonElement("username")]
    public string Username { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;
    
    [BsonElement("role")]
    public string Role { get; set; } = "user";

    [BsonElement("active")]
    public bool IsActive { get; set; } = true;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
