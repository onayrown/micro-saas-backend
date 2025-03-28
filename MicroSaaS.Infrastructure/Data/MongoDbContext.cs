using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MicroSaaS.Infrastructure.Persistence;
using MicroSaaS.Infrastructure.Entities;

namespace MicroSaaS.Infrastructure.Data;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> mongoDbSettings)
    {
        var mongoClient = new MongoClient(mongoDbSettings.Value.ConnectionString);
        _database = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
    }

    // Note: Estas coleções usam UserEntity e outras entidades da infraestrutura, não do domínio
    public IMongoCollection<UserEntity> Users
        => _database.GetCollection<UserEntity>("users");

    public IMongoCollection<ContentCreatorEntity> ContentCreators
        => _database.GetCollection<ContentCreatorEntity>("content_creators");

    public IMongoCollection<ContentPostEntity> ContentPosts
        => _database.GetCollection<ContentPostEntity>("content_posts");

    public IMongoCollection<SocialMediaAccountEntity> SocialMediaAccounts
        => _database.GetCollection<SocialMediaAccountEntity>("social_media_accounts");

    public IMongoCollection<ContentPerformanceEntity> ContentPerformances
        => _database.GetCollection<ContentPerformanceEntity>("content_performances");

    public IMongoCollection<PerformanceMetricsEntity> PerformanceMetrics
        => _database.GetCollection<PerformanceMetricsEntity>("performance_metrics");

    public IMongoCollection<DashboardInsightsEntity> DashboardInsights
        => _database.GetCollection<DashboardInsightsEntity>("dashboard_insights");

    // Método genérico para obter coleções
    public IMongoCollection<T> GetCollection<T>(string name)
    {
        return _database.GetCollection<T>(name);
    }
}