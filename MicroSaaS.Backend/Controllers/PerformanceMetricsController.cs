using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Backend.Attributes;

namespace MicroSaaS.Backend.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PerformanceMetricsController : ControllerBase
{
    private readonly IPerformanceAnalysisService _performanceService;
    private readonly ICacheService _cacheService;

    public PerformanceMetricsController(
        IPerformanceAnalysisService performanceService,
        ICacheService cacheService)
    {
        _performanceService = performanceService;
        _cacheService = cacheService;
    }

    [HttpGet("dashboard")]
    [Cache("performance:dashboard", minutes: 5)]
    public async Task<IActionResult> GetDashboardMetrics()
    {
        var metrics = await _performanceService.GetDashboardMetricsAsync();
        return Ok(metrics);
    }

    [HttpGet("content/{contentId}")]
    [Cache("performance:content:{contentId}", minutes: 15)]
    public async Task<IActionResult> GetContentMetrics(string contentId)
    {
        var metrics = await _performanceService.GetContentMetricsAsync(contentId);
        return Ok(metrics);
    }

    [HttpGet("creator/{creatorId}")]
    [Cache("performance:creator:{creatorId}", minutes: 30)]
    public async Task<IActionResult> GetCreatorMetrics(string creatorId)
    {
        var metrics = await _performanceService.GetCreatorMetricsAsync(creatorId);
        return Ok(metrics);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshMetrics()
    {
        // Limpar cache ao forçar atualização
        await _cacheService.RemoveAsync("performance:dashboard");
        await _performanceService.RefreshMetricsAsync();
        return Ok();
    }
} 