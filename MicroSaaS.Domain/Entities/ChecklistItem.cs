namespace MicroSaaS.Domain.Entities;

public class ChecklistItem
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsRequired { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Novos campos para prazos e lembretes
    public DateTime? DueDate { get; set; }
    public bool HasReminder { get; set; }
    public DateTime? ReminderDate { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
}

public enum TaskPriority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Urgent = 3
}
