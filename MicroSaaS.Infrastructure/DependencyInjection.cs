using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DomainRepos = MicroSaaS.Domain.Repositories;
using AppRepos = MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Infrastructure.Database;
using MicroSaaS.Infrastructure.Repositories;
using MicroSaaS.Infrastructure.Settings;
using MongoDB.Driver;

namespace MicroSaaS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure MongoDB
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
            services.AddSingleton<IMongoDbContext, MongoDbContext>();

            // Register repositories
            services.AddScoped<DomainRepos.IUserRepository, UserRepository>();
            services.AddScoped<DomainRepos.IContentCreatorRepository, ContentCreatorRepository>();
            services.AddScoped<DomainRepos.IContentPostRepository, ContentPostRepository>();
            services.AddScoped<DomainRepos.ISocialMediaAccountRepository, SocialMediaAccountRepository>();
            services.AddScoped<AppRepos.IContentPerformanceRepository, ContentPerformanceRepository>();
            services.AddScoped<AppRepos.IPerformanceMetricsRepository, PerformanceMetricsRepository>();
            services.AddScoped<AppRepos.IDashboardInsightsRepository, DashboardInsightsRepository>();

            return services;
        }
    }
} 