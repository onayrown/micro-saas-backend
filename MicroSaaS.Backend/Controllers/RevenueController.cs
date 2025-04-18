using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroSaaS.Application.DTOs.Revenue;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.Backend.Controllers;

/// <summary>
/// Controlador responsável por gerenciar receitas e monetização de conteúdo
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class RevenueController : ControllerBase
{
    private readonly IRevenueService _revenueService;

    /// <summary>
    /// Construtor do RevenueController
    /// </summary>
    /// <param name="revenueService">Serviço de gestão de receitas</param>
    public RevenueController(IRevenueService revenueService)
    {
        _revenueService = revenueService;
    }

    /// <summary>
    /// Obtém um resumo das receitas de um criador de conteúdo
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <param name="startDate">Data de início do período</param>
    /// <param name="endDate">Data de fim do período</param>
    /// <returns>Resumo das receitas no período</returns>
    /// <remarks>
    /// Exemplo de resposta:
    /// 
    /// ```json
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "totalRevenue": 1250.75,
    ///     "currency": "BRL",
    ///     "adSenseRevenue": 850.25,
    ///     "sponsorshipsRevenue": 400.50,
    ///     "affiliateRevenue": 0.00,
    ///     "revenueByPlatform": {
    ///       "Instagram": 450.25,
    ///       "YouTube": 650.50,
    ///       "Blog": 150.00
    ///     },
    ///     "previousPeriodRevenue": 980.50,
    ///     "revenueGrowth": 27.56,
    ///     "projectedRevenue": 1500.00
    ///   },
    ///   "message": "Resumo de receitas recuperado com sucesso"
    /// }
    /// ```
    /// </remarks>
    /// <response code="200">Resumo de receitas recuperado com sucesso</response>
    /// <response code="400">Parâmetros de consulta inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para acessar estes dados</response>
    /// <response code="404">Criador não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("revenue/{creatorId}")]
    [ProducesResponseType(typeof(ApiResponse<RevenueSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<RevenueSummaryDto>>> GetRevenueSummary(
        Guid creatorId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            // Validar parâmetros
            if (startDate >= endDate)
            {
                return BadRequest(new ApiResponse<RevenueSummaryDto> { Success = false, Message = "A data de início deve ser anterior à data de fim" });
            }

            // Obter dados de receita (serviço já retorna o tipo Shared)
            var revenueSummaryDto = await _revenueService.GetRevenueAsync(
                creatorId,
                startDate,
                endDate);
                
            if (revenueSummaryDto == null) // Adicionar verificação de nulo
            {
                return NotFound(new ApiResponse<RevenueSummaryDto> { Success = false, Message = "Resumo de receitas não encontrado para o período." });
            }

            return Ok(new ApiResponse<RevenueSummaryDto>
            {
                Success = true,
                Data = revenueSummaryDto, // Usar diretamente o objeto retornado pelo serviço
                Message = "Resumo de receitas recuperado com sucesso"
            });
        }
        catch (KeyNotFoundException) // Se o serviço lançar essa exceção
        {
            return NotFound(new ApiResponse<RevenueSummaryDto> { Success = false, Message = "Criador não encontrado" });
        }
        catch (Exception ex) // Logar exceção
        {
            // _logger?.LogError(ex, "Erro ao recuperar resumo de receitas para {CreatorId}", creatorId); // Adicionar logger se necessário
            return StatusCode(500, new ApiResponse<RevenueSummaryDto> { Success = false, Message = "Erro interno ao recuperar resumo de receitas." });
        }
    }

    /// <summary>
    /// Obtém receitas detalhadas por plataforma para um criador
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <param name="startDate">Data de início do período</param>
    /// <param name="endDate">Data de fim do período</param>
    /// <returns>Lista de receitas por plataforma</returns>
    /// <remarks>
    /// Exemplo de resposta:
    /// 
    /// ```json
    /// {
    ///   "success": true,
    ///   "data": [
    ///     {
    ///       "platform": "Instagram",
    ///       "totalRevenue": 450.25,
    ///       "adRevenue": 150.25,
    ///       "sponsorshipRevenue": 300.00,
    ///       "affiliateRevenue": 0.00,
    ///       "revenuePerPost": 37.52,
    ///       "revenuePerFollower": 0.085,
    ///       "previousPeriodRevenue": 380.50,
    ///       "revenueGrowth": 18.33
    ///     },
    ///     {
    ///       "platform": "YouTube",
    ///       "totalRevenue": 650.50,
    ///       "adRevenue": 550.00,
    ///       "sponsorshipRevenue": 100.50,
    ///       "affiliateRevenue": 0.00,
    ///       "revenuePerPost": 54.21,
    ///       "revenuePerFollower": 0.134,
    ///       "previousPeriodRevenue": 500.00,
    ///       "revenueGrowth": 30.10
    ///     }
    ///   ],
    ///   "message": "Receitas por plataforma recuperadas com sucesso"
    /// }
    /// ```
    /// </remarks>
    /// <response code="200">Receitas por plataforma recuperadas com sucesso</response>
    /// <response code="400">Parâmetros de consulta inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para acessar estes dados</response>
    /// <response code="404">Criador não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("revenue/{creatorId}/by-platform")]
    [ProducesResponseType(typeof(ApiResponse<List<PlatformRevenueDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<PlatformRevenueDto>>>> GetRevenueByPlatform(
        Guid creatorId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            // Validar parâmetros
            if (startDate >= endDate)
            {
                 return BadRequest(new ApiResponse<List<PlatformRevenueDto>> { Success = false, Message = "A data de início deve ser anterior à data de fim" });
            }

            // Obter dados de receita por plataforma (serviço já retorna o tipo Shared)
            var platformRevenueDtos = await _revenueService.GetRevenueByPlatformAsync(
                creatorId,
                startDate,
                endDate);
                
            return Ok(new ApiResponse<List<PlatformRevenueDto>>
            {
                Success = true,
                Data = platformRevenueDtos.ToList(), // Usar diretamente o objeto retornado pelo serviço
                Message = "Receitas por plataforma recuperadas com sucesso"
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ApiResponse<List<PlatformRevenueDto>> { Success = false, Message = "Criador não encontrado" });
        }
        catch (Exception ex) // Logar exceção
        {
             // _logger?.LogError(ex, "Erro ao recuperar receitas por plataforma para {CreatorId}", creatorId);
            return StatusCode(500, new ApiResponse<List<PlatformRevenueDto>> { Success = false, Message = "Erro ao recuperar dados de receita por plataforma" });
        }
    }

    /// <summary>
    /// Obtém as receitas diárias de um criador no período especificado
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <param name="startDate">Data de início do período</param>
    /// <param name="endDate">Data de fim do período</param>
    /// <returns>Lista de receitas diárias</returns>
    /// <remarks>
    /// Exemplo de resposta:
    /// 
    /// ```json
    /// {
    ///   "success": true,
    ///   "data": [
    ///     {
    ///       "date": "2023-06-01",
    ///       "totalRevenue": 42.75,
    ///       "adRevenue": 32.50,
    ///       "sponsorshipRevenue": 10.25,
    ///       "affiliateRevenue": 0.00,
    ///       "platformBreakdown": {
    ///         "Instagram": 15.50,
    ///         "YouTube": 27.25
    ///       }
    ///     },
    ///     {
    ///       "date": "2023-06-02",
    ///       "totalRevenue": 38.50,
    ///       "adRevenue": 28.50,
    ///       "sponsorshipRevenue": 10.00,
    ///       "affiliateRevenue": 0.00,
    ///       "platformBreakdown": {
    ///         "Instagram": 14.25,
    ///         "YouTube": 24.25
    ///       }
    ///     }
    ///   ],
    ///   "message": "Receitas diárias recuperadas com sucesso"
    /// }
    /// ```
    /// </remarks>
    /// <response code="200">Receitas diárias recuperadas com sucesso</response>
    /// <response code="400">Parâmetros de consulta inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para acessar estes dados</response>
    /// <response code="404">Criador não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("revenue/{creatorId}/by-day")]
    [ProducesResponseType(typeof(ApiResponse<List<DailyRevenueDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<DailyRevenueDto>>>> GetRevenueByDay(
        Guid creatorId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            // Validar parâmetros
            if (startDate >= endDate)
            {
                 return BadRequest(new ApiResponse<List<DailyRevenueDto>> { Success = false, Message = "A data de início deve ser anterior à data de fim" });
            }

            // Limitar o período (manter validação se necessário)
            var maxPeriod = TimeSpan.FromDays(90);
            if (endDate - startDate > maxPeriod)
            {
                 return BadRequest(new ApiResponse<List<DailyRevenueDto>> { Success = false, Message = "O período de análise não pode exceder 90 dias" });
            }

            // Obter dados de receita por dia (serviço já retorna o tipo Shared)
            var dailyRevenueDtos = await _revenueService.GetRevenueByDayAsync(
                creatorId,
                startDate,
                endDate);
                
            return Ok(new ApiResponse<List<DailyRevenueDto>>
            {
                Success = true,
                Data = dailyRevenueDtos.ToList(), // Usar diretamente o objeto retornado pelo serviço
                Message = "Receitas diárias recuperadas com sucesso"
            });
        }
        catch (KeyNotFoundException)
        {
             return NotFound(new ApiResponse<List<DailyRevenueDto>> { Success = false, Message = "Criador não encontrado" });
        }
        catch (Exception ex) // Logar exceção
        {
             // _logger?.LogError(ex, "Erro ao recuperar receitas diárias para {CreatorId}", creatorId);
            return StatusCode(500, new ApiResponse<List<DailyRevenueDto>> { Success = false, Message = "Erro ao recuperar dados de receita diária" });
        }
    }

    /// <summary>
    /// Obtém métricas avançadas de monetização para um criador
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <param name="startDate">Data de início do período</param>
    /// <param name="endDate">Data de fim do período</param>
    /// <returns>Métricas de monetização</returns>
    /// <remarks>
    /// Exemplo de resposta:
    /// 
    /// ```json
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "rpm": 12.45,
    ///     "cpm": 8.75,
    ///     "ctr": 2.35,
    ///     "revenuePerFollower": 0.125,
    ///     "revenuePerPost": 42.50,
    ///     "revenuePerEngagement": 0.85,
    ///     "bestPerformingContentTypes": [
    ///       {
    ///         "contentType": "Tutorial",
    ///         "averageRevenue": 68.50,
    ///         "totalRevenue": 685.00
    ///       },
    ///       {
    ///         "contentType": "Vlog",
    ///         "averageRevenue": 42.25,
    ///         "totalRevenue": 422.50
    ///       }
    ///     ],
    ///     "bestTimeForPosting": "18:00-21:00",
    ///     "bestDayForPosting": "Quinta-feira"
    ///   },
    ///   "message": "Métricas de monetização recuperadas com sucesso"
    /// }
    /// ```
    /// </remarks>
    /// <response code="200">Métricas recuperadas com sucesso</response>
    /// <response code="400">Parâmetros de consulta inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para acessar estas métricas</response>
    /// <response code="404">Criador não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("monetization/{creatorId}")]
    [ProducesResponseType(typeof(ApiResponse<MonetizationMetricsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<MonetizationMetricsDto>>> GetMonetizationMetrics(
        Guid creatorId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            // Validar parâmetros
            if (startDate >= endDate)
            {
                 return BadRequest(new ApiResponse<MonetizationMetricsDto> { Success = false, Message = "A data de início deve ser anterior à data de fim" });
            }

            // Obter métricas (Serviço já retorna DTO)
            var metrics = await _revenueService.GetMonetizationMetricsAsync(
                creatorId,
                startDate,
                endDate);
            
             if (metrics == null) // Adicionar verificação de nulo
            {
                return NotFound(new ApiResponse<MonetizationMetricsDto> { Success = false, Message = "Métricas de monetização não encontradas." });
            }

            return Ok(new ApiResponse<MonetizationMetricsDto>
            {
                Success = true,
                Data = metrics,
                Message = "Métricas de monetização recuperadas com sucesso"
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ApiResponse<MonetizationMetricsDto> { Success = false, Message = "Criador não encontrado" });
        }
        catch (Exception ex) // Logar exceção e tratar
        {
             // _logger?.LogError(ex, "Erro ao recuperar métricas de monetização para {CreatorId}", creatorId);
             // Retornar BadRequest pode expor detalhes internos, considerar 500
            return StatusCode(500, new ApiResponse<MonetizationMetricsDto> { Success = false, Message = "Erro interno ao recuperar métricas de monetização." /* + ex.Message */ }); 
        }
    }
} 