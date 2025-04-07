using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Interfaces;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Infrastructure.Services;

public class DashboardInsightsService : MicroSaaS.Application.Interfaces.Services.IDashboardInsightsService, MicroSaaS.Domain.Interfaces.IDashboardInsightsService
{
    private readonly IDashboardInsightsRepository _repository;
    private readonly IContentPerformanceRepository _performanceRepository;
    private readonly IContentPostRepository _postRepository;

    public DashboardInsightsService(
        IDashboardInsightsRepository repository,
        IContentPerformanceRepository performanceRepository,
        IContentPostRepository postRepository)
    {
        _repository = repository;
        _performanceRepository = performanceRepository;
        _postRepository = postRepository;
    }

    public async Task<DashboardInsights> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<List<DashboardInsights>> GetByCreatorIdAsync(Guid creatorId)
    {
        return await _repository.GetByCreatorIdAsync(creatorId);
    }

    public async Task<DashboardInsights> GetLatestByCreatorIdAsync(Guid creatorId)
    {
        return await _repository.GetLatestByCreatorIdAsync(creatorId);
    }

    public async Task<DashboardInsights> GenerateInsightsAsync(Guid creatorId)
    {
        var performances = await _performanceRepository.GetByCreatorIdAsync(creatorId);
        var posts = await _postRepository.GetByCreatorIdAsync(creatorId);

        var insights = new List<ContentInsight>();
        var recommendations = new List<ContentRecommendation>();

        // Gerar insights baseados no desempenho
        foreach (var post in posts)
        {
            var performance = performances.FirstOrDefault(p => p.PostId == post.Id);
            if (performance != null)
            {
                insights.Add(new ContentInsight
                {
                    Type = InsightType.HighReach,
                    Title = $"Análise de Desempenho - {post.Title}",
                    Description = $"O post teve {performance.Views} visualizações e {performance.EngagementRate}% de engajamento",
                    Metrics = new Dictionary<string, double>
                    {
                        { "views", performance.Views },
                        { "likes", performance.Likes },
                        { "comments", performance.Comments },
                        { "shares", performance.Shares },
                        { "engagementRate", (double)performance.EngagementRate }
                    },
                    Trend = performance.EngagementRate > 5 ? TrendDirection.Up : TrendDirection.Down,
                    ComparisonPeriod = "Últimos 7 dias",
                    PercentageChange = 0
                });
            }
        }

        // Gerar recomendações baseadas nos insights
        recommendations.Add(new ContentRecommendation
        {
            RecommendationType = RecommendationType.Platform,
            Description = "Recomendação de plataforma baseada no desempenho",
            SuggestedTopics = new List<string> { "Tecnologia", "Desenvolvimento", "Programação" },
            Platform = SocialMediaPlatform.All,
            Priority = RecommendationPriority.Medium,
            PotentialImpact = "Alto impacto no engajamento"
        });

        var dashboardInsights = new DashboardInsights
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            Date = DateTime.UtcNow,
            Insights = insights,
            Recommendations = recommendations,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        return await _repository.AddAsync(dashboardInsights);
    }

    public async Task<DashboardInsights> UpdateAsync(DashboardInsights insights)
    {
        return await _repository.UpdateAsync(insights);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }
}