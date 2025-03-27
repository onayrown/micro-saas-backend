using MicroSaaS.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MicroSaaS.Infrastructure.Persistence;

namespace MicroSaaS.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        _database = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users
        => _database.GetCollection<User>("users");

    public IMongoCollection<ContentCreator> ContentCreators
        => _database.GetCollection<ContentCreator>("content_creators");

    public IMongoCollection<ContentPost> ContentPosts
        => _database.GetCollection<ContentPost>("content_posts");

    public IMongoCollection<SocialMediaAccount> SocialMediaAccounts
        => _database.GetCollection<SocialMediaAccount>("social_media_accounts");

    // Método genérico para obter coleções
    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
}