using MicroSaaS.Application.DTOs.Performance;
using MicroSaaS.Application.Interfaces.Repositories; // Corrigido para usar interfaces da Application
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities; // Dependência do Domínio para Entidades
using MicroSaaS.Shared.Enums;
using System; // Adicionado using System
using System.Collections.Generic; // Adicionado using
using System.Linq; // Adicionado using
using System.Threading.Tasks; // Adicionado using

namespace MicroSaaS.Application.Services; // Namespace corrigido para Application

public class PerformanceAnalysisService : IPerformanceAnalysisService
{
    // Dependências injetadas como interfaces da Application (ou Domain onde apropriado)
    private readonly IContentPostRepository _contentRepository;
    private readonly IContentCreatorRepository _creatorRepository;
    private readonly ISocialMediaAccountRepository _socialMediaRepository;
    private readonly IPerformanceMetricsRepository _metricsRepository;
    private readonly IRevenueService _revenueService; // Interface de Serviço da Application
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
        _contentRepository = contentRepository ?? throw new ArgumentNullException(nameof(contentRepository));
        _creatorRepository = creatorRepository ?? throw new ArgumentNullException(nameof(creatorRepository));
        _socialMediaRepository = socialMediaRepository ?? throw new ArgumentNullException(nameof(socialMediaRepository));
        _metricsRepository = metricsRepository ?? throw new ArgumentNullException(nameof(metricsRepository));
        _revenueService = revenueService ?? throw new ArgumentNullException(nameof(revenueService));
        _contentPerformanceRepository = contentPerformanceRepository ?? throw new ArgumentNullException(nameof(contentPerformanceRepository));
        _insightsRepository = insightsRepository ?? throw new ArgumentNullException(nameof(insightsRepository));
    }

    public async Task<DashboardMetricsDto> GetDashboardMetricsAsync()
    {
        var creators = await _creatorRepository.GetAllAsync();
        var metrics = new DashboardMetricsDto
        {
            TotalCreators = creators.Count(),
            // Note: TotalPosts calculation might be inefficient if Posts is lazy loaded. Consider a dedicated count method.
            TotalPosts = creators.Sum(c => c.Posts?.Count ?? 0)
        };

        var creatorPerformances = new List<CreatorPerformanceDto>();
        foreach (var creator in creators)
        {
            // Assuming Creator entity fetched via GetAllAsync includes Posts. If not, this needs adjustment.
            var posts = creator.Posts ?? new List<ContentPost>();
            long totalViewsLong = posts.Sum(p => p.Views);
            long totalLikesLong = posts.Sum(p => p.Likes);
            long totalCommentsLong = posts.Sum(p => p.Comments);

            // Potential overflow if sums exceed int.MaxValue. Consider using long in DTO if necessary.
            int totalViews = (int)totalViewsLong;
            int totalLikes = (int)totalLikesLong;
            int totalComments = (int)totalCommentsLong;

            if (totalViews > 0) // Avoid division by zero or meaningless engagement rate
            {
                creatorPerformances.Add(new CreatorPerformanceDto
                {
                    CreatorId = creator.Id,
                    Name = creator.Name,
                    Views = totalViews,
                    Likes = totalLikes,
                    Comments = totalComments,
                    // Assuming GetAverageEngagementRateAsync exists and works correctly
                    EngagementRate = await _contentPerformanceRepository.GetAverageEngagementRateAsync(creator.Id)
                });
            }
        }

        metrics.TopCreators = creatorPerformances
            .OrderByDescending(c => c.Views)
            .Take(5)
            .ToList();

        // Calculating average across all creators might be more meaningful than just the first?
        metrics.AverageEngagementRate = await _contentPerformanceRepository.GetAverageEngagementRateAsync(creators.FirstOrDefault()?.Id ?? Guid.Empty);

        return metrics;
    }

    public async Task<ContentMetricsDto> GetContentMetricsAsync(string contentId)
    {
        if (string.IsNullOrEmpty(contentId))
            throw new ArgumentException("O ID do conteúdo não pode ser nulo ou vazio", nameof(contentId));

        if (!Guid.TryParse(contentId, out Guid contentGuid))
            throw new ArgumentException("O ID do conteúdo deve ser um GUID válido", nameof(contentId));

        return await GetContentMetricsAsync(contentGuid);
    }

    public async Task<ContentMetricsDto> GetContentMetricsAsync(Guid contentId)
    {
        var result = new ContentMetricsDto
        {
            ContentId = contentId,
            // Default values
            Views = 0,
            Likes = 0,
            Comments = 0,
            Shares = 0,
            EngagementRate = 0,
            Revenue = 0 // Usamos a propriedade Revenue em vez de EstimatedRevenue
        };

        // Aqui seria a lógica real para buscar métricas do banco de dados
        // por enquanto, geramos dados de teste

        var rand = new Random();
        result.Views = rand.Next(500, 50000);
        result.Likes = (int)(result.Views * rand.NextDouble() * 0.2); // Up to 20% of views
        result.Comments = (int)(result.Likes * rand.NextDouble() * 0.1); // Up to 10% of likes
        result.Shares = (int)(result.Likes * rand.NextDouble() * 0.05); // Up to 5% of likes
        result.EngagementRate = (decimal)((result.Likes + result.Comments + result.Shares) / (double)result.Views * 100);
        result.Revenue = Math.Round((decimal)(result.Views * 0.002 * (rand.NextDouble() + 0.5)), 2); // $0.001-0.003 per view
        // Não existe a propriedade AudienceRetention no DTO, então removemos

        return result;
    }

    public async Task<CreatorMetricsDto> GetCreatorMetricsAsync(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            throw new KeyNotFoundException($"Criador não encontrado: {creatorId}");

        var metrics = new CreatorMetricsDto
        {
            CreatorId = creator.Id,
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

    // This method seems more like a coordinator. Is there specific analysis logic here?
    // Or should it call specific analysis methods?
    public async Task RefreshMetricsAsync()
    {
        // These calls seem appropriate for an Application service to orchestrate.
        await _metricsRepository.RefreshMetricsAsync();
        await _revenueService.RefreshRevenueMetricsAsync(); // Assuming IRevenueService has this
        await _socialMediaRepository.RefreshSocialMediaMetricsAsync();
    }

    public async Task<PerformanceMetrics> GetDailyMetricsAsync(Guid creatorId, SocialMediaPlatform platform, DateTime date)
    {
        var metrics = await _metricsRepository.GetByCreatorAndDateAsync(creatorId, date, platform);

        if (metrics == null)
        {
            // Returning a new empty metric seems reasonable for non-existent data points.
            metrics = new PerformanceMetrics
            {
                CreatorId = creatorId,
                Platform = platform,
                Date = date.Date, // Ensure date only, no time component
                Followers = 0,
                FollowersGrowth = 0,
                TotalViews = 0,
                TotalLikes = 0,
                TotalComments = 0,
                TotalShares = 0,
                EngagementRate = 0,
                EstimatedRevenue = 0,
                TopPerformingContentIds = new List<Guid>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            // Consider if this 'empty' metric should be saved or just returned transiently.
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
        // Check if insights for this exact period already exist
        var existingInsights = await _insightsRepository.GetByCreatorAndPeriodAsync(creatorId, startDate, endDate);
        if (existingInsights != null)
        {
            return existingInsights; // Return existing if found
        }

        // Fetch necessary data
        var metrics = await _metricsRepository.GetByDateRangeAsync(creatorId, startDate, endDate);
        var platforms = metrics.Select(m => m.Platform).Distinct().ToList();

        var performances = await _contentPerformanceRepository.GetByCreatorIdAsync(creatorId);
        var periodPerformances = performances
            .Where(p => p.Date >= startDate && p.Date <= endDate) // Assuming ContentPerformance has a Date
            .ToList();

        // Perform calculations
        var growthRate = CalculateFollowerGrowthRate(metrics); // Renamed for clarity
        var totalRevenue = periodPerformances.Sum(p => p.EstimatedRevenue);

        var topPerformingPeriodPerformances = periodPerformances
            .OrderByDescending(p => p.Views)
            .Take(5)
            .ToList();

        var contentInsights = new List<ContentInsight>();
        foreach (var performance in topPerformingPeriodPerformances)
        {
            // Consider fetching posts in bulk if performance is high
            var post = await _contentRepository.GetByIdAsync(Guid.Parse(performance.PostId.ToString()));
            if (post != null)
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
                     InsightType = DetermineInsightType(performance), // Placeholder logic
                     Insight = GenerateInsightText(performance, post) // Placeholder logic
                 });
            }
        }

        // Generate recommendations (placeholder logic)
        var recommendations = GenerateRecommendations(periodPerformances, platforms);
        var bestTimes = await GetBestTimeToPostAsync(creatorId, platforms.FirstOrDefault()); // Placeholder logic

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
            ComparisonWithPreviousPeriod = await CalculateComparisonWithPreviousPeriod(creatorId, startDate, endDate), // Placeholder
            TopContentInsights = contentInsights,
            Recommendations = recommendations,
            BestTimeToPost = bestTimes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Save the newly generated insights
        await _insightsRepository.AddAsync(insights);
        return insights;
    }

    // --- Helper / Private methods ---
    // These methods contain the core analysis/calculation logic and belong here in the Application Service.

    public async Task<List<ContentPost>> GetTopPerformingContentAsync(Guid creatorId, int limit = 5)
    {
        var topPerformances = await _contentPerformanceRepository.GetTopPerformingByViewsAsync(creatorId, limit);
        var result = new List<ContentPost>();

        // Consider fetching posts in bulk by ID list for efficiency
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

    // Placeholder implementation - replace with actual analysis
    public async Task<List<PostTimeRecommendation>> GetBestTimeToPostAsync(Guid creatorId, SocialMediaPlatform platform)
    {
        // TODO: Implement actual logic based on performance data analysis for the specific platform
        await Task.Delay(10); // Simulate async work
        var recommendations = new List<PostTimeRecommendation>
        {
            new PostTimeRecommendation { DayOfWeek = DayOfWeek.Monday, TimeOfDay = new TimeSpan(18, 0, 0), EngagementScore = 8.5 },
            new PostTimeRecommendation { DayOfWeek = DayOfWeek.Wednesday, TimeOfDay = new TimeSpan(12, 0, 0), EngagementScore = 9.0 },
        };

        return recommendations;
    }

    public async Task<decimal> CalculateAverageEngagementRateAsync(Guid creatorId, SocialMediaPlatform platform)
    {
        var performances = await _contentPerformanceRepository.GetByCreatorIdAsync(creatorId);

        // Filtrar pela plataforma específica
        var filteredPerformances = performances.Where(p => p.Platform == platform);

        var performanceList = filteredPerformances.ToList();

        if (!performanceList.Any())
        {
            return 0;
        }

        // Ensure CalculateEngagementRate handles potential division by zero
        var totalEngagementRateSum = performanceList.Sum(p => CalculateEngagementRate(p));
        return totalEngagementRateSum / performanceList.Count;
    }

    public async Task<decimal> CalculateRevenueGrowthAsync(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        var currentPeriodRevenue = await _revenueService.GetTotalRevenueAsync(creatorId, startDate, endDate);

        var previousPeriodLength = endDate - startDate;
        // Handle edge case where start and end date are the same
        if (previousPeriodLength <= TimeSpan.Zero) return 0;

        var previousPeriodStart = startDate - previousPeriodLength;
        var previousPeriodEnd = startDate.AddTicks(-1); // End just before the current period starts

        var previousPeriodRevenue = await _revenueService.GetTotalRevenueAsync(creatorId, previousPeriodStart, previousPeriodEnd);

        if (previousPeriodRevenue == 0)
        {
            // If current revenue is also 0, growth is 0. If current is positive, it's infinite (represent as 100% or specific value?)
            return currentPeriodRevenue > 0 ? 100m : 0m;
        }

        return ((currentPeriodRevenue - previousPeriodRevenue) / previousPeriodRevenue) * 100;
    }

    public async Task<int> CalculateFollowerGrowthAsync(Guid creatorId, SocialMediaPlatform platform, DateTime startDate, DateTime endDate)
    {
        // Fetch metrics for the specific platform within the broader range needed
        var startMetrics = await _metricsRepository.GetByCreatorAndDateAsync(creatorId, startDate.Date, platform);
        var endMetrics = await _metricsRepository.GetByCreatorAndDateAsync(creatorId, endDate.Date, platform);

        // Ensure we have data for both start and end dates
        if (startMetrics == null || endMetrics == null)
            return 0;

        return endMetrics.Followers - startMetrics.Followers;
    }

    private decimal CalculateEngagementRate(ContentPerformance performance)
    {
        if (performance == null || performance.Views <= 0) // Check for null and non-positive views
            return 0;

        var totalEngagements = performance.Likes + performance.Comments + performance.Shares;
        // Use decimal for calculation to avoid integer division issues
        return ((decimal)totalEngagements / performance.Views) * 100;
    }

    private decimal CalculateFollowerGrowthRate(IEnumerable<PerformanceMetrics> metrics) // Renamed for clarity
    {
        if (metrics == null || !metrics.Any())
            return 0;

        var orderedMetrics = metrics.OrderBy(m => m.Date).ToList();
        var firstMetrics = orderedMetrics.First();
        var lastMetrics = orderedMetrics.Last();

        // Use the earliest follower count as the base
        var initialFollowers = firstMetrics.Followers;

        if (initialFollowers <= 0) // Handle zero or negative initial followers
        {
            // If final followers > 0, growth is infinite/undefined, represent as 100%? Or 0? Depends on business rule.
            return lastMetrics.Followers > 0 ? 100m : 0m;
        }

        // Calculate growth based on the difference relative to the initial count
        return ((decimal)(lastMetrics.Followers - initialFollowers) / initialFollowers) * 100;
    }

    // Placeholder logic
    private InsightType DetermineInsightType(ContentPerformance performance)
    {
        var engagementRate = CalculateEngagementRate(performance);

        if (engagementRate > 10) return InsightType.HighEngagement;
        if (performance.Views > 10000) return InsightType.HighReach;
        if (performance.EstimatedRevenue > 50) return InsightType.HighRevenue; // Adjusted threshold
        return InsightType.Normal;
    }

    // Placeholder logic
    private string GenerateInsightText(ContentPerformance performance, ContentPost post)
    {
        var engagementRate = CalculateEngagementRate(performance);
        return $"O post '{post?.Title ?? "N/A"}' teve taxa de engajamento de {engagementRate:F1}% ({performance.Views} views). Receita estimada: R${performance.EstimatedRevenue:F2}.";
    }

    // Placeholder logic
    private List<ContentRecommendation> GenerateRecommendations(
        List<ContentPerformance> performances,
        List<SocialMediaPlatform> platforms)
    {
        // TODO: Implement real recommendation logic based on performance analysis
        return new List<ContentRecommendation>
        {
            new ContentRecommendation { Type = RecommendationType.ContentFormat, Platform = platforms.FirstOrDefault(), Description = "Considere usar mais vídeos curtos." },
            new ContentRecommendation { Type = RecommendationType.Topic, Platform = platforms.FirstOrDefault(), Description = "Tópicos sobre IA estão performando bem." }
        };
    }

     // Placeholder - Calculate actual comparison
    private async Task<Dictionary<string, decimal>> CalculateComparisonWithPreviousPeriod(Guid creatorId, DateTime startDate, DateTime endDate)
    {
        await Task.Delay(10); // Simulate work
        // TODO: Fetch data for previous period and calculate comparison metrics (e.g., engagement change by platform)
        return new Dictionary<string, decimal>(); // Retorna dicionário vazio por enquanto
    }


    // --- Methods likely belonging solely to Infrastructure or needing adjustment ---
    // These seem like they might belong in Infrastructure or need careful review
    // if they contain Application logic vs simple data retrieval formatting.

    private async Task<List<PlatformMetricsDto>> GetPlatformMetricsAsync(ContentPost content)
    {
        // Fetches performance data - ok for Application service
        var performances = await _contentPerformanceRepository.GetByPostIdAsync(content.Id.ToString());

        // Transforms data into DTO - this calculation/transformation logic is Application logic
        return performances.Select(p => new PlatformMetricsDto
        {
            Platform = p.Platform.ToString(),
            Views = (int)p.Views,
            Likes = (int)p.Likes,
            Comments = (int)p.Comments,
            Shares = (int)p.Shares,
            EngagementRate = CalculateEngagementRate(p) // Calculation belongs here
        }).ToList();
    }

    private async Task<List<DailyMetricsDto>> GetDailyMetricsAsync(ContentPost content)
    {
        // Fetches performance data - ok
        var performances = await _contentPerformanceRepository.GetByPostIdAsync(content.Id.ToString());

        // Transforms data into DTO, including calculation - Application logic
        return performances
            .OrderBy(p => p.Date)
            .Select(p => new DailyMetricsDto
            {
                Date = p.Date,
                Views = (int)p.Views,
                Likes = (int)p.Likes,
                Comments = (int)p.Comments,
                Shares = (int)p.Shares,
                EngagementRate = CalculateEngagementRate(p), // Calculation belongs here
                Revenue = p.EstimatedRevenue
            }).ToList();
    }

    private async Task<List<PlatformFollowersDto>> GetPlatformFollowersAsync(ContentCreator creator)
    {
        // Fetches accounts - ok
        var accounts = await _socialMediaRepository.GetByCreatorIdAsync(creator.Id);
        var result = new List<PlatformFollowersDto>();

        foreach (var account in accounts)
        {
            // Fetches metrics for platform - ok
            var metrics = await _metricsRepository.GetByCreatorAndPlatformAsync(creator.Id, account.Platform);
            var latestMetrics = metrics.OrderByDescending(m => m.Date).FirstOrDefault();

            if (latestMetrics != null)
            {
                // Calculates growth rate - Application logic
                var growthRate = latestMetrics.Followers > 0
                    ? ((decimal)latestMetrics.FollowersGrowth / latestMetrics.Followers) * 100
                    : (latestMetrics.FollowersGrowth > 0 ? 100m : 0m); // Handle 0 initial followers

                result.Add(new PlatformFollowersDto
                {
                    Platform = account.Platform.ToString(),
                    Followers = latestMetrics.Followers,
                    Growth = latestMetrics.FollowersGrowth,
                    GrowthRate = growthRate // Calculation belongs here
                });
            }
            // Consider adding an entry with 0 followers if latestMetrics is null
        }

        return result;
    }

    private async Task<List<MonthlyRevenueDto>> GetMonthlyRevenueAsync(ContentCreator creator)
    {
        var endDate = DateTime.UtcNow;
        var startDate = endDate.AddMonths(-12); // Get data for the last 12 months

        // Fetch daily revenue - ok
        var dailyRevenue = await _revenueService.GetRevenueByDayAsync(creator.Id, startDate, endDate);

        // Group and sum by month - Application logic
        var monthlyData = dailyRevenue
            .GroupBy(r => new DateTime(r.Date.Year, r.Date.Month, 1))
            .Select(g => new
            {
                Month = g.Key,
                Revenue = g.Sum(r => r.Amount)
            })
            .OrderBy(m => m.Month)
            .ToList();

        // Calculate growth rate - Application logic
        var result = new List<MonthlyRevenueDto>();
        for(int i = 0; i < monthlyData.Count; i++)
        {
             var currentMonth = monthlyData[i];
             decimal previousMonthRevenue = (i > 0) ? monthlyData[i-1].Revenue : 0;
             decimal growth = currentMonth.Revenue - previousMonthRevenue;
             decimal growthRate = 0m;
             if (previousMonthRevenue != 0)
             {
                 growthRate = (growth / previousMonthRevenue) * 100m;
             }
             else if (currentMonth.Revenue > 0)
             {
                 // Se a receita anterior era 0 e a atual é > 0, o crescimento é infinito, representamos como 100%?
                 // Ou talvez retornar um valor especial ou null? Por enquanto, 100%.
                 growthRate = 100m;
             }

             result.Add(new MonthlyRevenueDto
             {
                 Month = currentMonth.Month,
                 Revenue = currentMonth.Revenue,
                 Growth = growth,
                 GrowthRate = growthRate
             });
        }

        return result;
    }

    private async Task<List<ContentPerformanceSummaryDto>> GetTopPerformingContentByCreatorAsync(Guid creatorId, int limit)
    {
        // Fetch top performance records - ok
        var performances = await _contentPerformanceRepository.GetTopPerformingByViewsAsync(creatorId, limit);
        var result = new List<ContentPerformanceSummaryDto>();

        var postIds = performances.Select(p => Guid.Parse(p.PostId.ToString())).ToList();
        // Fetch posts in bulk if possible
        var posts = await _contentRepository.GetByIdsAsync(postIds); // Assuming this method exists
        var postDict = posts.ToDictionary(p => p.Id);

        foreach (var performance in performances)
        {
            if (postDict.TryGetValue(Guid.Parse(performance.PostId.ToString()), out var post))
            {
                 result.Add(new ContentPerformanceSummaryDto
                 {
                     ContentId = post.Id.ToString(),
                     Title = post.Title,
                     CreatorName = await GetCreatorNameAsync(post.CreatorId),
                     Views = (int)performance.Views,
                     Likes = (int)performance.Likes,
                     Comments = (int)performance.Comments,
                     EngagementRate = CalculateEngagementRate(performance) // Calculation belongs here
                 });
            }
        }

        return result;
    }

    // --- Methods below seem highly suspect for Application layer ---
    // These aggregate across ALL creators or perform very broad calculations
    // that might be better suited for a dedicated reporting/aggregation service,
    // possibly even running as a background job, rather than a core Application service.
    // Or they might belong in specific Repository implementations if they are complex queries.

    // Consider if these cross-creator aggregations belong here.
    private async Task<RevenueMetricsDto> GetRevenueMetricsAsync()
    {
        // This fetches ALL creators and iterates, calculating revenue.
        // This could be very resource intensive for a synchronous request.
        // Suggest moving to a background task or reporting module.
        var now = DateTime.UtcNow;
        var allCreators = await _creatorRepository.GetAllAsync();
        var totalMonthlyRevenue = 0m;

        foreach (var creator in allCreators)
        {
            totalMonthlyRevenue += await _revenueService.GetTotalRevenueAsync(
                creator.Id,
                now.Date.AddMonths(-1),
                now.Date);
        }

        var previousMonthRevenue = await GetPreviousMonthRevenue_Aggregated(allCreators); // Helper for aggregation
        var revenueGrowth = previousMonthRevenue > 0
            ? ((totalMonthlyRevenue - previousMonthRevenue) / previousMonthRevenue) * 100
            : (totalMonthlyRevenue > 0 ? 100m : 0m);

        return new RevenueMetricsDto
        {
            DailyRevenue = await GetTotalDailyRevenue_Aggregated(allCreators),
            WeeklyRevenue = await GetTotalWeeklyRevenue_Aggregated(allCreators),
            MonthlyRevenue = totalMonthlyRevenue,
            RevenueGrowth = revenueGrowth
        };
    }

    // Aggregation helper - consider implications
    private async Task<decimal> GetPreviousMonthRevenue_Aggregated(IEnumerable<ContentCreator> creators)
    {
        var now = DateTime.UtcNow;
        // Corrected previous month calculation
        var previousMonthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
        var previousMonthEnd = new DateTime(now.Year, now.Month, 1).AddTicks(-1);
        var totalRevenue = 0m;

        // Consider fetching revenue in bulk if possible
        foreach (var creator in creators)
        {
            totalRevenue += await _revenueService.GetTotalRevenueAsync(
                creator.Id,
                previousMonthStart,
                previousMonthEnd);
        }
        return totalRevenue;
    }

    // Aggregation helper - consider implications
    private async Task<decimal> GetTotalDailyRevenue_Aggregated(IEnumerable<ContentCreator> creators)
    {
         var now = DateTime.UtcNow;
         var totalRevenue = 0m;
         foreach (var creator in creators)
         {
             totalRevenue += await _revenueService.GetTotalRevenueAsync(
                 creator.Id,
                 now.Date, // Start of today
                 now.Date.AddDays(1).AddTicks(-1)); // End of today
         }
         return totalRevenue;
    }

    // Aggregation helper - consider implications
    private async Task<decimal> GetTotalWeeklyRevenue_Aggregated(IEnumerable<ContentCreator> creators)
    {
         var now = DateTime.UtcNow;
         var weekStart = now.Date.AddDays(-(int)now.DayOfWeek); // Assuming Sunday is start
         var weekEnd = weekStart.AddDays(7).AddTicks(-1);
         var totalRevenue = 0m;
         foreach (var creator in creators)
         {
             totalRevenue += await _revenueService.GetTotalRevenueAsync(
                 creator.Id,
                 weekStart,
                 weekEnd);
         }
         return totalRevenue;
    }


    // Aggregation across all creators - consider performance/placement
    private async Task<EngagementMetricsDto> GetEngagementMetricsAsync()
    {
        var now = DateTime.UtcNow;
        var allCreators = await _creatorRepository.GetAllAsync();
        var metrics = new EngagementMetricsDto();

        long monthlyEngagements = 0;

        // These loops over all creators fetching metrics could be slow.
        // Consider aggregation at the database level if possible or background jobs.
        foreach (var creator in allCreators)
        {
             // Daily - Example for one creator, repeat for others
            var dailyMetrics = await _metricsRepository.GetByCreatorAndDateAsync(
                creator.Id,
                now.Date,
                SocialMediaPlatform.Instagram); // Alterado de null para valor padrão
            if (dailyMetrics != null)
            {
                metrics.DailyEngagements += dailyMetrics.TotalLikes + dailyMetrics.TotalComments + dailyMetrics.TotalShares;
            }

            // Weekly - Example range
            var weekStart = now.Date.AddDays(-(int)now.DayOfWeek);
            var weeklyMetrics = await _metricsRepository.GetByDateRangeAsync(creator.Id, weekStart, now.Date);
            metrics.WeeklyEngagements += weeklyMetrics.Sum(m => m.TotalLikes + m.TotalComments + m.TotalShares);

            // Monthly - Example range
            var monthStart = new DateTime(now.Year, now.Month, 1);
            var monthlyMetrics = await _metricsRepository.GetByDateRangeAsync(creator.Id, monthStart, now.Date);
            monthlyEngagements += monthlyMetrics.Sum(m => m.TotalLikes + m.TotalComments + m.TotalShares);
        }

        // metrics.MonthlyEngagements = monthlyEngagements; // Atribuição movida para após o cálculo

        var previousMonthEngagements = await GetPreviousMonthEngagements_Aggregated(allCreators); // Aggregation helper
        var engagementGrowth = previousMonthEngagements > 0
            ? ((decimal)(monthlyEngagements - previousMonthEngagements) / previousMonthEngagements) * 100
            : (monthlyEngagements > 0 ? 100m : 0m);

        // Atualiza o DTO após todos os cálculos
        metrics.MonthlyEngagements = (int)monthlyEngagements; // Cast para int aqui
        metrics.EngagementGrowth = engagementGrowth;

        return metrics;
    }

     // Aggregation helper - consider performance/placement
    private async Task<long> GetPreviousMonthEngagements_Aggregated(IEnumerable<ContentCreator> creators)
    {
        var now = DateTime.UtcNow;
        var previousMonthStart = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
        var previousMonthEnd = new DateTime(now.Year, now.Month, 1).AddTicks(-1);
        long totalEngagements = 0;

        foreach (var creator in creators)
        {
            var metrics = await _metricsRepository.GetByDateRangeAsync(
                creator.Id,
                previousMonthStart,
                previousMonthEnd);
            totalEngagements += metrics.Sum(m => (long)m.TotalLikes + m.TotalComments + m.TotalShares); // Use long for sum
        }
        return totalEngagements;
    }


    // This calculation might be redundant if CalculateEngagementRate(ContentPerformance) exists and is preferred.
    private async Task<decimal> CalculateEngagementRateAsync(Guid postId)
    {
        // Fetching post just for this calculation seems inefficient if performance data is available.
        var post = await _contentRepository.GetByIdAsync(postId);
        if (post == null || post.Views <= 0)
            return 0;

        var totalEngagements = post.Likes + post.Comments + post.Shares;
        return ((decimal)totalEngagements / post.Views) * 100;
    }


    // Aggregation across creators - consider performance/placement
    private async Task<List<ContentPerformanceSummaryDto>> GetTopPerformingCreatorsAsync(int limit)
    {
        // Fetches ALL creators and ALL their posts - potentially very inefficient.
        // This logic is highly suspect for a real-time Application service request.
        // Should likely be a specialized query in a repository or a background task result.
        var creators = await _creatorRepository.GetAllAsync();
        var result = new List<ContentPerformanceSummaryDto>();

        foreach (var creator in creators)
        {
            var posts = await _contentRepository.GetByCreatorIdAsync(creator.Id); // Fetch posts per creator
            if (posts == null || !posts.Any()) continue;

            long totalViews = posts.Sum(p => p.Views);
            long totalLikes = posts.Sum(p => p.Likes);
            long totalComments = posts.Sum(p => p.Comments);

            if (totalViews > 0)
            {
                 var engagementRate = ((decimal)(totalLikes + totalComments) / totalViews) * 100;
                 result.Add(new ContentPerformanceSummaryDto
                 {
                     ContentId = creator.Id.ToString(), // Using CreatorId as ContentId? Seems wrong.
                     Title = creator.Name, // Using Creator Name as Title? Seems wrong.
                     CreatorName = creator.Name,
                     Views = (int)totalViews,
                     Likes = (int)totalLikes,
                     Comments = (int)totalComments,
                     EngagementRate = engagementRate
                 });
            }
        }

        // Order by views and take limit
        return result.OrderByDescending(c => c.Views).Take(limit).ToList();
    }

    // Novo método helper para buscar o nome do criador
    private async Task<string> GetCreatorNameAsync(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        return creator?.Name ?? "Desconhecido";
    }
}