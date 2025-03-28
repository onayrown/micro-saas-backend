using Microsoft.Extensions.Configuration;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Infrastructure.Data;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Services;

public class RevenueService : IRevenueService
{
    private readonly IContentCreatorRepository _creatorRepository;
    private readonly IConfiguration _configuration;
    private readonly MongoDbContext _context;

    public RevenueService(
        IContentCreatorRepository creatorRepository,
        IConfiguration configuration,
        MongoDbContext context)
    {
        _creatorRepository = creatorRepository;
        _configuration = configuration;
        _context = context;
    }

    public async Task<Application.Interfaces.Services.RevenueSummary> GetRevenueSummaryAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new ArgumentException("Criador de conteúdo não encontrado");

        var dailyRevenues = await GetDailyRevenueAsync(creatorId, startDate, endDate);
        var platformRevenues = await GetPlatformRevenueAsync(creatorId, startDate, endDate);
        var totalRevenue = await GetTotalRevenueAsync(creatorId, startDate, endDate);

        return new Application.Interfaces.Services.RevenueSummary
        {
            TotalRevenue = totalRevenue,
            EstimatedMonthlyRevenue = CalculateMRR(platformRevenues.ToList()),
            AverageRevenuePerView = totalRevenue / platformRevenues.Sum(p => p.Views)
        };
    }

    public async Task<IEnumerable<Application.Interfaces.Services.DailyRevenue>> GetDailyRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        return await GetRevenueByDayAsync(creatorId, startDate, endDate);
    }

    public async Task<IEnumerable<Application.Interfaces.Services.PlatformRevenue>> GetPlatformRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        return await GetRevenueByPlatformAsync(creatorId, startDate, endDate);
    }

    public async Task<decimal> GetTotalRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var filter = Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.And(
            Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.Gte(p => p.Date, startDate),
            Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.Lte(p => p.Date, endDate)
        );

        var performances = await _context.ContentPerformances
            .Find(filter)
            .ToListAsync();
            
        return performances.Sum(p => p.EstimatedRevenue);
    }

    public async Task<string> GetAdSenseAuthUrlAsync(Guid creatorId, string callbackUrl)
    {
        // Implementação temporária
        return string.Empty;
    }

    public async Task<bool> ConnectAdSenseAsync(Guid creatorId, string authorizationCode)
    {
        // Implementação temporária
        return false;
    }

    public async Task<bool> IntegrateGoogleAdSenseAsync(ContentCreator creator, string accessToken)
    {
        // Implementação temporária
        return false;
    }

    public async Task<decimal> GetEstimatedRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var filter = Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.And(
            Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.Gte(p => p.Date, startDate),
            Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.Lte(p => p.Date, endDate)
        );

        var performances = await _context.ContentPerformances
            .Find(filter)
            .ToListAsync();
            
        return performances.Sum(p => p.EstimatedRevenue);
    }

    public async Task<Application.Interfaces.Services.RevenueSummary> GetRevenueAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var filter = Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.And(
            Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.Gte(p => p.Date, startDate),
            Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.Lte(p => p.Date, endDate)
        );

        var performances = await _context.ContentPerformances
            .Find(filter)
            .ToListAsync();
            
        var totalRevenue = performances.Sum(p => p.EstimatedRevenue);
        var totalViews = performances.Sum(p => p.Views);
        
        return new Application.Interfaces.Services.RevenueSummary
        {
            TotalRevenue = totalRevenue,
            EstimatedMonthlyRevenue = totalRevenue * 30 / (endDate - startDate).Days,
            AverageRevenuePerView = totalViews > 0 ? totalRevenue / totalViews : 0
        };
    }

    public async Task<List<Application.Interfaces.Services.PlatformRevenue>> GetRevenueByPlatformAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var filter = Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.And(
            Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.Gte(p => p.Date, startDate),
            Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.Lte(p => p.Date, endDate)
        );

        var performances = await _context.ContentPerformances
            .Find(filter)
            .ToListAsync();
            
        return performances
            .GroupBy(p => p.Platform)
            .Select(g => new Application.Interfaces.Services.PlatformRevenue
            {
                Platform = g.Key.ToString(),
                Revenue = g.Sum(p => p.EstimatedRevenue),
                Views = g.Sum(p => p.Views)
            })
            .ToList();
    }

    public async Task<List<Application.Interfaces.Services.DailyRevenue>> GetRevenueByDayAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var filter = Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.And(
            Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.Gte(p => p.Date, startDate),
            Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.Lte(p => p.Date, endDate)
        );

        var performances = await _context.ContentPerformances
            .Find(filter)
            .ToListAsync();
            
        return performances
            .GroupBy(p => p.Date.Date)
            .Select(g => new Application.Interfaces.Services.DailyRevenue
            {
                Date = g.Key,
                Revenue = g.Sum(p => p.EstimatedRevenue),
                Views = g.Sum(p => p.Views),
                Amount = g.Sum(p => p.EstimatedRevenue)
            })
            .ToList();
    }

    private decimal CalculateMRR(IEnumerable<Application.Interfaces.Services.PlatformRevenue> platformRevenues)
    {
        // Implementação temporária
        return 0;
    }

    private decimal CalculateARR(IEnumerable<Application.Interfaces.Services.PlatformRevenue> platformRevenues)
    {
        // Implementação temporária
        return 0;
    }

    public async Task<decimal> CalculateContentRevenueAsync(Guid contentId)
    {
        var filter = Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.Eq("PostId", contentId.ToString());
        var performances = await _context.ContentPerformances
            .Find(filter)
            .ToListAsync();
            
        return performances.Sum(p => p.EstimatedRevenue);
    }

    public async Task<decimal> CalculateCreatorRevenueAsync(Guid creatorId)
    {
        var filter = Builders<MicroSaaS.Infrastructure.Entities.ContentPostEntity>.Filter.Eq(x => x.CreatorId, creatorId);
        var posts = await _context.ContentPosts
            .Find(filter)
            .ToListAsync();
            
        var postIds = posts.Select(p => p.Id).ToList();
        
        var performanceFilter = Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.In(x => x.PostId, postIds);
        var performances = await _context.ContentPerformances
            .Find(performanceFilter)
            .ToListAsync();
            
        return performances.Sum(p => p.EstimatedRevenue);
    }

    public async Task RefreshRevenueMetricsAsync()
    {
        var filter = Builders<MicroSaaS.Infrastructure.Entities.ContentPostEntity>.Filter.Empty;
        var posts = await _context.ContentPosts
            .Find(filter)
            .ToListAsync();
            
        foreach (var post in posts)
        {
            var performanceFilter = Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Filter.Eq(p => p.PostId, post.Id);
            var performances = await _context.ContentPerformances
                .Find(performanceFilter)
                .ToListAsync();
                
            foreach (var performance in performances)
            {
                // Em uma implementação real, você atualizaria as métricas de receita
                // Por enquanto, apenas atualizamos o timestamp
                performance.CollectedAt = DateTime.UtcNow;
                var update = Builders<MicroSaaS.Infrastructure.Entities.ContentPerformanceEntity>.Update
                    .Set(p => p.CollectedAt, DateTime.UtcNow);
                    
                await _context.ContentPerformances.UpdateOneAsync(
                    p => p.Id == performance.Id,
                    update);
            }
        }
    }
} 