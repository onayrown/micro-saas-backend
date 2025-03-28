using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MicroSaaS.Infrastructure.Entities;

public class ContentChecklistEntity
{
    public ContentChecklistEntity()
    {
        Id = ObjectId.GenerateNewId().ToString();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Items = new List<ChecklistItemEntity>();
        Title = string.Empty;
        Description = string.Empty;
        CreatorId = string.Empty;
    }

    public ContentChecklistEntity(string creatorId, string title, string description)
    {
        Id = ObjectId.GenerateNewId().ToString();
        CreatorId = creatorId;
        Title = title;
        Description = description;
        Status = ChecklistStatus.NotStarted;
        Items = new List<ChecklistItemEntity>();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("creator_id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string CreatorId { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("status")]
    public ChecklistStatus Status { get; set; }

    [BsonElement("items")]
    public List<ChecklistItemEntity> Items { get; set; } = new();

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("completed_at")]
    public DateTime? CompletedAt { get; set; }
}

public class ChecklistItemEntity
{
    public ChecklistItemEntity()
    {
        Id = ObjectId.GenerateNewId().ToString();
        Title = string.Empty;
        Description = string.Empty;
    }

    public ChecklistItemEntity(string title, string description)
    {
        Id = ObjectId.GenerateNewId().ToString();
        Title = title;
        Description = description;
        IsCompleted = false;
    }

    [BsonElement("id")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("is_completed")]
    public bool IsCompleted { get; set; }

    [BsonElement("completed_at")]
    public DateTime? CompletedAt { get; set; }
} 