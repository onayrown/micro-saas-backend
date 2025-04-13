using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.MongoDB;
using MicroSaaS.Shared.Enums;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Persistence.Repositories
{
    public class DashboardInsightsRepository : IDashboardInsightsRepository
    {
        private readonly IMongoCollection<DashboardInsights> _collection;
        private readonly IMongoCollection<DashboardInsights> _insights;

        public DashboardInsightsRepository(IMongoDbContext context)
        {
            _insights = context.GetCollection<DashboardInsights>("DashboardInsights");
            _collection = _insights;
        }

        public async Task<DashboardInsights?> GetByIdAsync(Guid id)
        {
            var filter = Builders<DashboardInsights>.Filter.Eq(d => d.Id, id);
            return await _insights.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DashboardInsights>> GetAllAsync()
        {
            return await _insights.Find(Builders<DashboardInsights>.Filter.Empty).ToListAsync();
        }

        public async Task<IEnumerable<DashboardInsights>> GetByCreatorIdAsync(Guid creatorId)
        {
            var filter = Builders<DashboardInsights>.Filter.Eq(d => d.CreatorId, creatorId);
            return await _insights.Find(filter).ToListAsync();
        }

        public async Task<DashboardInsights?> GetLatestByCreatorIdAsync(Guid creatorId)
        {
            var filter = Builders<DashboardInsights>.Filter.Eq(i => i.CreatorId, creatorId);
            return await _insights
                .Find(filter)
                .SortByDescending(i => i.GeneratedDate)
                .FirstOrDefaultAsync();
        }

        public async Task<DashboardInsights?> GetByCreatorAndPeriodAsync(Guid creatorId, DateTime periodStart, DateTime periodEnd)
        {
            var filter = Builders<DashboardInsights>.Filter.And(
                Builders<DashboardInsights>.Filter.Eq(d => d.CreatorId, creatorId),
                Builders<DashboardInsights>.Filter.Gte(d => d.GeneratedDate, periodStart),
                Builders<DashboardInsights>.Filter.Lt(d => d.GeneratedDate, periodEnd)
            );
            return await _insights.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<DashboardInsights> AddAsync(DashboardInsights insights)
        {
            insights.Id = insights.Id == Guid.Empty ? Guid.NewGuid() : insights.Id;
            insights.GeneratedDate = DateTime.UtcNow;
            insights.CreatedAt = DateTime.UtcNow;
            insights.UpdatedAt = DateTime.UtcNow;
            await _insights.InsertOneAsync(insights);
            return insights;
        }

        public async Task<DashboardInsights?> UpdateAsync(DashboardInsights insights)
        {
            insights.UpdatedAt = DateTime.UtcNow;
            var filter = Builders<DashboardInsights>.Filter.Eq(i => i.Id, insights.Id);
            var result = await _insights.ReplaceOneAsync(filter, insights);
            return result.IsAcknowledged && result.ModifiedCount > 0 ? insights : null;
        }

        public async Task DeleteAsync(Guid id)
        {
            var filter = Builders<DashboardInsights>.Filter.Eq(d => d.Id, id);
            await _insights.DeleteOneAsync(filter);
        }

        public async Task<List<DashboardInsights>> GetByCreatorIdAndDateRangeAsync(Guid creatorId, DateTime startDate, DateTime endDate, InsightType type)
        {
            return await _insights
                .Find(i => i.CreatorId == creatorId && i.Type == type && i.GeneratedDate >= startDate && i.GeneratedDate < endDate)
                .SortByDescending(i => i.GeneratedDate)
                .ToListAsync();
        }

        public async Task<List<DashboardInsights>> GetByCreatorIdAsync(Guid creatorId, InsightType type)
        {
            return await _insights
                .Find(i => i.CreatorId == creatorId && i.Type == type)
                .SortByDescending(i => i.GeneratedDate)
                .ToListAsync();
        }
    }
} 