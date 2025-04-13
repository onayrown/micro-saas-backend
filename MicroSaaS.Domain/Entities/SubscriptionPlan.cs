using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MicroSaaS.Domain.Entities;

public class SubscriptionPlan
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("price")]
    public decimal Price { get; set; }

    [BsonElement("max_posts")]
    public int MaxPosts { get; set; }

    [BsonElement("is_free_plan")]
    public bool IsFreePlan { get; set; }

    public List<string> Features { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
