using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MongoDB.Driver;
using Moq;
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Controllers;
using MicroSaaS.IntegrationTests.Controllers;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Routing;

namespace MicroSaaS.IntegrationTests
{
    /// <summary>
    /// Factory personalizada para testes de integração que substitui a configuração da aplicação
    /// com a configuração de teste especificada em TestStartup
    /// </summary>
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Definir ambiente de testes
            builder.UseEnvironment("Testing");

            // Configurar o logging para testes
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
            });

            // Substituir o Startup original pelo nosso TestStartup
            builder.UseStartup<TestStartup>();

            // Configurar serviços adicionais
            builder.ConfigureServices(services =>
            {
                // Remover quaisquer configurações existentes de RouteOptions para evitar conflitos
                var routeOptionsDescriptors = services
                    .Where(d => d.ServiceType == typeof(IConfigureOptions<RouteOptions>))
                    .ToList();

                foreach (var descriptor in routeOptionsDescriptors)
                {
                    services.Remove(descriptor);
                }

                // Configurar RouteOptions de forma limpa, apenas uma vez
                services.Configure<RouteOptions>(options =>
                {
                    // Se a chave já estiver presente, remova-a antes de adicionar novamente
                    if (options.ConstraintMap.ContainsKey("apiVersion"))
                    {
                        options.ConstraintMap.Remove("apiVersion");
                    }

                    // Adicionar a constraint
                    options.ConstraintMap.Add("apiVersion", typeof(ApiVersionRouteConstraint));
                });
            });
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder.UseStartup<TestStartup>();
                    webBuilder.UseTestServer();
                });
        }
    }
    
    public class StartupFilterToReplaceStartup : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            // Discard the original startup configuration
            return app => 
            {
                // Use TestStartup's configuration instead
                // (will be applied by the WebHost)
            };
        }
    }
} 