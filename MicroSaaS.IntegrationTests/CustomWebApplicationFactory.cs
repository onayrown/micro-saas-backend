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
                        // Remover todos os serviços de versionamento e route options existentes
                        // para evitar duplicações
                        var descriptors = services
                            .Where(d => d.ServiceType.Namespace?.Contains("Versioning") == true || 
                                   d.ImplementationType?.Namespace?.Contains("Versioning") == true ||
                                   d.ServiceType == typeof(IConfigureOptions<RouteOptions>) || 
                                   d.ServiceType == typeof(IPostConfigureOptions<RouteOptions>))
                            .ToList();

                        foreach (var descriptor in descriptors)
                        {
                            services.Remove(descriptor);
                        }
                    });
                    webBuilder.UseStartup<CustomTestStartup>();
                });
        }
    }

    /// <summary>
    /// Startup extremamente simplificado para testes
    /// </summary>
    public class CustomTestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Adicionar mock services básicos
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
                });

            // Adicionar controladores sem qualquer configuração adicional
            services.AddControllers();

            // Adicionar API versioning com configuração mínima e sem restrições adicionais
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
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
} 