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
using Microsoft.AspNetCore.Mvc;

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

                // Adicionar API versioning simplificado
                services.AddApiVersioning();

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
                    typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestRevenueController) ||
                    typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestContentPostController) ||
                    typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestSocialMediaAccountController) ||
                    typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestContentChecklistController) ||
                    typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestContentCreatorController) ||
                    typeInfo == typeof(MicroSaaS.IntegrationTests.Controllers.TestRecommendationController) ||
                    typeInfo == typeof(MicroSaaS.IntegrationTests.Controllers.HealthController))
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