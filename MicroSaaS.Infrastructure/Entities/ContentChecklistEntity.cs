using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MicroSaaS.Infrastructure.Entities;

public class ContentChecklistEntity
{
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