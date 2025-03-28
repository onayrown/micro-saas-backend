using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Database;
using MicroSaaS.Infrastructure.Entities;
using MicroSaaS.Infrastructure.Mappers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Repositories;

public class DashboardInsightsRepository : IDashboardInsightsRepository
{
    private readonly IMongoCollection<DashboardInsightsEntity> _collection;

    public DashboardInsightsRepository(IMongoDbContext context)
    {
        _collection = context.GetCollection<DashboardInsightsEntity>("DashboardInsights");
    }

    public async Task<DashboardInsights> GetByIdAsync(Guid id)
    {
        var filter = Builders<DashboardInsightsEntity>.Filter.Eq(d => d.Id, id);
        var entity = await _collection.Find(filter).FirstOrDefaultAsync();
        return DashboardInsightsMapper.ToDomain(entity);
    }

    public async Task<IEnumerable<DashboardInsights>> GetAllAsync()
    {
        var entities = await _collection.Find(_ => true).ToListAsync();
        return entities.Select(DashboardInsightsMapper.ToDomain);
    }

    public async Task<DashboardInsights> GetByCreatorAndPeriodAsync(Guid creatorId, DateTime periodStart, DateTime periodEnd)
    {
        var filter = Builders<DashboardInsightsEntity>.Filter.And(
            Builders<DashboardInsightsEntity>.Filter.Eq(x => x.CreatorId, creatorId),
            Builders<DashboardInsightsEntity>.Filter.Eq(x => x.PeriodStart, periodStart),
            Builders<DashboardInsightsEntity>.Filter.Eq(x => x.PeriodEnd, periodEnd)
        );
        var entity = await _collection.Find(filter).FirstOrDefaultAsync();
        return DashboardInsightsMapper.ToDomain(entity);
    }

    public async Task<IEnumerable<DashboardInsights>> GetByCreatorIdAsync(Guid creatorId)
    {
        var filter = Builders<DashboardInsightsEntity>.Filter.Eq(d => d.CreatorId, creatorId);
        var entities = await _collection.Find(filter).ToListAsync();
        return entities.Select(DashboardInsightsMapper.ToDomain);
    }

    public async Task<DashboardInsights> GetLatestByCreatorIdAsync(Guid creatorId)
    {
        var filter = Builders<DashboardInsightsEntity>.Filter.Eq(d => d.CreatorId, creatorId);
        var entities = await _collection.Find(filter)
            .Sort(Builders<DashboardInsightsEntity>.Sort.Descending(x => x.GeneratedDate))
            .Limit(1)
            .ToListAsync();
        return entities.Select(DashboardInsightsMapper.ToDomain).FirstOrDefault();
    }

    public async Task<DashboardInsights> AddAsync(DashboardInsights insights)
    {
        var entity = DashboardInsightsMapper.ToEntity(insights);
        await _collection.InsertOneAsync(entity);
        return DashboardInsightsMapper.ToDomain(entity);
    }

    public async Task<DashboardInsights> UpdateAsync(DashboardInsights insights)
    {
        var entity = DashboardInsightsMapper.ToEntity(insights);
        var filter = Builders<DashboardInsightsEntity>.Filter.Eq(d => d.Id, entity.Id);
        await _collection.ReplaceOneAsync(filter, entity);
        return DashboardInsightsMapper.ToDomain(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = Builders<DashboardInsightsEntity>.Filter.Eq(d => d.Id, id);
        await _collection.DeleteOneAsync(filter);
    }

    public async Task<IEnumerable<DashboardInsights>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var filter = Builders<DashboardInsightsEntity>.Filter.And(
            Builders<DashboardInsightsEntity>.Filter.Gte(d => d.PeriodStart, startDate),
            Builders<DashboardInsightsEntity>.Filter.Lte(d => d.PeriodEnd, endDate)
        );
        var entities = await _collection.Find(filter).ToListAsync();
        return entities.Select(DashboardInsightsMapper.ToDomain);
    }

    public async Task DeleteByCreatorIdAsync(Guid creatorId)
    {
        var filter = Builders<DashboardInsightsEntity>.Filter.Eq(d => d.CreatorId, creatorId);
        await _collection.DeleteManyAsync(filter);
    }

    public async Task RefreshMetricsAsync()
    {
        // Implementação da lógica de atualização de métricas
    }
} 