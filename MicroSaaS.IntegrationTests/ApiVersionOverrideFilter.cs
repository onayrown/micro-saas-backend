using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Routing;

namespace MicroSaaS.IntegrationTests
{
    /// <summary>
    /// Intercepta e substitui a lógica original de configuração da apiVersion no ConstraintMap
    /// para evitar o erro de chave duplicada.
    /// </summary>
    public class ApiVersionOverridePostConfigureOptions : IPostConfigureOptions<RouteOptions>
    {
        private readonly bool _doNotAddConstraint;

        public ApiVersionOverridePostConfigureOptions(bool doNotAddConstraint = false)
        {
            _doNotAddConstraint = doNotAddConstraint;
        }

        public void PostConfigure(string? name, RouteOptions options)
        {
            // Sempre remover a chave existente para evitar a exceção de chave duplicada
            if (options.ConstraintMap.ContainsKey("apiVersion"))
            {
                options.ConstraintMap.Remove("apiVersion");
            }

            // Adicionar a chave apenas se não solicitado explicitamente para não adicionar
            if (!_doNotAddConstraint)
            {
                options.ConstraintMap.Add("apiVersion", typeof(ApiVersionRouteConstraint));
            }
        }
    }

    /// <summary>
    /// Extensões para ajudar na configuração de versionamento de API em testes
    /// </summary>
    public static class ApiVersioningTestExtensions
    {
        /// <summary>
        /// Registra o filtro que substitui a configuração original de apiVersion
        /// </summary>
        public static IServiceCollection OverrideApiVersionConstraint(this IServiceCollection services)
        {
            // Remover configurações existentes de RouteOptions
            var descriptors = services
                .Where(d => d.ServiceType == typeof(IConfigureOptions<RouteOptions>)
                         || d.ServiceType == typeof(IPostConfigureOptions<RouteOptions>))
                .ToList();

            foreach (var descriptor in descriptors)
            {
                var implementationType = descriptor.ImplementationType;
                if (implementationType != null && implementationType.Name.Contains("ApiVersioning"))
                {
                    services.Remove(descriptor);
                }
            }

            // Adicionar o nosso configurador personalizado usando a forma correta de factory
            services.AddSingleton<IPostConfigureOptions<RouteOptions>>(
                provider => new ApiVersionOverridePostConfigureOptions());

            return services;
        }
    }
} 