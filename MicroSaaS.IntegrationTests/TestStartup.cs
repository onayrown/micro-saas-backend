using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.IntegrationTests.Controllers;
using MicroSaaS.IntegrationTests.Utils;
using MongoDB.Driver;
using Moq;
using System;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MicroSaaS.IntegrationTests
{
    public class TestStartup
    {
        private readonly IConfiguration _configuration;

        public TestStartup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Este método será chamado pelo runtime para adicionar serviços ao container
        public void ConfigureServices(IServiceCollection services)
        {
            // Não vamos adicionar versionamento aqui - será feito na SharedTestFactory 
            // para garantir apenas uma configuração

            // Adicionar mock services
            services.AddScoped<IAuthService, MockAuthService>();
            services.AddScoped<ILoggingService, MockLoggingService>();
            services.AddScoped<ITokenService, MockTokenService>();
            services.AddScoped<ISocialMediaIntegrationService, MockSocialMediaIntegrationService>();
            services.AddScoped<IRecommendationService, MockRecommendationService>();
            services.AddScoped<ISchedulerService, MockSchedulerService>();

            // Configurar serviços MVC
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    // Limpar todos os provedores existentes
                    manager.FeatureProviders.Clear();
                    // Adicionar apenas o provedor que considera os controladores do namespace de testes
                    manager.FeatureProviders.Add(new FilteredControllersFeatureProvider());
                });
        }

        // Este método será chamado pelo runtime para configurar o pipeline HTTP
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class FilteredControllersFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            // Garantir que apenas nossos controladores de teste sejam reconhecidos
            if (typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestAuthController).GetTypeInfo() ||
                typeInfo == typeof(TestRecommendationController).GetTypeInfo() ||
                typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestAnalyticsController).GetTypeInfo() ||
                typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestRevenueController).GetTypeInfo())
            {
                return true;
            }
            
            return false;
        }
    }
} 