using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Infrastructure.MongoDB;
using MicroSaaS.Infrastructure.Persistence.Repositories;
using MicroSaaS.Infrastructure.Services;
using MicroSaaS.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MicroSaaS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuração do MongoDB - corrigido para usar a seção "MongoDB" do appsettings.json
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDB"));
            
            // Registrar MongoDbSettings como Singleton usando IOptions
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);
            
            // Registrar MongoClient
            services.AddSingleton<IMongoClient>(sp => {
                var settings = sp.GetRequiredService<MongoDbSettings>();
                return new MongoClient(settings.ConnectionString);
            });
            
            // Registrar IMongoDbContext
            services.AddSingleton<IMongoDbContext, MongoDbContext>();

            // Repositórios (Namespace: Persistence.Repositories)
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IContentCreatorRepository, ContentCreatorRepository>();
            services.AddScoped<IMediaRepository, MediaRepository>();
            services.AddScoped<IContentPerformanceRepository, ContentPerformanceRepository>();
            services.AddScoped<IContentChecklistRepository, ContentChecklistRepository>();
            services.AddScoped<IContentPostRepository, ContentPostRepository>();
            services.AddScoped<ISocialMediaAccountRepository, SocialMediaAccountRepository>();
            services.AddScoped<IDashboardInsightsRepository, DashboardInsightsRepository>();
            services.AddScoped<IPerformanceMetricsRepository, PerformanceMetricsRepository>();

            // Serviços (Namespace: Services)
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<MicroSaaS.Application.Interfaces.Services.ITokenService, TokenService>();
            services.AddScoped<IStorageService, LocalStorageService>();
            services.AddScoped<ILoggingService, SerilogService>();
            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<MicroSaaS.Application.Interfaces.Services.IRevenueService, RevenueService>();
            services.AddScoped<MicroSaaS.Application.Interfaces.Services.ISocialMediaIntegrationService, SocialMediaIntegrationService>();
            services.AddSingleton<ICacheService, RedisCacheService>();

            // Removi os registros comentados para IDashboardInsightsService e IPerformanceAnalysisService
            // pois suas implementações estão na camada de Application

            // Configuração de HttpClient para serviços externos
            services.AddHttpClient<MicroSaaS.Application.Interfaces.Services.IRevenueService, RevenueService>();

            // Registrar serviços
            // Nota: IContentAnalysisService, IContentPlanningService, IRecommendationService, ISchedulerService, IDashboardService
            // foram migrados para MicroSaaS.Application
            
            // Os seguintes serviços estão referenciados, mas parecem não ter implementações correspondentes no projeto
            // Comentando temporariamente para corrigir erros de compilação
            // TODO: Implementar estes serviços ou removê-los permanentemente do registro de DI
            // services.AddScoped<IAuthService, AuthService>();
            // services.AddScoped<IUserService, UserService>();
            // services.AddScoped<ISocialMediaIntegrationService, SocialMediaIntegrationService>();
            // services.AddScoped<IEmailService, EmailService>();
            // services.AddScoped<IStorageService, AzureBlobStorageService>();
            // services.AddScoped<INotificationService, NotificationService>();
            // services.AddScoped<IPaymentProcessingService, StripePaymentService>();
            // services.AddScoped<ISubscriptionService, SubscriptionService>();

            return services;
        }
    }
} 