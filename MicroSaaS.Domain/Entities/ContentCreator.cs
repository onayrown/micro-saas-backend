namespace MicroSaaS.Domain.Entities;

public class ContentCreator
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public List<SocialMediaAccount> SocialMediaAccounts { get; set; }
    public SubscriptionPlan SubscriptionPlan { get; set; }
}
