using MongoDB.Driver;
using MicroSaaS.Infrastructure.Entities;
using System;

namespace MicroSaaS.Infrastructure.Database
{
    public interface IMongoDbContext
    {
        IMongoCollection<UserEntity> Users { get; }
        IMongoCollection<ContentCreatorEntity> ContentCreators { get; }
        IMongoCollection<ContentPostEntity> ContentPosts { get; }
        IMongoCollection<SocialMediaAccountEntity> SocialMediaAccounts { get; }
        IMongoCollection<T> GetCollection<T>(string name) where T : class;
        IMongoDatabase Database { get; }
    }
} 