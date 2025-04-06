using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;

namespace MicroSaaS.Infrastructure.Settings
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public int ServerSelectionTimeout { get; set; }
        public int ConnectTimeout { get; set; }
    }

    public static class MongoDbServiceExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services)
        {
            services.AddSingleton<MongoDB.Driver.MongoClient>(sp => {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                var mongoSettings = MongoDB.Driver.MongoClientSettings.FromUrl(
                    new MongoDB.Driver.MongoUrl(settings.ConnectionString));
                
                // Configurar timeouts
                mongoSettings.ServerSelectionTimeout = TimeSpan.FromMilliseconds(settings.ServerSelectionTimeout);
                mongoSettings.ConnectTimeout = TimeSpan.FromMilliseconds(settings.ConnectTimeout);
                
                // Adicionar configurações adicionais para resolver problemas de DNS
                mongoSettings.DirectConnection = true;
                mongoSettings.MaxConnectionPoolSize = 100;
                mongoSettings.MinConnectionPoolSize = 10;
                mongoSettings.ConnectTimeout = TimeSpan.FromSeconds(60);
                mongoSettings.SocketTimeout = TimeSpan.FromSeconds(60);
                mongoSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(60);
                
                // Desabilitar heartbeats e outros recursos que podem causar problemas
                mongoSettings.HeartbeatInterval = TimeSpan.FromSeconds(60);
                
                // Registrar um resolvedor de DNS personalizado para mapear "mongodb" para "localhost"
                var customResolver = new MongoDB.Driver.Core.Configuration.DnsEndPointResolver(new[] 
                {
                    new KeyValuePair<string, string>("mongodb", "127.0.0.1")
                });
                mongoSettings.EndpointResolver = customResolver;
                
                Log.Information("Conectando ao MongoDB com a string de conexão: {ConnectionString}", settings.ConnectionString);
                
                return new MongoDB.Driver.MongoClient(mongoSettings);
            });

            return services;
        }
    }
} 