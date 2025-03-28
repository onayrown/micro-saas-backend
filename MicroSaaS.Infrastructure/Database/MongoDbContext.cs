using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MicroSaaS.Infrastructure.Entities;
using MicroSaaS.Infrastructure.Settings;
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
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);

            _users = _database.GetCollection<UserEntity>("users");
            _contentCreators = _database.GetCollection<ContentCreatorEntity>("content_creators");
            _contentPosts = _database.GetCollection<ContentPostEntity>("content_posts");
            _socialMediaAccounts = _database.GetCollection<SocialMediaAccountEntity>("social_media_accounts");
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