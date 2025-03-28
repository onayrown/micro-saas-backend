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
using MongoDB.Driver;
using Moq;
using System;
using System.Reflection;

namespace MicroSaaS.IntegrationTests
{
    public class TestStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // Registrar serviços de logging
            services.AddLogging(options =>
            {
                options.ClearProviders();
                options.AddConsole();
                options.AddDebug();
            });

            // Registrar serviços MVC
            services.AddControllers()
                .ConfigureApplicationPartManager(manager =>
                {
                    // Remover todos os provedores de recursos existentes
                    manager.FeatureProviders.Clear();
                    
                    // Adicionar nosso provedor personalizado
                    manager.FeatureProviders.Add(new TestControllersFeatureProvider());
                });

            // Registrar serviços necessários para os testes
            services.AddSingleton<IMongoDatabase>(sp => Mock.Of<IMongoDatabase>());
            services.AddScoped<ISocialMediaAccountRepository, MockSocialMediaAccountRepository>();
            services.AddScoped<IUserRepository, MockUserRepository>();
            services.AddScoped<ITokenService, MockTokenService>();
            services.AddScoped<IAuthService, MockAuthService>();
            services.AddScoped<ISocialMediaIntegrationService, MockSocialMediaIntegrationService>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<TestStartup> logger)
        {
            // Middleware para logging de requisições
            app.Use(async (context, next) =>
            {
                logger.LogInformation($"Incoming request: {context.Request.Method} {context.Request.Path}");
                await next();
            });

            // Middleware para logging de respostas
            app.Use(async (context, next) =>
            {
                var originalBodyStream = context.Response.Body;
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                await next();

                responseBody.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                logger.LogInformation($"Response: {context.Response.StatusCode} - {responseText}");

                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
            });

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public class TestControllersFeatureProvider : ControllerFeatureProvider
    {
        protected override bool IsController(TypeInfo typeInfo)
        {
            // Garantir que apenas nosso controlador de teste seja reconhecido
            if (typeInfo == typeof(TestAuthController).GetTypeInfo())
            {
                return true;
            }
            
            return false;
        }
    }
} 