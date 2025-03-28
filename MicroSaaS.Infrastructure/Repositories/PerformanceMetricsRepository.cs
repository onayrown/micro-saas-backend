using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Database;
using MicroSaaS.Infrastructure.Entities;
using MicroSaaS.Infrastructure.Mappers;
using MicroSaaS.Shared.Enums;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Repositories;

public class PerformanceMetricsRepository : IPerformanceMetricsRepository
{
    private readonly IMongoDbContext _context;

    public PerformanceMetricsRepository(IMongoDbContext context)
    {
        _context = context;
    }

    public async Task<PerformanceMetrics> GetByIdAsync(Guid id)
    {
        var entity = await _context.GetCollection<PerformanceMetricsEntity>("performance_metrics")
            .Find(p => p.Id == id.ToString())
            .FirstOrDefaultAsync();
        
        return entity.ToDomain();
    }

    public async Task<IEnumerable<PerformanceMetrics>> GetAllAsync()
    {
        var entities = await _context.GetCollection<PerformanceMetricsEntity>("performance_metrics")
            .Find(_ => true)
            .ToListAsync();
        
        return entities.Select(e => e.ToDomain());
    }

    public async Task<IEnumerable<PerformanceMetrics>> GetByCreatorIdAsync(Guid creatorId)
    {
        var entities = await _context.GetCollection<PerformanceMetricsEntity>("performance_metrics")
            .Find(p => p.CreatorId == creatorId.ToString())
            .ToListAsync();
        
        return entities.Select(e => e.ToDomain());
    }

    public async Task<IEnumerable<PerformanceMetrics>> GetByCreatorAndPlatformAsync(Guid creatorId, SocialMediaPlatform platform)
    {
        var entities = await _context.GetCollection<PerformanceMetricsEntity>("performance_metrics")
            .Find(p => p.CreatorId == creatorId.ToString() && p.Platform == platform)
            .ToListAsync();
        
        return entities.Select(e => e.ToDomain());
    }

    public async Task<IEnumerable<PerformanceMetrics>> GetByDateRangeAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var entities = await _context.GetCollection<PerformanceMetricsEntity>("performance_metrics")
            .Find(p => p.CreatorId == creatorId.ToString() && p.Date >= startDate && p.Date <= endDate)
            .ToListAsync();
        
        return entities.Select(e => e.ToDomain());
    }

    public async Task<PerformanceMetrics> GetByCreatorAndDateAsync(Guid creatorId, DateTime date, SocialMediaPlatform platform)
    {
        var startOfDay = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Utc);
        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

        var entity = await _context.GetCollection<PerformanceMetricsEntity>("performance_metrics")
            .Find(p => p.CreatorId == creatorId.ToString() && p.Platform == platform &&
                   p.Date >= startOfDay && p.Date <= endOfDay)
            .FirstOrDefaultAsync();
        
        return entity.ToDomain();
    }

    public async Task<PerformanceMetrics> AddAsync(PerformanceMetrics metrics)
    {
        var entity = metrics.ToEntity();
        await _context.GetCollection<PerformanceMetricsEntity>("performance_metrics").InsertOneAsync(entity);
        return metrics;
    }

    public async Task<PerformanceMetrics> UpdateAsync(PerformanceMetrics metrics)
    {
        var entity = metrics.ToEntity();
        var filter = Builders<PerformanceMetricsEntity>.Filter.Eq(p => p.Id, metrics.Id.ToString());
        await _context.GetCollection<PerformanceMetricsEntity>("performance_metrics").ReplaceOneAsync(filter, entity);
        return metrics;
    }

    public async Task DeleteAsync(Guid id)
    {
        var filter = Builders<PerformanceMetricsEntity>.Filter.Eq(p => p.Id, id.ToString());
        await _context.GetCollection<PerformanceMetricsEntity>("performance_metrics").DeleteOneAsync(filter);
    }

    public async Task<decimal> GetAverageEngagementRateAsync()
    {
        var entities = await _context.GetCollection<PerformanceMetricsEntity>("performance_metrics")
            .Find(_ => true)
            .ToListAsync();
            
        if (!entities.Any())
            return 0;
            
        var totalEngagementRate = entities.Sum(e => e.EngagementRate);
        return totalEngagementRate / entities.Count;
    }

    public async Task<decimal> GetAverageEngagementRateByCreatorAsync(Guid creatorId)
    {
        var entities = await _context.GetCollection<PerformanceMetricsEntity>("performance_metrics")
            .Find(p => p.CreatorId == creatorId.ToString())
            .ToListAsync();
            
        if (!entities.Any())
            return 0;
            
        var totalEngagementRate = entities.Sum(e => e.EngagementRate);
        return totalEngagementRate / entities.Count;
    }

    public async Task RefreshMetricsAsync()
    {
        var entities = await _context.GetCollection<PerformanceMetricsEntity>("performance_metrics")
            .Find(_ => true)
            .ToListAsync();
            
        foreach (var entity in entities)
        {
            if (entity.TotalViews > 0)
            {
                entity.EngagementRate = ((decimal)(entity.TotalLikes + entity.TotalComments + entity.TotalShares) / entity.TotalViews) * 100;
            }
        }
        
        foreach (var entity in entities)
        {
            var filter = Builders<PerformanceMetricsEntity>.Filter.Eq(p => p.Id, entity.Id);
            await _context.GetCollection<PerformanceMetricsEntity>("performance_metrics").ReplaceOneAsync(filter, entity);
        }
    }
} 