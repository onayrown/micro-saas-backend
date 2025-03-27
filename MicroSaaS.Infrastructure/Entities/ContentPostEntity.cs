using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Infrastructure.Entities
{
    public class ContentPostEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string? Id { get; set; }
        
        [BsonRepresentation(BsonType.String)]
        public string? CreatorId { get; set; }
        
        public SocialMediaPlatform Platform { get; set; }
        public string? Content { get; set; }
        public DateTime ScheduledTime { get; set; }
        public DateTime? PostedTime { get; set; }
        public PostStatus Status { get; set; }
    }
}
