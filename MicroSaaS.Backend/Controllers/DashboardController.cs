using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IPerformanceAnalysisService _performanceService;
    private readonly IDashboardInsightsRepository _insightsRepository;
    private readonly IPerformanceMetricsRepository _metricsRepository;
    private readonly IContentPerformanceRepository _contentPerformanceRepository;

    public DashboardController(
        IPerformanceAnalysisService performanceService, 
        IDashboardInsightsRepository insightsRepository,
        IPerformanceMetricsRepository metricsRepository,
        IContentPerformanceRepository contentPerformanceRepository)
    {
        _performanceService = performanceService;
        _insightsRepository = insightsRepository;
        _metricsRepository = metricsRepository;
        _contentPerformanceRepository = contentPerformanceRepository;
    }

    [HttpGet("insights/{creatorId}")]
    public async Task<ActionResult<DashboardInsights>> GetLatestInsights(Guid creatorId)
    {
        var insights = await _insightsRepository.GetLatestByCreatorIdAsync(creatorId);
        if (insights == null)
        {
            // Se não existir insights, geramos para os últimos 30 dias
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddDays(-30);
            
            insights = await _performanceService.GenerateInsightsAsync(creatorId, startDate, endDate);
        }
        
        return Ok(insights);
    }

    [HttpGet("insights/{creatorId}/generate")]
    public async Task<ActionResult<DashboardInsights>> GenerateInsights(
        Guid creatorId, 
        [FromQuery] DateTime? startDate = null, 
        [FromQuery] DateTime? endDate = null)
    {
        // Valores padrão se não fornecidos
        var end = endDate ?? DateTime.UtcNow;
        var start = startDate ?? end.AddDays(-30);
        
        var insights = await _performanceService.GenerateInsightsAsync(creatorId, start, end);
        return Ok(insights);
    }

    [HttpGet("metrics/{creatorId}")]
    public async Task<ActionResult<IEnumerable<PerformanceMetrics>>> GetMetrics(
        Guid creatorId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] SocialMediaPlatform? platform = null)
    {
        // Valores padrão se não fornecidos
        var end = endDate ?? DateTime.UtcNow;
        var start = startDate ?? end.AddDays(-30);
        
        var metrics = await _performanceService.GetMetricsTimelineAsync(creatorId, start, end, platform);
        return Ok(metrics);
    }

    [HttpGet("metrics/{creatorId}/daily")]
    public async Task<ActionResult<PerformanceMetrics>> GetDailyMetrics(
        Guid creatorId,
        [FromQuery] DateTime? date = null,
        [FromQuery] SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
    {
        var targetDate = date ?? DateTime.UtcNow.Date;
        var metrics = await _performanceService.GetDailyMetricsAsync(creatorId, platform, targetDate);
        return Ok(metrics);
    }

    [HttpGet("content/{creatorId}/top")]
    public async Task<ActionResult<List<ContentPost>>> GetTopContent(
        Guid creatorId,
        [FromQuery] int limit = 5)
    {
        var topContent = await _performanceService.GetTopPerformingContentAsync(creatorId, limit);
        return Ok(topContent);
    }

    [HttpGet("recommendations/{creatorId}/posting-times")]
    public async Task<ActionResult<List<MicroSaaS.Domain.Entities.PostTimeRecommendation>>> GetBestTimeToPost(
        Guid creatorId,
        [FromQuery] SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
    {
        var recommendations = await _performanceService.GetBestTimeToPostAsync(creatorId, platform);
        return Ok(recommendations);
    }

    [HttpGet("analytics/{creatorId}/engagement")]
    public async Task<ActionResult<decimal>> GetAverageEngagementRate(
        Guid creatorId,
        [FromQuery] SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
    {
        var engagementRate = await _performanceService.CalculateAverageEngagementRateAsync(creatorId, platform);
        return Ok(engagementRate);
    }

    [HttpGet("analytics/{creatorId}/revenue-growth")]
    public async Task<ActionResult<decimal>> GetRevenueGrowth(
        Guid creatorId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        // Valores padrão se não fornecidos
        var end = endDate ?? DateTime.UtcNow;
        var start = startDate ?? end.AddDays(-30);
        
        var growth = await _performanceService.CalculateRevenueGrowthAsync(creatorId, start, end);
        return Ok(growth);
    }

    [HttpGet("analytics/{creatorId}/follower-growth")]
    public async Task<ActionResult<int>> GetFollowerGrowth(
        Guid creatorId,
        [FromQuery] SocialMediaPlatform platform = SocialMediaPlatform.Instagram,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        // Valores padrão se não fornecidos
        var end = endDate ?? DateTime.UtcNow;
        var start = startDate ?? end.AddDays(-30);
        
        var growth = await _performanceService.CalculateFollowerGrowthAsync(creatorId, platform, start, end);
        return Ok(growth);
    }

    // Endpoints para adicionar e atualizar métricas de desempenho (para simulação/testes)
    [HttpPost("metrics")]
    public async Task<ActionResult<PerformanceMetrics>> AddMetrics([FromBody] PerformanceMetrics metrics)
    {
        metrics.Id = Guid.NewGuid();
        metrics.CreatedAt = DateTime.UtcNow;
        metrics.UpdatedAt = DateTime.UtcNow;
        
        var result = await _metricsRepository.AddAsync(metrics);
        return CreatedAtAction(nameof(GetDailyMetrics), new { creatorId = metrics.CreatorId, date = metrics.Date }, result);
    }

    [HttpPost("content-performance")]
    public async Task<ActionResult<ContentPerformance>> AddContentPerformance([FromBody] ContentPerformance performance)
    {
        performance.Id = Guid.NewGuid();
        performance.CollectedAt = DateTime.UtcNow;
        
        var result = await _contentPerformanceRepository.AddAsync(performance);
        return CreatedAtAction(nameof(GetTopContent), new { creatorId = result.Id }, result);
    }
} 