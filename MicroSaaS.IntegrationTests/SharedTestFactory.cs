using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace MicroSaaS.IntegrationTests
{
    /// <summary>
    /// Fábrica de teste compartilhada que é reutilizada entre os testes para evitar
    /// a duplicação de inicialização de servidores de teste e o problema de apiVersion
    /// </summary>
    public class SharedTestFactory : WebApplicationFactory<MicroSaaS.Backend.Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Use a classe TestStartup em vez do Startup normal
            builder.UseStartup<TestStartup>();
            
            // Use ambiente de testes
            builder.UseEnvironment("Testing");
            
            // Configurar serviços adicionais
            builder.ConfigureServices(services =>
            {
                // Remover serviços de versionamento existentes
                var descriptors = services
                    .Where(d => d.ServiceType.Namespace?.Contains("Versioning") == true || 
                           d.ImplementationType?.Namespace?.Contains("Versioning") == true)
                    .ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                // Remover qualquer IConfigureOptions ou IPostConfigureOptions para RouteOptions
                var routeOptionsDescriptors = services
                    .Where(d => d.ServiceType == typeof(IConfigureOptions<RouteOptions>) || 
                           d.ServiceType == typeof(IPostConfigureOptions<RouteOptions>))
                    .ToList();

                foreach (var descriptor in routeOptionsDescriptors)
                {
                    services.Remove(descriptor);
                }

                // Registrar nossa opção personalizada para evitar duplicação de apiVersion
                // Corrigindo o registro para usar factory com provedor de serviço
                services.AddSingleton<IPostConfigureOptions<RouteOptions>>(
                    sp => new ApiVersionOverridePostConfigureOptions(false));
                    
                // Garante que os controladores de teste sejam encontrados
                services.AddMvcCore().ConfigureApplicationPartManager(manager => 
                {
                    manager.FeatureProviders.Add(new TestControllerFeatureProvider());
                });
            });
        }

        // Classe auxiliar para garantir que os nossos controladores de teste sejam encontrados
        public class TestControllerFeatureProvider : ControllerFeatureProvider
        {
            protected override bool IsController(TypeInfo typeInfo)
            {
                if (typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestAuthController) ||
                    typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestAnalyticsController) ||
                    typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestRevenueController))
                {
                    return true;
                }
                
                return base.IsController(typeInfo);
            }
        }
    }
} 