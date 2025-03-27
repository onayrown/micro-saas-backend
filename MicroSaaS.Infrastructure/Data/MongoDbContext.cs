using MicroSaaS.Infrastructure.Configuration;
using MicroSaaS.Infrastructure.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MicroSaaS.Infrastructure.Data
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<UserEntity> Users
            => _database.GetCollection<UserEntity>("users");

        public IMongoCollection<ContentCreatorEntity> ContentCreators
            => _database.GetCollection<ContentCreatorEntity>("content_creators");

        public IMongoCollection<ContentPostEntity> ContentPosts
            => _database.GetCollection<ContentPostEntity>("content_posts");

        public IMongoCollection<SocialMediaAccountEntity> SocialMediaAccounts
            => _database.GetCollection<SocialMediaAccountEntity>("social_media_accounts");

        // Método genérico para obter coleções
        public IMongoCollection<T> GetCollection<T>(string name)
        {
            return _database.GetCollection<T>(name);
        }
    }
}