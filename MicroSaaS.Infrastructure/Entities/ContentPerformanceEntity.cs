using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MicroSaaS.Shared.Enums;
using System;

namespace MicroSaaS.Infrastructure.Entities;

public class ContentPerformanceEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }
    
    [BsonElement("postId")]
    [BsonRepresentation(BsonType.String)]
    public Guid PostId { get; set; }
    
    [BsonElement("creatorId")]
    [BsonRepresentation(BsonType.String)]
    public Guid CreatorId { get; set; }
    
    [BsonElement("platform")]
    public SocialMediaPlatform Platform { get; set; }
    
    [BsonElement("date")]
    public DateTime Date { get; set; }
    
    [BsonElement("collectedAt")]
    public DateTime CollectedAt { get; set; }
    
    [BsonElement("views")]
    public long Views { get; set; }
    
    [BsonElement("likes")]
    public long Likes { get; set; }
    
    [BsonElement("comments")]
    public long Comments { get; set; }
    
    [BsonElement("shares")]
    public long Shares { get; set; }
    
    [BsonElement("engagementRate")]
    public decimal EngagementRate { get; set; }
    
    [BsonElement("estimatedRevenue")]
    public decimal EstimatedRevenue { get; set; }
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
} 