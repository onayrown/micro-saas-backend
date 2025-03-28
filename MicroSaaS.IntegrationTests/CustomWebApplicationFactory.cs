using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using System;
using Microsoft.Extensions.Logging;

namespace MicroSaaS.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<TestStartup>
    {
        protected override IHostBuilder CreateHostBuilder()
        {
            var hostBuilder = Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<TestStartup>();
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseEnvironment("Testing");
                    
                    // Configurar o servidor de teste
                    webBuilder.UseTestServer(options =>
                    {
                        options.PreserveExecutionContext = true;
                    });
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddDebug();
                });
                
            Console.WriteLine("CreateHostBuilder: Host builder configured");
            return hostBuilder;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Console.WriteLine("ConfigureWebHost: Starting configuration");
            
            builder.UseEnvironment("Testing");
            
            // Configurar outros aspectos do servidor, como logging
            builder.ConfigureServices(services =>
            {
                // Adicionar logging mais detalhado
                services.AddLogging(options =>
                {
                    options.ClearProviders();
                    options.AddConsole();
                    options.AddDebug();
                });
                
                Console.WriteLine("ConfigureWebHost: Services configured");
            });
            
            Console.WriteLine("ConfigureWebHost: Configuration complete");
        }
    }
} 