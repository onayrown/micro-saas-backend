using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.MongoDB;
using MicroSaaS.Shared.Enums;
using MongoDB.Driver;

namespace MicroSaaS.Infrastructure.Persistence.Repositories
{
    public class PerformanceMetricsRepository : IPerformanceMetricsRepository
    {
        private readonly IMongoCollection<PerformanceMetrics> _collection;
        private readonly ICacheService _cacheService;

        public PerformanceMetricsRepository(IMongoDbContext context, ICacheService cacheService)
        {
            _collection = context.GetCollection<PerformanceMetrics>("PerformanceMetrics");
            _cacheService = cacheService;
        }

        public async Task<PerformanceMetrics> AddAsync(PerformanceMetrics metrics)
        {
            metrics.CreatedAt = DateTime.UtcNow;
            metrics.UpdatedAt = DateTime.UtcNow;
            await _collection.InsertOneAsync(metrics);
            await InvalidateCacheAsync(metrics.CreatorId);
            return metrics;
        }

        public async Task<PerformanceMetrics> UpdateAsync(PerformanceMetrics metrics)
        {
            metrics.UpdatedAt = DateTime.UtcNow;
            var filter = Builders<PerformanceMetrics>.Filter.Eq(m => m.Id, metrics.Id);
            await _collection.ReplaceOneAsync(filter, metrics);
            await InvalidateCacheAsync(metrics.CreatorId);
            return metrics;
        }

        public async Task DeleteAsync(Guid id)
        {
            var filter = Builders<PerformanceMetrics>.Filter.Eq(m => m.Id, id.ToString());
            var metrics = await _collection.Find(filter).FirstOrDefaultAsync();
            
            if (metrics != null)
            {
                await _collection.DeleteOneAsync(filter);
                await InvalidateCacheAsync(metrics.CreatorId);
            }
        }

        public async Task<PerformanceMetrics> GetByIdAsync(Guid id)
        {
            var filter = Builders<PerformanceMetrics>.Filter.Eq(m => m.Id, id.ToString());
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PerformanceMetrics>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<PerformanceMetrics>> GetByCreatorIdAsync(Guid creatorId)
        {
            string cacheKey = $"metrics:creator:{creatorId}:all";
            var cachedData = await _cacheService.GetAsync<List<PerformanceMetrics>>(cacheKey);
            if (cachedData != null) return cachedData;

            var filter = Builders<PerformanceMetrics>.Filter.Eq(m => m.CreatorId, creatorId.ToString());
            var data = await _collection.Find(filter).ToListAsync();

            await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromHours(1));
            return data;
        }

        public async Task<IEnumerable<PerformanceMetrics>> GetByCreatorAndPlatformAsync(Guid creatorId, SocialMediaPlatform platform)
        {
            string cacheKey = $"metrics:creator:{creatorId}:platform:{platform}";
            var cachedData = await _cacheService.GetAsync<List<PerformanceMetrics>>(cacheKey);
            if (cachedData != null) return cachedData;

            var filter = Builders<PerformanceMetrics>.Filter.And(
                Builders<PerformanceMetrics>.Filter.Eq(m => m.CreatorId, creatorId.ToString()),
                Builders<PerformanceMetrics>.Filter.Eq(m => m.Platform, platform)
            );
            var data = await _collection.Find(filter).ToListAsync();

            await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromHours(1));
            return data;
        }

