namespace MicroSaaS.Domain.Entities;

public class ChecklistItem
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}
