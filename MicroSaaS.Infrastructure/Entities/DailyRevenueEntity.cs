using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MicroSaaS.Infrastructure.Entities;

public class DailyRevenueEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("creatorId")]
    [BsonRepresentation(BsonType.String)]
    public Guid CreatorId { get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; }

    [BsonElement("amount")]
    public decimal Amount { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
} 