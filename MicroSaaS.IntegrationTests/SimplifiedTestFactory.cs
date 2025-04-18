using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MicroSaaS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;
using MicroSaaS.IntegrationTests.Controllers;
using MicroSaaS.IntegrationTests.Utils;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using MicroSaaS.IntegrationTests.Mocks;

namespace MicroSaaS.IntegrationTests
{
    /// <summary>
    /// Fábrica de teste extremamente simplificada que evita problemas com o versionamento da API
    /// </summary>
    public class SimplifiedTestFactory : WebApplicationFactory<Program>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddDebug();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseEnvironment("Testing");
                    webBuilder.UseStartup<SimplifiedTestStartup>();
                });
        }
    }

    /// <summary>
    /// Classe Program vazia apenas para satisfazer o requisito do WebApplicationFactory
    /// </summary>
    public class Program
    {
        // Classe intencionalmente vazia
    }

    /// <summary>
    /// Startup simplificado para testes, sem versionamento de API
    /// </summary>
    public class SimplifiedTestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Adicionar mock services
            services.AddScoped<IAuthService, MockAuthService>();
            services.AddScoped<ILoggingService, MockLoggingService>();
            services.AddScoped<ITokenService, MockTokenService>();
            services.AddScoped<ISocialMediaIntegrationService, MockSocialMediaIntegrationService>();
            services.AddScoped<IRecommendationService, MockRecommendationService>();
            services.AddScoped<ISchedulerService, MockSchedulerService>();
            
            // Adicionar configuração de autenticação para testes
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options => {
                    // Configuração mínima para testes
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    
                    // Configurar TokenValidationParameters para aceitar nossos tokens de teste
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = false
                    };
                });

            // Configurar para usar apenas os controladores de teste
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    // Limpar todos os provedores existentes
                    manager.FeatureProviders.Clear();
                    // Adicionar apenas o provedor que considera os controladores do namespace de testes
                    manager.FeatureProviders.Add(new SimplifiedControllerFeatureProvider());
                });

            // Adicionar API versioning de forma simplificada
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            
            // Adicionar middleware de autenticação
            app.UseAuthentication();
            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    /// <summary>
    /// Provedor que identifica apenas controladores específicos dos testes
    /// </summary>
    public class SimplifiedControllerFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            // Verificamos explicitamente os controladores que queremos incluir
            if (typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestAuthController) ||
                typeInfo == typeof(MicroSaaS.IntegrationTests.Controllers.HealthController) ||
                typeInfo == typeof(MicroSaaS.IntegrationTests.Controllers.TestHealthController) ||
                typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestAnalyticsController) ||
                typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestRevenueController) ||
                typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestContentPostController) ||
                typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestSocialMediaAccountController) ||
                typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestContentChecklistController) ||
                typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestContentCreatorController) ||
                typeInfo == typeof(MicroSaaS.IntegrationTests.Controllers.TestRecommendationController))
            {
                return true;
            }
            
            // Incluir outros controladores do namespace Utils e Controllers
            bool isInUtilsNamespace = typeInfo.Namespace == typeof(MicroSaaS.IntegrationTests.Utils.TestAuthController).Namespace;
            bool isInControllersNamespace = typeInfo.Namespace == typeof(MicroSaaS.IntegrationTests.Controllers.HealthController).Namespace;
            
            return (isInUtilsNamespace || isInControllersNamespace) && base.IsController(typeInfo);
        }
    }
} 