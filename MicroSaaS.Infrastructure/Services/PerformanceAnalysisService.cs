using MicroSaaS.Application.DTOs.Performance;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Infrastructure.Services;

public class PerformanceAnalysisService : IPerformanceAnalysisService
{
    private readonly IContentPostRepository _contentRepository;
    private readonly IContentCreatorRepository _creatorRepository;
    private readonly ISocialMediaAccountRepository _socialMediaRepository;
    private readonly IPerformanceMetricsRepository _metricsRepository;
    private readonly IRevenueService _revenueService;
    private readonly IContentPerformanceRepository _contentPerformanceRepository;
    private readonly IDashboardInsightsRepository _insightsRepository;

    public PerformanceAnalysisService(
        IContentPostRepository contentRepository,
        IContentCreatorRepository creatorRepository,
        ISocialMediaAccountRepository socialMediaRepository,
        IPerformanceMetricsRepository metricsRepository,
        IRevenueService revenueService,
        IContentPerformanceRepository contentPerformanceRepository,
        IDashboardInsightsRepository insightsRepository)
    {
        _contentRepository = contentRepository;
        _creatorRepository = creatorRepository;
        _socialMediaRepository = socialMediaRepository;
        _metricsRepository = metricsRepository;
        _revenueService = revenueService;
        _contentPerformanceRepository = contentPerformanceRepository;
        _insightsRepository = insightsRepository;
    }

    public async Task<DashboardMetricsDto> GetDashboardMetricsAsync()
    {
        var creators = await _creatorRepository.GetAllAsync();
        var metrics = new DashboardMetricsDto
        {
            TotalCreators = creators.Count(),
            TotalPosts = creators.Sum(c => c.Posts?.Count ?? 0)
        };

        var creatorPerformances = new List<CreatorPerformanceDto>();
        foreach (var creator in creators)
        {
            var posts = creator.Posts ?? new List<ContentPost>();
            long totalViewsLong = posts.Sum(p => p.Views);
            long totalLikesLong = posts.Sum(p => p.Likes);
            long totalCommentsLong = posts.Sum(p => p.Comments);
            int totalViews = (int)totalViewsLong;
            int totalLikes = (int)totalLikesLong;
            int totalComments = (int)totalCommentsLong;

            if (totalViews > 0)
            {
                creatorPerformances.Add(new CreatorPerformanceDto
                {
                    CreatorId = creator.Id.ToString(),
                    Name = creator.Name,
                    Views = totalViews,
                    Likes = totalLikes,
                    Comments = totalComments,
                    EngagementRate = await _contentPerformanceRepository.GetAverageEngagementRateAsync(creator.Id)
                });
            }
        }

        metrics.TopCreators = creatorPerformances
            .OrderByDescending(c => c.Views)
            .Take(5)
            .ToList();

        metrics.AverageEngagementRate = await _contentPerformanceRepository.GetAverageEngagementRateAsync(creators.FirstOrDefault()?.Id ?? Guid.Empty);

        return metrics;
    }

    public async Task<ContentMetricsDto> GetContentMetricsAsync(string contentId)
    {
        var content = await _contentRepository.GetByIdAsync(Guid.Parse(contentId));
        if (content == null)
            throw new KeyNotFoundException($"Conteúdo não encontrado: {contentId}");

        var metrics = new ContentMetricsDto
        {
            ContentId = content.Id.ToString(),
            Title = content.Title,
            CreatorName = content.Creator.Name,
            CreatedAt = content.CreatedAt,
            PublishedAt = content.PublishedAt,
            Status = content.Status.ToString(),
            Views = (int)content.Views, // Explicit cast from long to int
            Likes = (int)content.Likes, // Explicit cast from long to int
            Comments = (int)content.Comments, // Explicit cast from long to int
            Shares = (int)content.Shares, // Explicit cast from long to int
            EngagementRate = await CalculateEngagementRateAsync(Guid.Parse(contentId)),
            Revenue = await _revenueService.CalculateContentRevenueAsync(content.Id),
            PlatformMetrics = await GetPlatformMetricsAsync(content),
            DailyMetrics = await GetDailyMetricsAsync(content)
        };

        return metrics;
    }

