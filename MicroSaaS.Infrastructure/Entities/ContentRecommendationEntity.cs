using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;

namespace MicroSaaS.Infrastructure.Entities;

public class ContentRecommendationEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("creatorId")]
    [BsonRepresentation(BsonType.String)]
    public Guid CreatorId { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("type")]
    public RecommendationType Type { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("recommendationType")]
    public RecommendationType RecommendationType { get; set; }

    [BsonElement("suggestedTopics")]
    public List<string> SuggestedTopics { get; set; } = new();

    [BsonElement("platform")]
    public SocialMediaPlatform Platform { get; set; }

    [BsonElement("priority")]
    public int Priority { get; set; }

    [BsonElement("potentialImpact")]
    public string PotentialImpact { get; set; } = string.Empty;
} 