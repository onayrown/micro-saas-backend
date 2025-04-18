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
using MicroSaaS.IntegrationTests.Mocks;
using MicroSaaS.IntegrationTests.Utils;
using MongoDB.Driver;
using Moq;
using System;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Routing;

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
            // Adicionar o suporte ao versionamento de API de forma simplificada
            services.AddApiVersioning();

            // Adicionar mock repositories (referenciando o namespace Mocks)
            services.AddScoped<IUserRepository, MicroSaaS.IntegrationTests.Mocks.MockUserRepository>();
            services.AddScoped<ISocialMediaAccountRepository, MicroSaaS.IntegrationTests.Mocks.MockSocialMediaAccountRepository>();

            // Adicionar mock services (referenciando o namespace Mocks)
            services.AddScoped<IAuthService, MicroSaaS.IntegrationTests.Mocks.MockAuthService>();
            services.AddScoped<ILoggingService, MicroSaaS.IntegrationTests.Mocks.MockLoggingService>();
            services.AddScoped<ITokenService, MicroSaaS.IntegrationTests.Mocks.MockTokenService>();
            services.AddScoped<IDashboardService, MicroSaaS.IntegrationTests.Mocks.MockDashboardService>();
            services.AddScoped<IRecommendationService, MicroSaaS.IntegrationTests.Mocks.MockRecommendationService>();
            services.AddScoped<ISocialMediaIntegrationService, MicroSaaS.IntegrationTests.Mocks.MockSocialMediaIntegrationService>();
            services.AddScoped<ISchedulerService, MicroSaaS.IntegrationTests.Mocks.MockSchedulerService>();

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
                typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestRevenueController).GetTypeInfo() ||
                typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestContentPostController).GetTypeInfo() ||
                typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestSocialMediaAccountController).GetTypeInfo() ||
                typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestContentChecklistController).GetTypeInfo() ||
                typeInfo == typeof(MicroSaaS.IntegrationTests.Utils.TestContentCreatorController).GetTypeInfo())
            {
                return true;
            }
            
            return false;
        }
    }
} 