    public async Task<CreatorMetricsDto> GetCreatorMetricsAsync(string creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(Guid.Parse(creatorId));
        if (creator == null)
            throw new KeyNotFoundException($"Criador não encontrado: {creatorId}");

        var metrics = new CreatorMetricsDto
        {
            CreatorId = creator.Id.ToString(),
            Name = creator.Name,
            Email = creator.Email,
            JoinedAt = creator.CreatedAt,
            TotalContent = await _contentRepository.CountByCreatorAsync(creator.Id),
            TotalFollowers = await _socialMediaRepository.GetTotalFollowersByCreatorAsync(creator.Id),
            AverageEngagementRate = await _metricsRepository.GetAverageEngagementRateByCreatorAsync(creator.Id),
            TotalRevenue = await _revenueService.CalculateCreatorRevenueAsync(creator.Id),
            PlatformFollowers = await GetPlatformFollowersAsync(creator),
            MonthlyRevenue = await GetMonthlyRevenueAsync(creator),
            TopPerformingContent = await GetTopPerformingContentByCreatorAsync(creator.Id, 5)
        };

        return metrics;
    }

    public async Task RefreshMetricsAsync()
    {
        await _metricsRepository.RefreshMetricsAsync();
        await _revenueService.RefreshRevenueMetricsAsync();
        await _socialMediaRepository.RefreshSocialMediaMetricsAsync();
    }

    public async Task<PerformanceMetrics> GetDailyMetricsAsync(Guid creatorId, SocialMediaPlatform platform, DateTime date)
    {
        var metrics = await _metricsRepository.GetByCreatorAndDateAsync(creatorId, date, platform);
        
        if (metrics == null)
        {
            metrics = new PerformanceMetrics
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Platform = platform,
                Date = date,
                Followers = 0,
                FollowersGrowth = 0,
                TotalViews = 0,
                TotalLikes = 0,
                TotalComments = 0,
                TotalShares = 0,
                EngagementRate = 0,
                EstimatedRevenue = 0,
                TopPerformingContentIds = new List<string>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }

        return metrics;
    }

    public async Task<IEnumerable<PerformanceMetrics>> GetMetricsTimelineAsync(
        Guid creatorId, DateTime startDate, DateTime endDate, SocialMediaPlatform? platform = null)
    {
        var metrics = await _metricsRepository.GetByDateRangeAsync(creatorId, startDate, endDate);
        
        if (platform.HasValue)
        {
            metrics = metrics.Where(m => m.Platform == platform.Value);
        }

        return metrics.OrderBy(m => m.Date);
    }

    public async Task<DashboardInsights> GenerateInsightsAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var existingInsights = await _insightsRepository.GetByCreatorAndPeriodAsync(creatorId, startDate, endDate);
        if (existingInsights != null)
        {
            return existingInsights;
        }

        var metrics = await _metricsRepository.GetByDateRangeAsync(creatorId, startDate, endDate);
        var platforms = metrics.Select(m => m.Platform).Distinct().ToList();
        
        var performances = await _contentPerformanceRepository.GetByCreatorIdAsync(creatorId);
        var periodPerformances = performances
            .Where(p => p.Date >= startDate && p.Date <= endDate)
            .ToList();

        var growthRate = CalculateGrowthRate(metrics);
        var totalRevenue = periodPerformances.Sum(p => p.EstimatedRevenue);
        
        var topContentIds = periodPerformances
            .OrderByDescending(p => p.Views)
            .Take(5)
            .Select(p => p.PostId)
            .ToList();
        
        var contentInsights = new List<ContentInsight>();
        foreach (var postId in topContentIds)
        {
            var post = await _contentRepository.GetByIdAsync(Guid.Parse(postId.ToString()));
            if (post != null)
            {
                var performance = periodPerformances.FirstOrDefault(p => p.PostId == postId);
                if (performance != null)
                {
                    contentInsights.Add(new ContentInsight
                    {
                        PostId = post.Id,
                        Title = post.Title,
                        Platform = post.Platform,
                        PublishedAt = post.PublishedAt.GetValueOrDefault(DateTime.MinValue),
                        Views = performance.Views,
                        EngagementRate = CalculateEngagementRate(performance),
                        Revenue = performance.EstimatedRevenue,
                        InsightType = DetermineInsightType(performance),
                        Insight = GenerateInsightText(performance, post)
                    });
                }
            }
        }
        
        var recommendations = GenerateRecommendations(periodPerformances, platforms);
        var bestTimes = await GetBestTimeToPostAsync(creatorId, platforms.FirstOrDefault());
        
