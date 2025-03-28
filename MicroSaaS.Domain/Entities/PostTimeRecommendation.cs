using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace MicroSaaS.Domain.Entities;

public class PostTimeRecommendation
{
    [BsonElement("dayOfWeek")]
    public DayOfWeek DayOfWeek { get; set; }

    [BsonElement("timeOfDay")]
    public TimeSpan TimeOfDay { get; set; }

    [BsonElement("engagementScore")]
    public double EngagementScore { get; set; }
} 