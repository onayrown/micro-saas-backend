using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Domain.Entities;

public class ContentChecklist
{
    public Guid Id { get; set; }
    public Guid CreatorId { get; set; }
    public required ContentCreator Creator { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required List<ChecklistItem> Items { get; set; } = new();
    public ChecklistStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
