using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MicroSaaS.Infrastructure.Entities;

public class SubscriptionPlanEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public string? Id { get; set; }

    [BsonElement("name")]
    public string? Name { get; set; }

    [BsonElement("price")]
    public decimal Price { get; set; }

    [BsonElement("max_posts")]
    public int MaxPosts { get; set; }

    [BsonElement("is_free_plan")]
    public bool IsFreePlan { get; set; }

    [BsonElement("features")]
    public List<string> Features { get; set; } = new();

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }
} 