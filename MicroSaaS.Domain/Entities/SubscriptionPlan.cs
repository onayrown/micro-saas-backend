namespace MicroSaaS.Domain.Entities;

public class SubscriptionPlan
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public decimal Price { get; set; }
    public int MaxPosts { get; set; }
    public bool IsFreePlan { get; set; }
    public List<string> Features { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
