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
    private readonly IDashboardService _dashboardService;
    private readonly ITokenService _tokenService;

    public DashboardController(
        IDashboardService dashboardService,
        ITokenService tokenService)
    {
        _dashboardService = dashboardService;
        _tokenService = tokenService;
    }

    [HttpGet("insights/{creatorId}")]
    public async Task<ActionResult<DashboardInsights>> GetLatestInsights(Guid creatorId)
    {
        // Verificar autenticação
        var authHeader = HttpContext.Request.Headers["Authorization"].ToString();
        if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
        {
            var token = authHeader.Substring("Bearer ".Length).Trim();
            var isValidToken = _tokenService.ValidateToken(token);
            
            if (!isValidToken)
            {
                return Forbid();
            }
        }
        
        var insights = await _dashboardService.GetLatestInsightsAsync(creatorId);
        return Ok(insights);
    }

    [HttpGet("insights/{creatorId}/generate")]
    public async Task<ActionResult<DashboardInsights>> GenerateInsights(
        Guid creatorId, 
        [FromQuery] DateTime? startDate = null, 
        [FromQuery] DateTime? endDate = null)
    {
        var insights = await _dashboardService.GenerateInsightsAsync(creatorId, startDate, endDate);
        return Ok(insights);
    }

    [HttpGet("metrics/{creatorId}")]
    public async Task<ActionResult<IEnumerable<PerformanceMetrics>>> GetMetrics(
        Guid creatorId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] SocialMediaPlatform? platform = null)
    {
        var metrics = await _dashboardService.GetMetricsAsync(creatorId, startDate, endDate, platform);
        return Ok(metrics);
    }

    [HttpGet("metrics/{creatorId}/daily")]
    public async Task<ActionResult<PerformanceMetrics>> GetDailyMetrics(
        Guid creatorId,
        [FromQuery] DateTime? date = null,
        [FromQuery] SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
    {
        var metrics = await _dashboardService.GetDailyMetricsAsync(creatorId, date, platform);
        return Ok(metrics);
    }

    [HttpGet("content/{creatorId}/top")]
    public async Task<ActionResult<List<ContentPost>>> GetTopContent(
        Guid creatorId,
        [FromQuery] int limit = 5)
    {
        var topContent = await _dashboardService.GetTopContentAsync(creatorId, limit);
        return Ok(topContent);
    }

    [HttpGet("recommendations/{creatorId}/posting-times")]
    public async Task<ActionResult<List<MicroSaaS.Domain.Entities.PostTimeRecommendation>>> GetBestTimeToPost(
        Guid creatorId,
        [FromQuery] SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
    {
        var recommendations = await _dashboardService.GetBestTimeToPostAsync(creatorId, platform);
        return Ok(recommendations);
    }

    [HttpGet("analytics/{creatorId}/engagement")]
    public async Task<ActionResult<decimal>> GetAverageEngagementRate(
        Guid creatorId,
        [FromQuery] SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
    {
        var engagementRate = await _dashboardService.GetAverageEngagementRateAsync(creatorId, platform);
        return Ok(engagementRate);
    }

    [HttpGet("analytics/{creatorId}/revenue-growth")]
    public async Task<ActionResult<decimal>> GetRevenueGrowth(
        Guid creatorId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var growth = await _dashboardService.GetRevenueGrowthAsync(creatorId, startDate, endDate);
        return Ok(growth);
    }

    [HttpGet("analytics/{creatorId}/follower-growth")]
    public async Task<ActionResult<int>> GetFollowerGrowth(
        Guid creatorId,
        [FromQuery] SocialMediaPlatform platform = SocialMediaPlatform.Instagram,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var growth = await _dashboardService.GetFollowerGrowthAsync(creatorId, platform, startDate, endDate);
        return Ok(growth);
    }

    [HttpPost("metrics")]
    public async Task<ActionResult<PerformanceMetrics>> AddMetrics([FromBody] PerformanceMetrics metrics)
    {
        var result = await _dashboardService.AddMetricsAsync(metrics);
        return CreatedAtAction(nameof(GetDailyMetrics), new { creatorId = metrics.CreatorId, date = metrics.Date }, result);
    }

    [HttpPost("content-performance")]
    public async Task<ActionResult<ContentPerformance>> AddContentPerformance([FromBody] ContentPerformance performance)
    {
        var result = await _dashboardService.AddContentPerformanceAsync(performance);
        return CreatedAtAction(nameof(GetTopContent), new { creatorId = performance.CreatorId }, result);
    }
} 