        public async Task<IEnumerable<PerformanceMetrics>> GetByDateRangeAsync(Guid creatorId, DateTime startDate, DateTime endDate)
        {
            string cacheKey = $"metrics:creator:{creatorId}:range:{startDate:yyyyMMdd}-{endDate:yyyyMMdd}";
            var cachedData = await _cacheService.GetAsync<List<PerformanceMetrics>>(cacheKey);
            if (cachedData != null) return cachedData;

            var filter = Builders<PerformanceMetrics>.Filter.And(
                Builders<PerformanceMetrics>.Filter.Eq(m => m.CreatorId, creatorId.ToString()),
                Builders<PerformanceMetrics>.Filter.Gte(m => m.Date, startDate),
                Builders<PerformanceMetrics>.Filter.Lte(m => m.Date, endDate)
            );
            var data = await _collection.Find(filter).ToListAsync();

            await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromHours(1));
            return data;
        }

        public async Task<PerformanceMetrics> GetByCreatorAndDateAsync(Guid creatorId, DateTime date, SocialMediaPlatform platform)
        {
            string cacheKey = $"metrics:creator:{creatorId}:date:{date:yyyyMMdd}:platform:{platform}";
            var cachedData = await _cacheService.GetAsync<PerformanceMetrics>(cacheKey);
            if (cachedData != null) return cachedData;

            var filter = Builders<PerformanceMetrics>.Filter.And(
                Builders<PerformanceMetrics>.Filter.Eq(m => m.CreatorId, creatorId.ToString()),
                Builders<PerformanceMetrics>.Filter.Eq(m => m.Date.Date, date.Date),
                Builders<PerformanceMetrics>.Filter.Eq(m => m.Platform, platform)
            );
            var data = await _collection.Find(filter).FirstOrDefaultAsync();

            if (data != null)
            {
                await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromHours(1));
            }
            return data;
        }

        public async Task<decimal> GetAverageEngagementRateAsync()
        {
            string cacheKey = "metrics:avgEngagement:all";
            var cachedData = await _cacheService.GetAsync<decimal?>(cacheKey);
            if (cachedData.HasValue) return cachedData.Value;

            var metrics = await _collection.Find(_ => true).ToListAsync();
            if (!metrics.Any()) return 0;

            var avgEngagement = metrics.Average(m => m.EngagementRate);
            await _cacheService.SetAsync(cacheKey, avgEngagement, TimeSpan.FromHours(1));
            return avgEngagement;
        }

        public async Task<decimal> GetAverageEngagementRateByCreatorAsync(Guid creatorId)
        {
            string cacheKey = $"metrics:avgEngagement:creator:{creatorId}";
            var cachedData = await _cacheService.GetAsync<decimal?>(cacheKey);
            if (cachedData.HasValue) return cachedData.Value;

            var filter = Builders<PerformanceMetrics>.Filter.Eq(m => m.CreatorId, creatorId.ToString());
            var metrics = await _collection.Find(filter).ToListAsync();
            if (!metrics.Any()) return 0;

            var avgEngagement = metrics.Average(m => m.EngagementRate);
            await _cacheService.SetAsync(cacheKey, avgEngagement, TimeSpan.FromHours(1));
            return avgEngagement;
        }

        public async Task RefreshMetricsAsync()
        {
            // Implementação para atualizar métricas de desempenho
            // Esta implementação pode variar dependendo dos requisitos específicos
            // Por exemplo, pode envolver a busca de dados de APIs externas de mídia social
            
            // Por enquanto, apenas invalide o cache
            var allCreators = await _collection.Distinct(m => m.CreatorId, _ => true).ToListAsync();
            foreach (var creatorId in allCreators)
            {
                await InvalidateCacheAsync(creatorId);
            }
        }

        public async Task<IEnumerable<PerformanceMetrics>> GetByCreatorIdBetweenDatesAsync(Guid creatorId, DateTime startDate, DateTime endDate)
        {
            // Este método é similar ao GetByDateRangeAsync, mas pode ser mantido por razões de compatibilidade
            return await GetByDateRangeAsync(creatorId, startDate, endDate);
        }

        public async Task<IEnumerable<PerformanceMetrics>> GetByPostIdAsync(string postId)
        {
            if (string.IsNullOrEmpty(postId))
                return Enumerable.Empty<PerformanceMetrics>();

            string cacheKey = $"metrics:post:{postId}";
            var cachedData = await _cacheService.GetAsync<List<PerformanceMetrics>>(cacheKey);
            if (cachedData != null) return cachedData;

            // Procurando por métricas que possuem o PostId em TopPerformingContentIds
            var filter = Builders<PerformanceMetrics>.Filter.AnyEq(m => m.TopPerformingContentIds, postId);
            var data = await _collection.Find(filter).ToListAsync();

            await _cacheService.SetAsync(cacheKey, data, TimeSpan.FromHours(1));
            return data;
        }

        private async Task InvalidateCacheAsync(string creatorId)
        {
            await _cacheService.RemoveAsync($"metrics:creator:{creatorId}:all");
            await _cacheService.RemoveAsync($"metrics:avgEngagement:creator:{creatorId}");
            await _cacheService.RemoveAsync("metrics:avgEngagement:all");
            
            // Remover outros padrões de cache relacionados
            var platforms = Enum.GetValues(typeof(SocialMediaPlatform))
                .Cast<SocialMediaPlatform>();
            
            foreach (var platform in platforms)
            {
                await _cacheService.RemoveAsync($"metrics:creator:{creatorId}:platform:{platform}");
            }
        }
    }
} 