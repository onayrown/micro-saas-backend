namespace MicroSaaS.Domain.Entities;

public class SubscriptionPlan
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int MaxPosts { get; set; }
    public bool IsFreePlan { get; set; }
}
