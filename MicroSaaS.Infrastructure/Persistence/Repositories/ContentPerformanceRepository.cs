using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.MongoDB;
using MicroSaaS.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MicroSaaS.Infrastructure.Persistence.Repositories
{
    public class ContentPerformanceRepository : IContentPerformanceRepository
    {
        private readonly IMongoCollection<ContentPerformance> _collection;
        private readonly ICacheService _cacheService;

        public ContentPerformanceRepository(IMongoDbContext context, ICacheService cacheService)
        {
            _collection = context.GetCollection<ContentPerformance>("ContentPerformances");
            _cacheService = cacheService;
        }

        public async Task<ContentPerformance> AddAsync(ContentPerformance performance)
        {
            await _collection.InsertOneAsync(performance);
            await InvalidateCacheAsync(performance.CreatorId, performance.PostId);
            return performance;
        }

        public async Task<ContentPerformance> UpdateAsync(ContentPerformance performance)
        {
            var filter = Builders<ContentPerformance>.Filter.Eq(p => p.Id, performance.Id);
            await _collection.ReplaceOneAsync(filter, performance);
            
            // Invalidar cache
            await _cacheService.RemoveAsync($"perf:id:{performance.Id}");
            await _cacheService.RemoveAsync($"perf:creator:{performance.CreatorId}:all");
            await _cacheService.RemoveAsync($"perf:post:{performance.PostId}:all");
            return performance;
        }

        private async Task InvalidateCacheAsync(Guid creatorId, Guid? postId)
        {
            await _cacheService.RemoveAsync($"perf:creator:{creatorId}:all");
            if (postId.HasValue)
            {
                 await _cacheService.RemoveAsync($"perf:post:{postId.Value}:all");
            }
        }

        public async Task<IEnumerable<ContentPerformance>> GetByCreatorIdAsync(Guid creatorId, DateTime startDate, DateTime endDate)
        {
            string cacheKey = $"perf:creator:{creatorId}:{startDate:yyyyMMdd}-{endDate:yyyyMMdd}";
            var cachedData = await _cacheService.GetAsync<List<ContentPerformance>>(cacheKey);
            if (cachedData != null) return cachedData;

            var filter = Builders<ContentPerformance>.Filter.And(
                Builders<ContentPerformance>.Filter.Eq(p => p.CreatorId, creatorId),
                Builders<ContentPerformance>.Filter.Gte(p => p.Date, startDate),
                Builders<ContentPerformance>.Filter.Lte(p => p.Date, endDate)
            );
            var data = await _collection.Find(filter).ToListAsync();
            await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromHours(1));
            return data;
        }

        public async Task<ContentPerformance?> GetByPostIdAndDateAsync(Guid postId, DateTime date)
        {
             string cacheKey = $"perf:post:{postId}:{date:yyyyMMdd}";
             var cachedData = await _cacheService.GetAsync<ContentPerformance>(cacheKey);
             if (cachedData != null) return cachedData;

            var filter = Builders<ContentPerformance>.Filter.And(
                Builders<ContentPerformance>.Filter.Eq(p => p.PostId, postId),
                Builders<ContentPerformance>.Filter.Eq(p => p.Date, date.Date)
            );
             var data = await _collection.Find(filter).FirstOrDefaultAsync();

             if (data != null)
             {
                await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromHours(1));
             }
            return data;
        }

        public Task<IEnumerable<ContentPerformance>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<ContentPerformance> GetByIdAsync(Guid id)
        {
            string cacheKey = $"perf:id:{id}";
            var cachedData = await _cacheService.GetAsync<ContentPerformance>(cacheKey);
            if (cachedData != null) return cachedData;

            var filter = Builders<ContentPerformance>.Filter.Eq(p => p.Id, id);
            var data = await _collection.Find(filter).FirstOrDefaultAsync();

            if (data != null)
            {
                await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromHours(1));
            }
            return data;
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ContentPerformance>> GetByCreatorIdAsync(Guid creatorId)
        {
             string cacheKey = $"perf:creator:{creatorId}:all";
             var cachedData = await _cacheService.GetAsync<List<ContentPerformance>>(cacheKey);
             if (cachedData != null) return cachedData;

            var filter = Builders<ContentPerformance>.Filter.Eq(p => p.CreatorId, creatorId);
            
            // Verificar se a coleção é vazia antes de tentar fazer operações nela
            try 
            {
                var data = await _collection.Find(filter).ToListAsync();
                if (data != null && data.Any())
                {
                    await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromHours(1));
                }
                return data ?? new List<ContentPerformance>();
            }
            catch (ArgumentNullException)
            {
                // Se a coleção não estiver inicializada ou houver outro problema, retornar lista vazia
                return new List<ContentPerformance>();
            }
        }

        public Task<IEnumerable<ContentPerformance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ContentPerformance>> GetByPlatformAsync(SocialMediaPlatform platform)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ContentPerformance>> GetByPostIdAsync(string? postId)
        {
             if (string.IsNullOrEmpty(postId))
             {
                 return Task.FromResult(Enumerable.Empty<ContentPerformance>());
             }
             
             Guid postGuid;
             if (!Guid.TryParse(postId, out postGuid))
             {
                 return Task.FromResult(Enumerable.Empty<ContentPerformance>());
             }
             var filter = Builders<ContentPerformance>.Filter.Eq(p => p.PostId, postGuid);
             return _collection.Find(filter).ToListAsync().ContinueWith(t => (IEnumerable<ContentPerformance>)t.Result);
        }

        public Task<decimal> GetAverageEngagementRateByCreatorAsync(Guid creatorId)
        {
            throw new NotImplementedException();
        }

        public Task<decimal> GetAverageEngagementRateAsync(Guid postId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ContentPerformance>> GetTopPerformingByViewsAsync(Guid creatorId, int limit)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ContentPerformance>> GetTopPerformingByEngagementAsync(Guid creatorId, int limit)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ContentPerformance>> GetTopPerformingByRevenueAsync(Guid creatorId, int limit)
        {
            throw new NotImplementedException();
        }

        public async Task RefreshMetricsAsync()
        {
            // Implementação básica para atualizar um campo, por exemplo.
            // Ajuste conforme a lógica real necessária.
            var update = Builders<ContentPerformance>.Update
                .Set(x => x.CollectedAt, DateTime.UtcNow);
            await _collection.UpdateManyAsync(Builders<ContentPerformance>.Filter.Empty, update);

            // Invalidação de cache - considerar uma estratégia mais granular se possível
            // Remover uma chave geral pode ser ineficiente.
            await _cacheService.RemoveAsync("perf:all"); // Exemplo de chave geral
        }
    }
} 