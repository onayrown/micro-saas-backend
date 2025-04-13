using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MicroSaaS.Infrastructure.MongoDB;
using MicroSaaS.Infrastructure.Settings;

namespace MicroSaaS.Backend.HealthChecks
{
    public class MongoDbHealthCheck : IHealthCheck
    {
        private readonly IMongoDbContext _mongoContext;
        private readonly string _databaseName;

        public MongoDbHealthCheck(IMongoDbContext mongoContext, IConfiguration configuration)
        {
            _mongoContext = mongoContext ?? throw new ArgumentNullException(nameof(mongoContext));
            
            // Obter o nome do banco de dados da configuração
            var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
            _databaseName = settings?.DatabaseName ?? "microsaas";
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                // Obter a conexão do contexto
                var database = _mongoContext.GetDatabase();
                
                // Ping para verificar a conectividade
                var command = new BsonDocument("ping", 1);
                await database.RunCommandAsync<BsonDocument>(command, cancellationToken: cancellationToken);
                
                return HealthCheckResult.Healthy("MongoDB está acessível");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy("MongoDB não está acessível", ex);
            }
        }
    }
} 