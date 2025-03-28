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

namespace MicroSaaS.IntegrationTests
{
    public class TestWebApplicationFactory<TEntryPoint> 
        : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            builder.UseStartup<TestStartup>();
            
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddDebug();
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