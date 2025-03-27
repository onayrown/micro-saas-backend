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
    public class SocialMediaAccountEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public string? Id { get; set; }
        
        public SocialMediaPlatform Platform { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
