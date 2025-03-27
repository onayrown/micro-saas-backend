namespace MicroSaaS.Domain.Entities;

public class ContentChecklist
{
    public Guid Id { get; set; }
    public ContentCreator Creator { get; set; }
    public string Title { get; set; }
    public List<ChecklistItem> Items { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
