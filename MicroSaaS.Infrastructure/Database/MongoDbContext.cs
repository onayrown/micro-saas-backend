using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MicroSaaS.Infrastructure.Entities;
using MicroSaaS.Infrastructure.Settings;
using MicroSaaS.Infrastructure.MongoDB;
using System;

namespace MicroSaaS.Infrastructure.Database
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<UserEntity> _users;
        private readonly IMongoCollection<ContentCreatorEntity> _contentCreators;
        private readonly IMongoCollection<ContentPostEntity> _contentPosts;
        private readonly IMongoCollection<SocialMediaAccountEntity> _socialMediaAccounts;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            // Garantir que os serializadores estejam registrados antes de usar o MongoDB
            MongoDbInitializer.Initialize();
            
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);

            _users = _database.GetCollection<UserEntity>(CollectionNames.Users);
            _contentCreators = _database.GetCollection<ContentCreatorEntity>(CollectionNames.ContentCreators);
            _contentPosts = _database.GetCollection<ContentPostEntity>(CollectionNames.ContentPosts);
            _socialMediaAccounts = _database.GetCollection<SocialMediaAccountEntity>(CollectionNames.SocialMediaAccounts);
        }

        public IMongoDatabase Database => _database;

        public IMongoCollection<UserEntity> Users => _users;
        public IMongoCollection<ContentCreatorEntity> ContentCreators => _contentCreators;
        public IMongoCollection<ContentPostEntity> ContentPosts => _contentPosts;
        public IMongoCollection<SocialMediaAccountEntity> SocialMediaAccounts => _socialMediaAccounts;

        public IMongoCollection<T> GetCollection<T>(string name) where T : class
        {
            return _database.GetCollection<T>(name);
        }
    }
} 