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
        
        [BsonElement("platform")]
        public SocialMediaPlatform Platform { get; set; }

        [BsonElement("access_token")]
        public string? AccessToken { get; set; }

        [BsonElement("refresh_token")]
        public string? RefreshToken { get; set; }

        [BsonElement("token_expiry")]
        public DateTime? TokenExpiry { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