        var insights = new DashboardInsights
        {
            Id = Guid.NewGuid(),
            CreatorId = creatorId,
            GeneratedDate = DateTime.UtcNow,
            Platforms = platforms,
            PeriodStart = startDate,
            PeriodEnd = endDate,
            GrowthRate = growthRate,
            TotalRevenueInPeriod = totalRevenue,
            ComparisonWithPreviousPeriod = 0,
            TopContentInsights = contentInsights,
            Recommendations = recommendations,
            BestTimeToPost = bestTimes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        await _insightsRepository.AddAsync(insights);
        return insights;
    }

    public async Task<List<ContentPost>> GetTopPerformingContentAsync(Guid creatorId, int limit = 5)
    {
        var topPerformances = await _contentPerformanceRepository.GetTopPerformingByViewsAsync(creatorId, limit);
        var result = new List<ContentPost>();
        
        foreach (var performance in topPerformances)
        {
            var post = await _contentRepository.GetByIdAsync(Guid.Parse(performance.PostId.ToString()));
            if (post != null)
            {
                result.Add(post);
            }
        }
        
        return result;
    }

    public async Task<List<PostTimeRecommendation>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform)
    {
        var recommendations = new List<PostTimeRecommendation>
        {
            new PostTimeRecommendation { DayOfWeek = DayOfWeek.Monday, TimeOfDay = new TimeSpan(18, 0, 0), EngagementScore = 8.5 },
            new PostTimeRecommendation { DayOfWeek = DayOfWeek.Wednesday, TimeOfDay = new TimeSpan(12, 0, 0), EngagementScore = 9.0 },
            new PostTimeRecommendation { DayOfWeek = DayOfWeek.Friday, TimeOfDay = new TimeSpan(20, 0, 0), EngagementScore = 9.5 },
            new PostTimeRecommendation { DayOfWeek = DayOfWeek.Sunday, TimeOfDay = new TimeSpan(15, 0, 0), EngagementScore = 8.7 }
        };
        
        return recommendations;
    }

    public async Task<decimal> CalculateAverageEngagementRateAsync(Guid creatorId, SocialMediaPlatform platform)
    {
        var performances = await _contentPerformanceRepository.GetByCreatorIdAsync(creatorId);
        var platformPerformances = performances.Where(p => p.Platform == platform).ToList();
        
        if (!platformPerformances.Any())
        {
            return 0;
        }
        
        var totalEngagement = platformPerformances.Sum(p => CalculateEngagementRate(p));
        return totalEngagement / platformPerformances.Count;
    }

    public async Task<decimal> CalculateRevenueGrowthAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var currentPeriodRevenue = await _revenueService.GetTotalRevenueAsync(creatorId, startDate, endDate);
        
        var previousPeriodLength = endDate - startDate;
        var previousPeriodStart = startDate.AddDays(-previousPeriodLength.Days);
        var previousPeriodEnd = startDate.AddDays(-1);
        
        var previousPeriodRevenue = await _revenueService.GetTotalRevenueAsync(creatorId, previousPeriodStart, previousPeriodEnd);
        
        if (previousPeriodRevenue == 0)
            return 100; // Crescimento de 100% se não havia receita anterior
            
        return ((currentPeriodRevenue - previousPeriodRevenue) / previousPeriodRevenue) * 100;
    }

    public async Task<int> CalculateFollowerGrowthAsync(Guid creatorId, SocialMediaPlatform platform, DateTime startDate, DateTime endDate)
    {
        var metrics = await _metricsRepository.GetByCreatorAndPlatformAsync(creatorId, platform);
        var startMetrics = metrics.FirstOrDefault(m => m.Date.Date == startDate.Date);
        var endMetrics = metrics.FirstOrDefault(m => m.Date.Date == endDate.Date);
        
        if (startMetrics == null || endMetrics == null)
            return 0;
            
        return endMetrics.Followers - startMetrics.Followers;
    }

    private decimal CalculateEngagementRate(ContentPerformance performance)
    {
        if (performance.Views == 0)
            return 0;
            
        var totalEngagements = performance.Likes + performance.Comments + performance.Shares;
        return ((decimal)totalEngagements / performance.Views) * 100;
    }

    private decimal CalculateGrowthRate(IEnumerable<PerformanceMetrics> metrics)
    {
        if (!metrics.Any())
            return 0;
            
        var orderedMetrics = metrics.OrderBy(m => m.Date).ToList();
        var firstMetrics = orderedMetrics.First();
        var lastMetrics = orderedMetrics.Last();
        
        if (firstMetrics.Followers == 0)
            return 100;
            
        return ((decimal)(lastMetrics.Followers - firstMetrics.Followers) / firstMetrics.Followers) * 100;
    }

    private InsightType DetermineInsightType(ContentPerformance performance)
    {
        var engagementRate = CalculateEngagementRate(performance);
        
        if (engagementRate > 10)
            return InsightType.HighEngagement;
        else if (performance.Views > 10000)
            return InsightType.HighReach;
        else if (performance.EstimatedRevenue > 1000)
            return InsightType.HighRevenue;
        else
            return InsightType.Normal;
    }

    private string GenerateInsightText(ContentPerformance performance, ContentPost post)
    {
        var engagementRate = CalculateEngagementRate(performance);
        
        return $"Este post alcançou uma taxa de engajamento de {engagementRate:F1}% " +
               $"com {performance.Views:N0} visualizações, {performance.Likes:N0} curtidas e " +
               $"{performance.Comments:N0} comentários. " +
               $"A receita estimada foi de R$ {performance.EstimatedRevenue:F2}.";
    }

    private List<ContentRecommendation> GenerateRecommendations(
        List<ContentPerformance> performances,
        List<SocialMediaPlatform> platforms)
    {
        var recommendations = new List<ContentRecommendation>();
        
        // Analisar horários de maior engajamento
        var bestPerformingTimes = performances
            .OrderByDescending(p => CalculateEngagementRate(p))
            .Take(5)
            .Select(p => p.Date.TimeOfDay)
            .GroupBy(t => t.Hours)
            .OrderByDescending(g => g.Count())
            .First();
            
        recommendations.Add(new ContentRecommendation
        {
            Type = RecommendationType.Timing,
            Platform = platforms.FirstOrDefault(),
            Description = $"Os melhores horários para postagem são entre {bestPerformingTimes.Key}:00 e {bestPerformingTimes.Key + 1}:00"
        });
        
        // Analisar plataformas com melhor desempenho
        var bestPlatform = performances
            .GroupBy(p => p.Platform)
            .OrderByDescending(g => g.Average(p => CalculateEngagementRate(p)))
            .First();
            
        recommendations.Add(new ContentRecommendation
        {
            Type = RecommendationType.Platform,
            Platform = bestPlatform.Key,
            Description = $"A plataforma {bestPlatform.Key} apresenta as melhores taxas de engajamento"
        });
        
        return recommendations;
    }

    private async Task<List<PlatformMetricsDto>> GetPlatformMetricsAsync(ContentPost content)
    {
        var performances = await _contentPerformanceRepository.GetByPostIdAsync(content.Id.ToString());
        
        return performances.Select(p => new PlatformMetricsDto
        {
            Platform = p.Platform.ToString(),
            Views = (int)p.Views,
            Likes = (int)p.Likes,
            Comments = (int)p.Comments,
            Shares = (int)p.Shares,
            EngagementRate = CalculateEngagementRate(p)
        }).ToList();
    }

    private async Task<List<DailyMetricsDto>> GetDailyMetricsAsync(ContentPost content)
    {
        var performances = await _contentPerformanceRepository.GetByPostIdAsync(content.Id.ToString());
        
        return performances
            .OrderBy(p => p.Date)
            .Select(p => new DailyMetricsDto
            {
                Date = p.Date,
                Views = (int)p.Views,
                Likes = (int)p.Likes,
                Comments = (int)p.Comments,
                Shares = (int)p.Shares,
                EngagementRate = CalculateEngagementRate(p),
                Revenue = p.EstimatedRevenue
            }).ToList();
    }

    private async Task<List<PlatformFollowersDto>> GetPlatformFollowersAsync(ContentCreator creator)
    {
        var accounts = await _socialMediaRepository.GetByCreatorIdAsync(creator.Id);
        var result = new List<PlatformFollowersDto>();
        
        foreach (var account in accounts)
        {
            var metrics = await _metricsRepository.GetByCreatorAndPlatformAsync(creator.Id, account.Platform);
            var latestMetrics = metrics.OrderByDescending(m => m.Date).FirstOrDefault();
            
            if (latestMetrics != null)
            {
                result.Add(new PlatformFollowersDto
                {
                    Platform = account.Platform.ToString(),
                    Followers = latestMetrics.Followers,
                    Growth = latestMetrics.FollowersGrowth,
                    GrowthRate = latestMetrics.Followers > 0 
                        ? ((decimal)latestMetrics.FollowersGrowth / latestMetrics.Followers) * 100 
                        : 0
                });
            }
        }
        
        return result;
    }

    private async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(ContentCreator creator)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddMonths(-12);
        
        var dailyRevenue = await _revenueService.GetDailyRevenueAsync(creator.Id, startDate, endDate);
        
        return dailyRevenue
            .GroupBy(r => new DateTime(r.Date.Year, r.Date.Month, 1))
            .Select(g => new MonthlyRevenueDto
            {
                Month = g.Key,
                Revenue = g.Sum(r => r.Amount),
                Growth = 0, // Será calculado abaixo
                GrowthRate = 0 // Será calculado abaixo
            })
            .OrderBy(m => m.Month)
            .ToList();
    }

    private async Task<List<ContentPerformanceDto>> GetTopPerformingContentByCreatorAsync(Guid creatorId, int limit)
    {
        var performances = await _contentPerformanceRepository.GetTopPerformingByViewsAsync(creatorId, limit);
        var result = new List<ContentPerformanceDto>();
        
        foreach (var performance in performances)
        {
            var post = await _contentRepository.GetByIdAsync(Guid.Parse(performance.PostId.ToString()));
            if (post != null)
            {
                result.Add(new ContentPerformanceDto
                {
                    ContentId = post.Id.ToString(),
                    Title = post.Title,
                    CreatorName = post.Creator?.Name ?? "Desconhecido",
                    Views = (int)performance.Views,
                    Likes = (int)performance.Likes,
                    Comments = (int)performance.Comments,
                    EngagementRate = CalculateEngagementRate(performance)
                });
            }
        }
        
        return result;
    }

    private async Task<RevenueMetricsDto> GetRevenueMetricsAsync()
    {
        var now = DateTime.UtcNow;
        var allCreators = await _creatorRepository.GetAllAsync();
        var totalRevenue = 0m;
        
        foreach (var creator in allCreators)
        {
            // Receita diária
            var dailyRevenue = await _revenueService.GetTotalRevenueAsync(
                creator.Id,
                now.Date,
                now.Date.AddDays(1).AddTicks(-1));
                
            // Receita semanal
            var weeklyRevenue = await _revenueService.GetTotalRevenueAsync(
                creator.Id,
                now.Date.AddDays(-7),
                now.Date);
                
            // Receita mensal
            var monthlyRevenue = await _revenueService.GetTotalRevenueAsync(
                creator.Id,
                now.Date.AddMonths(-1),
                now.Date);
                
            totalRevenue += monthlyRevenue;
        }
        
        // Calcular crescimento
        var previousMonthRevenue = await GetPreviousMonthRevenue(allCreators);
        var revenueGrowth = previousMonthRevenue > 0 
            ? ((totalRevenue - previousMonthRevenue) / previousMonthRevenue) * 100 
            : 100;
            
        return new RevenueMetricsDto
        {
            DailyRevenue = await GetTotalDailyRevenue(allCreators),
            WeeklyRevenue = await GetTotalWeeklyRevenue(allCreators),
            MonthlyRevenue = totalRevenue,
            RevenueGrowth = revenueGrowth
        };
    }

    private async Task<decimal> GetPreviousMonthRevenue(IEnumerable<ContentCreator> creators)
    {
        var now = DateTime.UtcNow;
        var previousMonthStart = now.Date.AddMonths(-2);
        var previousMonthEnd = now.Date.AddMonths(-1);
        var totalRevenue = 0m;
        
        foreach (var creator in creators)
        {
            var revenue = await _revenueService.GetTotalRevenueAsync(
                creator.Id,
                previousMonthStart,
                previousMonthEnd);
                
            totalRevenue += revenue;
        }
        
        return totalRevenue;
    }

    private async Task<decimal> GetTotalDailyRevenue(IEnumerable<ContentCreator> creators)
    {
        var now = DateTime.UtcNow;
        var totalRevenue = 0m;
        
        foreach (var creator in creators)
        {
            var revenue = await _revenueService.GetTotalRevenueAsync(
                creator.Id,
                now.Date,
                now.Date.AddDays(1).AddTicks(-1));
                
            totalRevenue += revenue;
        }
        
        return totalRevenue;
    }

    private async Task<decimal> GetTotalWeeklyRevenue(IEnumerable<ContentCreator> creators)
    {
        var now = DateTime.UtcNow;
        var totalRevenue = 0m;
        
        foreach (var creator in creators)
        {
            var revenue = await _revenueService.GetTotalRevenueAsync(
                creator.Id,
                now.Date.AddDays(-7),
                now.Date);
                
            totalRevenue += revenue;
        }
        
        return totalRevenue;
    }

    private async Task<EngagementMetricsDto> GetEngagementMetricsAsync()
    {
        var now = DateTime.UtcNow;
        var allCreators = await _creatorRepository.GetAllAsync();
        var metrics = new EngagementMetricsDto();
        
        foreach (var creator in allCreators)
        {
            // Métricas diárias
            var dailyMetrics = await _metricsRepository.GetByCreatorAndDateAsync(
                creator.Id,
                now.Date,
                SocialMediaPlatform.All);
                
            if (dailyMetrics != null)
            {
                metrics.DailyEngagements += dailyMetrics.TotalLikes + 
                                          dailyMetrics.TotalComments + 
                                          dailyMetrics.TotalShares;
            }
            
            // Métricas semanais
            var weeklyMetrics = await _metricsRepository.GetByDateRangeAsync(
                creator.Id,
                now.Date.AddDays(-7),
                now.Date);
                
            metrics.WeeklyEngagements += weeklyMetrics.Sum(m => 
                m.TotalLikes + m.TotalComments + m.TotalShares);
                
            // Métricas mensais
            var monthlyMetrics = await _metricsRepository.GetByDateRangeAsync(
                creator.Id,
                now.Date.AddMonths(-1),
                now.Date);
                
            metrics.MonthlyEngagements += monthlyMetrics.Sum(m => 
                m.TotalLikes + m.TotalComments + m.TotalShares);
        }
        
        // Calcular crescimento
        var previousMonthEngagements = await GetPreviousMonthEngagements(allCreators);
        metrics.EngagementGrowth = previousMonthEngagements > 0 
            ? ((decimal)(metrics.MonthlyEngagements - previousMonthEngagements) / previousMonthEngagements) * 100 
            : 100;
            
        return metrics;
    }

    private async Task<int> GetPreviousMonthEngagements(IEnumerable<ContentCreator> creators)
    {
        var now = DateTime.UtcNow;
        var previousMonthStart = now.Date.AddMonths(-2);
        var previousMonthEnd = now.Date.AddMonths(-1);
        var totalEngagements = 0;
        
        foreach (var creator in creators)
        {
            var metrics = await _metricsRepository.GetByDateRangeAsync(
                creator.Id,
                previousMonthStart,
                previousMonthEnd);
                
            totalEngagements += metrics.Sum(m => 
                m.TotalLikes + m.TotalComments + m.TotalShares);
        }
        
        return totalEngagements;
    }

    private async Task<decimal> CalculateEngagementRateAsync(Guid postId)
    {
        var post = await _contentRepository.GetByIdAsync(Guid.Parse(postId.ToString()));
        if (post == null || post.Views == 0)
            return 0;

        return ((decimal)(post.Likes + post.Comments + post.Shares) / post.Views) * 100;
    }

    private async Task<List<ContentPerformanceDto>> GetTopPerformingCreatorsAsync(int limit)
    {
        var creators = await _creatorRepository.GetAllAsync();
        var result = new List<ContentPerformanceDto>();
        
        foreach (var creator in creators)
        {
            var posts = await _contentRepository.GetByCreatorIdAsync(creator.Id);
            var totalViews = posts.Sum(p => p.Views);
            var totalLikes = posts.Sum(p => p.Likes);
            var totalComments = posts.Sum(p => p.Comments);
            
            if (totalViews > 0)
            {
                result.Add(new ContentPerformanceDto
                {
                    ContentId = creator.Id.ToString(),
                    Title = creator.Name,
                    CreatorName = creator.Name,
                    Views = (int)totalViews,
                    Likes = (int)totalLikes,
                    Comments = (int)totalComments,
                    EngagementRate = ((decimal)(totalLikes + totalComments) / totalViews) * 100
                });
            }
        }
        
        return result.OrderByDescending(c => c.Views).Take(limit).ToList();
    }
} 