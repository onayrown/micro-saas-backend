using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DomainRepos = MicroSaaS.Domain.Repositories;
using AppRepos = MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Infrastructure.Database;
using MicroSaaS.Infrastructure.Repositories;
using MicroSaaS.Infrastructure.Settings;
using MongoDB.Driver;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Infrastructure.Services;

namespace MicroSaaS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure MongoDB
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDb"));
            
            // Registrar o contexto do MongoDB usando a implementação Database
            services.AddSingleton<IMongoDbContext, MongoDbContext>();

            // Register repositories
            services.AddScoped<DomainRepos.IUserRepository, UserRepository>();
            services.AddScoped<DomainRepos.IContentCreatorRepository, ContentCreatorRepository>();
            services.AddScoped<DomainRepos.IContentPostRepository, ContentPostRepository>();
            services.AddScoped<DomainRepos.ISocialMediaAccountRepository, SocialMediaAccountRepository>();
            services.AddScoped<AppRepos.IContentPerformanceRepository, ContentPerformanceRepository>();
            services.AddScoped<AppRepos.IPerformanceMetricsRepository, PerformanceMetricsRepository>();
            services.AddScoped<AppRepos.IDashboardInsightsRepository, DashboardInsightsRepository>();
            
            // Adicionar o serviço de análise de conteúdo
            services.AddScoped<IContentAnalysisService, ContentAnalysisService>();
            
            // Adicionar o serviço de recomendação
            services.AddScoped<IRecommendationService, RecommendationService>();
            
            // Adicionar serviço de logging
            services.AddScoped<ILoggingService, SerilogService>();
            
            // Adicionar serviços relacionados à otimização de desempenho
            services.AddSingleton<ICacheService, RedisCacheService>();
            
            // Configurar Redis para caching
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
                options.InstanceName = "MicroSaaS:";
            });

            return services;
        }
    }
} 