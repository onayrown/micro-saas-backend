using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Routing;

namespace MicroSaaS.IntegrationTests
{
    /// <summary>
    /// Garante que o API versioning seja configurado apenas uma vez durante a inicialização do host.
    /// Esta classe é executada antes de todo o resto quando o host é iniciado.
    /// </summary>
    public class VersioningHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remover todas as configurações existentes de versioning
                var descriptors = services
                    .Where(d => d.ServiceType.Namespace?.Contains("Versioning") == true || 
                           d.ImplementationType?.Namespace?.Contains("Versioning") == true)
                    .ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

                // Remover serviços relacionados a RouteOptions
                var routeOptionsDescriptors = services
                    .Where(d => d.ServiceType == typeof(IConfigureOptions<RouteOptions>) || 
                           d.ServiceType == typeof(IPostConfigureOptions<RouteOptions>))
                    .ToList();

                foreach (var descriptor in routeOptionsDescriptors)
                {
                    if (descriptor.ImplementationType?.Name.Contains("ApiVersioning") == true)
                    {
                        services.Remove(descriptor);
                    }
                }

                // Adicionar versioning API com configuração limpa
                services.AddApiVersioning(options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                });

                // Adicionar o nosso configurador personalizado que evita duplicação da chave apiVersion
                // Corrigindo o registro para usar factory com provedor de serviço
                services.AddSingleton<IPostConfigureOptions<RouteOptions>>(
                    sp => new ApiVersionOverridePostConfigureOptions());
            });
        }
    }
} 