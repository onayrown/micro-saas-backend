using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Routing;

// Esta classe não deve ser um Program nem ter um Main, é apenas uma classe de helper
namespace MicroSaaS.IntegrationTests
{
    public static class IntegrationTestsHelper
    {
        // Método utilitário para configuração de host, não um ponto de entrada
        public static IHostBuilder CreateCustomHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TestStartup>();
                    webBuilder.ConfigureServices(services =>
                    {
                        // Remover todas as opções de versionamento para evitar conflitos
                        var descriptors = services
                            .Where(d => d.ServiceType.Namespace?.Contains("Versioning") == true)
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

                        // Adicionar versioning com configuração mínima
                        services.AddApiVersioning(options =>
                        {
                            options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                            options.AssumeDefaultVersionWhenUnspecified = true;
                            options.ReportApiVersions = true;
                        });

                        // Adicionar o nosso configurador personalizado que evita duplicação da chave apiVersion
                        services.AddSingleton<IPostConfigureOptions<RouteOptions>>(
                            provider => new ApiVersionOverridePostConfigureOptions());
                    });
                });
    }
} 