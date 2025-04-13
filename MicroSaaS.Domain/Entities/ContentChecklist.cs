using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MicroSaaS.Domain.Entities;

public class ContentChecklist
{
    public ContentChecklist()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        Items = new List<ChecklistItem>();
        Title = string.Empty;
        Description = string.Empty;
        CreatorId = Guid.Empty;
    }

    public ContentChecklist(Guid creatorId, string title, string description)
    {
        Id = Guid.NewGuid();
        CreatorId = creatorId;
        Title = title;
        Description = description;
        Status = ChecklistStatus.NotStarted;
        Items = new List<ChecklistItem>();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("creator_id")]
    [BsonRepresentation(BsonType.String)]
    public Guid CreatorId { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("status")]
    public ChecklistStatus Status { get; set; }

    [BsonElement("items")]
    public List<ChecklistItem> Items { get; set; } = new();

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("completed_at")]
    public DateTime? CompletedAt { get; set; }
}
