using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MicroSaaS.Shared.Enums;
using System;

namespace MicroSaaS.Domain.Entities;

public class ChecklistItem
{
    public ChecklistItem()
    {
        Id = Guid.NewGuid();
        Title = string.Empty;
        Description = string.Empty;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public ChecklistItem(string title, string description)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        IsCompleted = false;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    [BsonElement("id")]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("is_completed")]
    public bool IsCompleted { get; set; }

    [BsonElement("completed_at")]
    public DateTime? CompletedAt { get; set; }

    public bool IsRequired { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }

    // Novos campos para prazos e lembretes
    public DateTime? DueDate { get; set; }
    public bool HasReminder { get; set; }
    public DateTime? ReminderDate { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
}


