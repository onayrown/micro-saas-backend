using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Settings;
// using MicroSaaS.Infrastructure.MongoDB; // Remover auto-referência?
using System;

namespace MicroSaaS.Infrastructure.MongoDB // Namespace corrigido
{
    // Mover IMongoDbContext para cá também
    public interface IMongoDbContext
    {
        IMongoCollection<User> Users { get; }
        IMongoCollection<ContentCreator> ContentCreators { get; }
        IMongoCollection<ContentPost> ContentPosts { get; }
        IMongoCollection<SocialMediaAccount> SocialMediaAccounts { get; }
        IMongoCollection<ContentPerformance> PerformanceMetrics { get; }
        IMongoCollection<DashboardInsights> DashboardInsights { get; }
        IMongoCollection<ContentRecommendation> ContentRecommendations { get; }
        IMongoCollection<ContentChecklist> ContentChecklists { get; }
        IMongoCollection<DailyRevenue> DailyRevenues { get; }
        IMongoCollection<T> GetCollection<T>(string name) where T : class;
        IMongoDatabase Database { get; }
        IMongoDatabase GetDatabase();
    }

    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<ContentCreator> _contentCreators;
        private readonly IMongoCollection<ContentPost> _contentPosts;
        private readonly IMongoCollection<SocialMediaAccount> _socialMediaAccounts;
        private readonly IMongoCollection<ContentPerformance> _performanceMetrics;
        private readonly IMongoCollection<DashboardInsights> _dashboardInsights;
        private readonly IMongoCollection<ContentRecommendation> _contentRecommendations;
        private readonly IMongoCollection<ContentChecklist> _contentChecklists;
        private readonly IMongoCollection<DailyRevenue> _dailyRevenues;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            MongoDbInitializer.Initialize();
            
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);

            _users = _database.GetCollection<User>(CollectionNames.Users);
            _contentCreators = _database.GetCollection<ContentCreator>(CollectionNames.ContentCreators);
            _contentPosts = _database.GetCollection<ContentPost>(CollectionNames.ContentPosts);
            _socialMediaAccounts = _database.GetCollection<SocialMediaAccount>(CollectionNames.SocialMediaAccounts);
            _performanceMetrics = _database.GetCollection<ContentPerformance>(CollectionNames.ContentPerformances);
            _dashboardInsights = _database.GetCollection<DashboardInsights>(CollectionNames.DashboardInsights);
            _contentRecommendations = _database.GetCollection<ContentRecommendation>(CollectionNames.ContentRecommendations);
            _contentChecklists = _database.GetCollection<ContentChecklist>(CollectionNames.ContentChecklists);
            _dailyRevenues = _database.GetCollection<DailyRevenue>(nameof(DailyRevenue) + "s");
        }

        public IMongoDatabase Database => _database;

        public IMongoCollection<User> Users => _users;
        public IMongoCollection<ContentCreator> ContentCreators => _contentCreators;
        public IMongoCollection<ContentPost> ContentPosts => _contentPosts;
        public IMongoCollection<SocialMediaAccount> SocialMediaAccounts => _socialMediaAccounts;
        public IMongoCollection<ContentPerformance> PerformanceMetrics => _performanceMetrics;
        public IMongoCollection<DashboardInsights> DashboardInsights => _dashboardInsights;
        public IMongoCollection<ContentRecommendation> ContentRecommendations => _contentRecommendations;
        public IMongoCollection<ContentChecklist> ContentChecklists => _contentChecklists;
        public IMongoCollection<DailyRevenue> DailyRevenues => _dailyRevenues;

        public IMongoCollection<T> GetCollection<T>(string name) where T : class
        {
            return _database.GetCollection<T>(name);
        }
        
        public IMongoDatabase GetDatabase()
        {
            return _database;
        }
    }
} 