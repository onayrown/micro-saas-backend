using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Data;
using MicroSaaS.Infrastructure.Entities;
using MicroSaaS.Infrastructure.Mappers;
using MicroSaaS.Shared.Enums;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Repositories;

public class ContentPerformanceRepository : IContentPerformanceRepository
{
    private readonly IMongoCollection<ContentPerformanceEntity> _collection;
    private readonly ICacheService _cacheService;
    private const string CacheKeyPrefix = "perf_";
    private readonly TimeSpan _defaultCacheTime = TimeSpan.FromMinutes(15);

    public ContentPerformanceRepository(IMongoDatabase database, ICacheService cacheService)
    {
        _collection = database.GetCollection<ContentPerformanceEntity>("contentPerformance");
        _cacheService = cacheService;
    }

    public async Task<ContentPerformance> GetByIdAsync(Guid id)
    {
        string cacheKey = $"{CacheKeyPrefix}id_{id}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.Id, id);
            var entity = await _collection.Find(filter).FirstOrDefaultAsync();
            return ContentPerformanceMapper.ToDomain(entity);
        }, _defaultCacheTime);
    }

    public async Task<IEnumerable<ContentPerformance>> GetAllAsync()
    {
        string cacheKey = $"{CacheKeyPrefix}all";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            var entities = await _collection.Find(_ => true).ToListAsync();
            return entities.Select(ContentPerformanceMapper.ToDomain);
        }, _defaultCacheTime);
    }

    public async Task<IEnumerable<ContentPerformance>> GetByPostIdAsync(string postId)
    {
        string cacheKey = $"{CacheKeyPrefix}post_{postId}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.PostId, Guid.Parse(postId));
            var entities = await _collection.Find(filter).ToListAsync();
            return entities.Select(ContentPerformanceMapper.ToDomain);
        }, _defaultCacheTime);
    }

    public async Task<IEnumerable<ContentPerformance>> GetByCreatorIdAsync(Guid creatorId)
    {
        string cacheKey = $"{CacheKeyPrefix}creator_{creatorId}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.CreatorId, creatorId);
            var entities = await _collection.Find(filter).ToListAsync();
            return entities.Select(ContentPerformanceMapper.ToDomain);
        }, _defaultCacheTime);
    }

    public async Task<IEnumerable<ContentPerformance>> GetByPlatformAsync(SocialMediaPlatform platform)
    {
        string cacheKey = $"{CacheKeyPrefix}platform_{platform}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.Platform, platform);
            var entities = await _collection.Find(filter).ToListAsync();
            return entities.Select(ContentPerformanceMapper.ToDomain);
        }, _defaultCacheTime);
    }

    public async Task<IEnumerable<ContentPerformance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        string cacheKey = $"{CacheKeyPrefix}date_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            var filter = Builders<ContentPerformanceEntity>.Filter.And(
                Builders<ContentPerformanceEntity>.Filter.Gte(x => x.Date, startDate),
                Builders<ContentPerformanceEntity>.Filter.Lte(x => x.Date, endDate)
            );
            var entities = await _collection.Find(filter).ToListAsync();
            return entities.Select(ContentPerformanceMapper.ToDomain);
        }, _defaultCacheTime);
    }

    public async Task<IEnumerable<ContentPerformance>> GetTopPerformingByViewsAsync(Guid creatorId, int limit = 10)
    {
        string cacheKey = $"{CacheKeyPrefix}topviews_{creatorId}_{limit}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.CreatorId, creatorId);
            var entities = await _collection.Find(filter)
                .Sort(Builders<ContentPerformanceEntity>.Sort.Descending(x => x.Views))
                .Limit(limit)
                .ToListAsync();
            return entities.Select(ContentPerformanceMapper.ToDomain);
        }, _defaultCacheTime);
    }

    public async Task<IEnumerable<ContentPerformance>> GetTopPerformingByEngagementAsync(Guid creatorId, int limit = 10)
    {
        string cacheKey = $"{CacheKeyPrefix}topengagement_{creatorId}_{limit}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.CreatorId, creatorId);
            var entities = await _collection.Find(filter)
                .Sort(Builders<ContentPerformanceEntity>.Sort.Descending(x => x.EngagementRate))
                .Limit(limit)
                .ToListAsync();
            return entities.Select(ContentPerformanceMapper.ToDomain);
        }, _defaultCacheTime);
    }

    public async Task<IEnumerable<ContentPerformance>> GetTopPerformingByRevenueAsync(Guid creatorId, int limit = 10)
    {
        string cacheKey = $"{CacheKeyPrefix}toprevenue_{creatorId}_{limit}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.CreatorId, creatorId);
            var entities = await _collection.Find(filter)
                .Sort(Builders<ContentPerformanceEntity>.Sort.Descending(x => x.EstimatedRevenue))
                .Limit(limit)
                .ToListAsync();
            return entities.Select(ContentPerformanceMapper.ToDomain);
        }, _defaultCacheTime);
    }

    public async Task<ContentPerformance> AddAsync(ContentPerformance performance)
    {
        var entity = ContentPerformanceMapper.ToEntity(performance);
        await _collection.InsertOneAsync(entity);
        
        // Invalidar cache relacionado
        await InvalidateCreatorCacheAsync(performance.CreatorId);
        
        return ContentPerformanceMapper.ToDomain(entity);
    }

    public async Task<ContentPerformance> UpdateAsync(ContentPerformance performance)
    {
        var entity = ContentPerformanceMapper.ToEntity(performance);
        var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.Id, entity.Id);
        await _collection.ReplaceOneAsync(filter, entity);
        
        // Invalidar cache relacionado
        await InvalidateCreatorCacheAsync(performance.CreatorId);
        await InvalidatePostCacheAsync(performance.PostId.ToString());
        await _cacheService.RemoveAsync($"{CacheKeyPrefix}id_{performance.Id}");
        
        return performance;
    }

    public async Task DeleteAsync(Guid id)
    {
        // Recuperar entidade para invalidar cache
        var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.Id, id);
        var entity = await _collection.Find(filter).FirstOrDefaultAsync();
        
        if (entity != null)
        {
            await _collection.DeleteOneAsync(filter);
            
            // Invalidar cache
            await InvalidateCreatorCacheAsync(entity.CreatorId);
            await InvalidatePostCacheAsync(entity.PostId.ToString());
            await _cacheService.RemoveAsync($"{CacheKeyPrefix}id_{id}");
        }
        else
        {
            await _collection.DeleteOneAsync(filter);
        }
    }

    public async Task<decimal> GetAverageEngagementRateAsync(Guid creatorId)
    {
        string cacheKey = $"{CacheKeyPrefix}avgrate_{creatorId}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("CreatorId", creatorId)),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", null },
                    { "averageRate", new BsonDocument("$avg", "$EngagementRate") }
                })
            };

            var result = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return result != null ? result["averageRate"].AsDecimal : 0;
        }, _defaultCacheTime);
    }

    public async Task<decimal> GetAverageEngagementRateByCreatorAsync(Guid creatorId)
    {
        string cacheKey = $"{CacheKeyPrefix}avgratecreator_{creatorId}";
        return await _cacheService.GetOrSetAsync(cacheKey, async () =>
        {
            var pipeline = new[]
            {
                new BsonDocument("$match", new BsonDocument("CreatorId", creatorId)),
                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", null },
                    { "averageRate", new BsonDocument("$avg", "$EngagementRate") }
                })
            };

            var result = await _collection.Aggregate<BsonDocument>(pipeline).FirstOrDefaultAsync();
            return result != null ? result["averageRate"].AsDecimal : 0;
        }, _defaultCacheTime);
    }

    public async Task RefreshMetricsAsync()
    {
        var update = Builders<ContentPerformanceEntity>.Update
            .Set(x => x.CollectedAt, DateTime.UtcNow);

        await _collection.UpdateManyAsync(
            Builders<ContentPerformanceEntity>.Filter.Empty,
            update);
            
        // Limpar todo o cache ao atualizar todas as métricas
        await _cacheService.RemoveAsync($"{CacheKeyPrefix}all");
    }
    
    // Métodos privados para gerenciar o cache
    
    private async Task InvalidateCreatorCacheAsync(Guid creatorId)
    {
        await _cacheService.RemoveAsync($"{CacheKeyPrefix}creator_{creatorId}");
        await _cacheService.RemoveAsync($"{CacheKeyPrefix}avgrate_{creatorId}");
        await _cacheService.RemoveAsync($"{CacheKeyPrefix}avgratecreator_{creatorId}");
        await _cacheService.RemoveAsync($"{CacheKeyPrefix}topviews_{creatorId}_10");
        await _cacheService.RemoveAsync($"{CacheKeyPrefix}topengagement_{creatorId}_10");
        await _cacheService.RemoveAsync($"{CacheKeyPrefix}toprevenue_{creatorId}_10");
    }
    
    private async Task InvalidatePostCacheAsync(string postId)
    {
        await _cacheService.RemoveAsync($"{CacheKeyPrefix}post_{postId}");
    }
} 