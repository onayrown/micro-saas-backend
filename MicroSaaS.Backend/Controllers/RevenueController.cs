using MicroSaaS.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RevenueController : ControllerBase
{
    private readonly IRevenueService _revenueService;

    public RevenueController(IRevenueService revenueService)
    {
        _revenueService = revenueService;
    }

    [HttpPost("connect-adsense/{creatorId}")]
    public async Task<IActionResult> ConnectAdSense(
        Guid creatorId,
        [FromBody] ConnectAdSenseRequest request)
    {
        // Iniciar processo de conexão com AdSense
        var authUrl = await _revenueService.GetAdSenseAuthUrlAsync(
            creatorId,
            Url.Action(nameof(AdSenseCallback), new { creatorId })
        );

        return Ok(new { authorizationUrl = authUrl });
    }

    [HttpGet("adsense-callback")]
    [AllowAnonymous]
    public async Task<IActionResult> AdSenseCallback(
        [FromQuery] Guid creatorId,
        [FromQuery] string code)
    {
        // Finalizar a conexão com AdSense
        await _revenueService.ConnectAdSenseAsync(creatorId, code);

        // Redirecionar para a aplicação frontend
        return Redirect($"https://seuapp.com/revenue/adsense-connected");
    }

    [HttpGet("revenue/{creatorId}")]
    public async Task<ActionResult<RevenueSummary>> GetRevenueSummary(
        Guid creatorId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        // Obter dados de receita
        var revenueSummary = await _revenueService.GetRevenueAsync(
            creatorId,
            startDate,
            endDate);

        return Ok(revenueSummary);
    }

    [HttpGet("revenue/{creatorId}/by-platform")]
    public async Task<ActionResult<List<PlatformRevenue>>> GetRevenueByPlatform(
        Guid creatorId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        // Obter dados de receita por plataforma
        var platformRevenue = await _revenueService.GetRevenueByPlatformAsync(
            creatorId,
            startDate,
            endDate);

        return Ok(platformRevenue);
    }

    [HttpGet("revenue/{creatorId}/by-day")]
    public async Task<ActionResult<List<DailyRevenue>>> GetRevenueByDay(
        Guid creatorId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        // Obter dados de receita por dia
        var dailyRevenue = await _revenueService.GetRevenueByDayAsync(
            creatorId,
            startDate,
            endDate);

        return Ok(dailyRevenue);
    }

    [HttpGet("monetization/{creatorId}")]
    public async Task<ActionResult<MonetizationMetricsDto>> GetMonetizationMetrics(
        Guid creatorId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            // Obter métricas avançadas de monetização
            var metrics = await _revenueService.GetMonetizationMetricsAsync(
                creatorId,
                startDate,
                endDate);

            return Ok(metrics);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("adsense/refresh/{creatorId}")]
    public async Task<IActionResult> RefreshAdSenseData(Guid creatorId)
    {
        try
        {
            // Atualizar os dados do AdSense
            await _revenueService.RefreshRevenueMetricsAsync();
            return Ok(new { message = "Dados do AdSense atualizados com sucesso" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

public class ConnectAdSenseRequest
{
    // Campos necessários para conectar com AdSense
    public string Email { get; set; }
} 