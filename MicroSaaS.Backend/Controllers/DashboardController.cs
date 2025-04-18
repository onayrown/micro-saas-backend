using MicroSaaS.Application.DTOs;
using MicroSaaS.Application.DTOs.Performance;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ContentPostDto = MicroSaaS.Application.DTOs.ContentPost.ContentPostDto;

namespace MicroSaaS.Backend.Controllers;

/// <summary>
/// Controlador responsável pelas operações relacionadas ao dashboard
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
//[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<DashboardController> _logger;

    /// <summary>
    /// Construtor do DashboardController
    /// </summary>
    /// <param name="dashboardService">Serviço de dashboard</param>
    /// <param name="tokenService">Serviço de validação de tokens</param>
    /// <param name="logger">Logger para registro de erros</param>
    public DashboardController(
        IDashboardService dashboardService,
        ITokenService tokenService,
        ILogger<DashboardController> logger)
    {
        _dashboardService = dashboardService;
        _tokenService = tokenService;
        _logger = logger;
    }

    /// <summary>
    /// Obtém as insights mais recentes do dashboard para um criador específico
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <returns>Insights do dashboard</returns>
    /// <response code="200">Insights recuperados com sucesso</response>
    /// <response code="403">Token inválido ou expirado</response>
    /// <response code="404">Criador não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("insights/{creatorId}")]
    [ProducesResponseType(typeof(DashboardInsightsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DashboardInsightsDto>> GetLatestInsights(Guid creatorId)
    {
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || userIdClaim.Value != creatorId.ToString())
        {
            return Forbid("Você não tem permissão para acessar insights deste criador.");
        }
        try
        {
            var insightsDto = await _dashboardService.GetLatestInsightsAsync(creatorId);
            if (insightsDto == null)
            {
                return NotFound("Nenhum insight encontrado para este criador.");
            }
            return Ok(insightsDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar insights para o criador {CreatorId}", creatorId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao buscar insights.");
        }
    }

    /// <summary>
    /// Gera novos insights para o dashboard de um criador específico
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <returns>Insights recém-gerados do dashboard</returns>
    /// <response code="200">Insights gerados com sucesso</response>
    /// <response code="403">Usuário não autorizado</response>
    /// <response code="404">Criador não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("insights/{creatorId}/generate")]
    [ProducesResponseType(typeof(DashboardInsightsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DashboardInsightsDto>> GenerateInsights(Guid creatorId)
    {
        var insightsDto = await _dashboardService.GenerateInsightsAsync(creatorId);
        if (insightsDto == null)
        {
            return NotFound("Não foi possível gerar insights para este criador.");
        }
        return Ok(insightsDto);
    }

    [HttpGet("metrics/{creatorId}")]
    public async Task<ActionResult<IEnumerable<DashboardMetricsDto>>> GetMetrics(
        Guid creatorId,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] SocialMediaPlatform? platform = null)
    {
        var metrics = await _dashboardService.GetMetricsAsync(creatorId, startDate, endDate, platform);
        return Ok(metrics);
    }

    [HttpGet("metrics/{creatorId}/daily")]
    public async Task<ActionResult<DashboardMetricsDto>> GetDailyMetrics(
        Guid creatorId,
        [FromQuery] DateTime? date = null,
        [FromQuery] SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
    {
        var metrics = await _dashboardService.GetDailyMetricsAsync(creatorId, date, platform);
        return Ok(metrics);
    }

    [HttpGet("content/{creatorId}/top")]
    public async Task<ActionResult<List<ContentPostDto>>> GetTopContent(
        Guid creatorId,
        [FromQuery] int limit = 5)
    {
        var topContent = await _dashboardService.GetTopContentAsync(creatorId, limit);
        return Ok(topContent);
    }

    [HttpGet("recommendations/{creatorId}/posting-times")]
    public async Task<ActionResult<List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>>> GetBestTimeToPost(
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
    public async Task<ActionResult<DashboardMetricsDto>> AddMetrics([FromBody] DashboardMetricsDto metrics)
    {
        var result = await _dashboardService.AddMetricsAsync(metrics);
        return CreatedAtAction(nameof(GetDailyMetrics), new { creatorId = metrics.TopCreators.FirstOrDefault()?.CreatorId, date = DateTime.UtcNow }, result);
    }

    [HttpPost("content-performance")]
    public async Task<ActionResult<ContentPerformance>> AddContentPerformance([FromBody] MicroSaaS.Application.DTOs.ContentPerformanceDto performance)
    {
        var result = await _dashboardService.AddContentPerformanceAsync(performance);
        return CreatedAtAction(nameof(GetTopContent), new { creatorId = performance.AccountId }, result);
    }

    [HttpGet("insights")]
    public async Task<ActionResult<DashboardInsightsDto>> GetDashboardInsights()
    {
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid creatorId))
        {
            return Unauthorized("ID do usuário inválido ou não encontrado no token.");
        }
        try
        {
            var insightsDto = await _dashboardService.GetLatestInsightsAsync(creatorId);
            if (insightsDto == null)
            {
                return NotFound("Nenhum insight encontrado para este criador.");
            }
            return Ok(insightsDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar insights para o criador {CreatorId}", creatorId);
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao buscar insights.");
        }
    }
}