using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
    [ProducesResponseType(typeof(DashboardInsights), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DashboardInsights>> GetLatestInsights(Guid creatorId)
    {
        // Correção: Linha ~62 - Verificar se User não é null E se claim existe E corresponde ao creatorId
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier);
        // Aqui precisamos decidir a lógica exata: 
        // 1. O usuário só pode ver seus próprios insights?
        // 2. Existe um role 'Admin' que pode ver qualquer um?
        // Assumindo que o usuário só pode ver o seu:
        if (userIdClaim == null || userIdClaim.Value != creatorId.ToString()) 
        {
             return Forbid("Você não tem permissão para acessar insights deste criador.");
        }

        try
        {
            var insights = await _dashboardService.GetLatestInsightsAsync(creatorId);
            if (insights == null)
            {
                return NotFound("Nenhum insight encontrado para este criador.");
            }
            return Ok(insights);
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
    /// <param name="startDate">Data de início opcional para o período de análise</param>
    /// <param name="endDate">Data de fim opcional para o período de análise</param>
    /// <returns>Insights recém-gerados do dashboard</returns>
    /// <response code="200">Insights gerados com sucesso</response>
    /// <response code="403">Usuário não autorizado</response>
    /// <response code="404">Criador não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("insights/{creatorId}/generate")]
    [ProducesResponseType(typeof(DashboardInsights), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

    [HttpGet("insights")]
    public async Task<ActionResult<DashboardInsights>> GetDashboardInsights()
    {
        // Obter o ID do usuário do token JWT
        var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid creatorId))
        {
            return Unauthorized("ID do usuário inválido ou não encontrado no token.");
        }

        try
        {
            // Assumindo que _dashboardService.GetLatestInsightsAsync(creatorId) retorna DashboardInsights
            var insights = await _dashboardService.GetLatestInsightsAsync(creatorId); 
            if (insights == null)
            {
                return NotFound("Nenhum insight encontrado para este criador.");
            }
            return Ok(insights);
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Erro ao buscar insights para o criador {CreatorId}", creatorId);
             return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao buscar insights.");
        }
    }
} 