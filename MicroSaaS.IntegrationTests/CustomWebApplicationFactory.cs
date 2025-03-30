using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MicroSaaS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;
using MicroSaaS.IntegrationTests.Utils;
using MicroSaaS.IntegrationTests.Controllers;

namespace MicroSaaS.IntegrationTests
{
    /// <summary>
    /// Fábrica de teste extremamente simplificada para evitar problemas com a configuração do API versioning
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<MicroSaaS.Backend.Program>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            // Usamos Create e não CreateDefault para evitar qualquer
            // configuração automática que possa causar problemas
            return Host.CreateDefaultBuilder()
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddDebug();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseEnvironment("Testing");
                    webBuilder.ConfigureServices(services =>
                    {
                        // Configuração similar à SimplifiedTestFactory
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
                                options.RequireHttpsMetadata = false;
                                options.SaveToken = true;
                                
                                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                                {
                                    ValidateIssuer = false,
                                    ValidateAudience = false,
                                    ValidateLifetime = false,
                                    ValidateIssuerSigningKey = false
                                };
                            });

                        // Configurar para usar apenas os controladores de teste
                        services.AddControllers().ConfigureApplicationPartManager(manager =>
                        {
                            // Limpar todos os provedores existentes
                            manager.FeatureProviders.Clear();
                            // Adicionar apenas o provedor que considera os controladores do namespace de testes
                            manager.FeatureProviders.Add(new TestControllerFeatureProvider());
                        });

                        // Adicionar API versioning simplificado
                        services.AddApiVersioning(options =>
                        {
                            options.DefaultApiVersion = new ApiVersion(1, 0);
                            options.AssumeDefaultVersionWhenUnspecified = true;
                            options.ReportApiVersions = true;
                            options.ApiVersionReader = new QueryStringApiVersionReader("api-version");
                        });
                    });
                    webBuilder.Configure(app =>
                    {
                        app.UseRouting();
                        
                        // Adicionar middleware de autenticação
                        app.UseAuthentication();
                        app.UseAuthorization();
                        
                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
                });
        }
        
        /// <summary>
        /// Provedor que identifica apenas controladores específicos dos testes
        /// </summary>
        public class TestControllerFeatureProvider : ControllerFeatureProvider
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
} 