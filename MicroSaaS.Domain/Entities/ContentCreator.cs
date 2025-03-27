using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Domain.Entities;

public class ContentCreator
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("user_id")]
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    [BsonElement("name")]
    public required string Name { get; set; }

    [BsonElement("bio")]
    public required string Bio { get; set; }

    [BsonElement("niche")]
    public required string Niche { get; set; }

    [BsonElement("content_type")]
    public ContentType ContentType { get; set; }

    [BsonElement("email")]
    public required string Email { get; set; }

    [BsonElement("username")]
    public required string Username { get; set; }

    [BsonElement("subscription_plan")]
    public required SubscriptionPlan SubscriptionPlan { get; set; }

    [BsonElement("social_media_accounts")]
    public List<SocialMediaAccount> SocialMediaAccounts { get; set; } = new();

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
