using MicroSaaS.Application.Interfaces.Repositories;
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

    public ContentPerformanceRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<ContentPerformanceEntity>("contentPerformance");
    }

    public async Task<ContentPerformance> GetByIdAsync(Guid id)
    {
        var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.Id, id);
        var entity = await _collection.Find(filter).FirstOrDefaultAsync();
        return ContentPerformanceMapper.ToDomain(entity);
    }

    public async Task<IEnumerable<ContentPerformance>> GetAllAsync()
    {
        var entities = await _collection.Find(_ => true).ToListAsync();
        return entities.Select(ContentPerformanceMapper.ToDomain);
    }

    public async Task<IEnumerable<ContentPerformance>> GetByPostIdAsync(string postId)
    {
        var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.PostId, Guid.Parse(postId));
        var entities = await _collection.Find(filter).ToListAsync();
        return entities.Select(ContentPerformanceMapper.ToDomain);
    }

    public async Task<IEnumerable<ContentPerformance>> GetByCreatorIdAsync(Guid creatorId)
    {
        var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.CreatorId, creatorId);
        var entities = await _collection.Find(filter).ToListAsync();
        return entities.Select(ContentPerformanceMapper.ToDomain);
    }

    public async Task<IEnumerable<ContentPerformance>> GetByPlatformAsync(SocialMediaPlatform platform)
    {
        var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.Platform, platform);
        var entities = await _collection.Find(filter).ToListAsync();
        return entities.Select(ContentPerformanceMapper.ToDomain);
    }

    public async Task<IEnumerable<ContentPerformance>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var filter = Builders<ContentPerformanceEntity>.Filter.And(
            Builders<ContentPerformanceEntity>.Filter.Gte(x => x.Date, startDate),
            Builders<ContentPerformanceEntity>.Filter.Lte(x => x.Date, endDate)
        );
        var entities = await _collection.Find(filter).ToListAsync();
        return entities.Select(ContentPerformanceMapper.ToDomain);
    }

    public async Task<IEnumerable<ContentPerformance>> GetTopPerformingByViewsAsync(Guid creatorId, int limit = 10)
    {
        var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.CreatorId, creatorId);
        var entities = await _collection.Find(filter)
            .Sort(Builders<ContentPerformanceEntity>.Sort.Descending(x => x.Views))
            .Limit(limit)
            .ToListAsync();
        return entities.Select(ContentPerformanceMapper.ToDomain);
    }

    public async Task<IEnumerable<ContentPerformance>> GetTopPerformingByEngagementAsync(Guid creatorId, int limit = 10)
    {
        var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.CreatorId, creatorId);
        var entities = await _collection.Find(filter)
            .Sort(Builders<ContentPerformanceEntity>.Sort.Descending(x => x.EngagementRate))
            .Limit(limit)
            .ToListAsync();
        return entities.Select(ContentPerformanceMapper.ToDomain);
    }

    public async Task<IEnumerable<ContentPerformance>> GetTopPerformingByRevenueAsync(Guid creatorId, int limit = 10)
    {
        var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.CreatorId, creatorId);
        var entities = await _collection.Find(filter)
            .Sort(Builders<ContentPerformanceEntity>.Sort.Descending(x => x.EstimatedRevenue))
            .Limit(limit)
            .ToListAsync();
        return entities.Select(ContentPerformanceMapper.ToDomain);
    }

    public async Task<ContentPerformance> AddAsync(ContentPerformance performance)
    {
        var entity = ContentPerformanceMapper.ToEntity(performance);
        await _collection.InsertOneAsync(entity);
        return ContentPerformanceMapper.ToDomain(entity);
    }

    public async Task<ContentPerformance> UpdateAsync(ContentPerformance performance)
    {
        var entity = ContentPerformanceMapper.ToEntity(performance);
        var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.Id, entity.Id);
        await _collection.ReplaceOneAsync(filter, entity);
        return performance;
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = Builders<ContentPerformanceEntity>.Filter.Eq(x => x.Id, id);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<decimal> GetAverageEngagementRateAsync(Guid creatorId)
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
    }

    public async Task<decimal> GetAverageEngagementRateByCreatorAsync(Guid creatorId)
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
    }

    public async Task RefreshMetricsAsync()
    {
        var update = Builders<ContentPerformanceEntity>.Update
            .Set(x => x.CollectedAt, DateTime.UtcNow);

        await _collection.UpdateManyAsync(
            Builders<ContentPerformanceEntity>.Filter.Empty,
            update);
    }
} 