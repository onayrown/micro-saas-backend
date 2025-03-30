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

            // Configurar para usar apenas os controladores de teste
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    // Limpar todos os provedores existentes
                    manager.FeatureProviders.Clear();
                    // Adicionar apenas o provedor que considera os controladores do namespace de testes
                    manager.FeatureProviders.Add(new SimplifiedControllerFeatureProvider());
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
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
            // Após remover o TestAuthController duplicado, agora usamos apenas
            // o do namespace MicroSaaS.IntegrationTests.Utils
            bool isInUtilsNamespace = typeInfo.Namespace == typeof(MicroSaaS.IntegrationTests.Utils.TestAuthController).Namespace;
            bool isInControllersNamespace = typeInfo.Namespace == typeof(MicroSaaS.IntegrationTests.Controllers.HealthController).Namespace;
            
            return (isInUtilsNamespace || isInControllersNamespace) && base.IsController(typeInfo);
        }
    }
} 