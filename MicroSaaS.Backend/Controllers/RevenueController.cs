using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    /// Inicia o processo de conexão com o Google AdSense para um criador
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <param name="request">Dados necessários para conexão</param>
    /// <returns>URL de autorização do Google AdSense</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    /// ```json
    /// {
    ///   "email": "criador@exemplo.com"
    /// }
    /// ```
    /// 
    /// Exemplo de resposta:
    /// 
    /// ```json
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "authorizationUrl": "https://accounts.google.com/o/oauth2/auth?client_id=..."
    ///   },
    ///   "message": "URL de autorização gerada com sucesso"
    /// }
    /// ```
    /// </remarks>
    /// <response code="200">URL de autorização gerada com sucesso</response>
    /// <response code="400">Dados inválidos para conexão</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para conectar esta conta</response>
    /// <response code="404">Criador não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("connect-adsense/{creatorId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ConnectAdSense(
        Guid creatorId,
        [FromBody] ConnectAdSenseRequest request)
    {
        try
        {
            // Validar o request
            if (string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Email é obrigatório"
                });
            }

            // Iniciar processo de conexão com AdSense
            var authUrl = await _revenueService.GetAdSenseAuthUrlAsync(
                creatorId,
                Url.Action(nameof(AdSenseCallback), new { creatorId })
            );

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { authorizationUrl = authUrl },
                Message = "URL de autorização gerada com sucesso"
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Criador não encontrado"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Erro ao conectar com AdSense"
            });
        }
    }

    /// <summary>
    /// Callback que recebe o código de autorização do Google AdSense após autenticação
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <param name="code">Código de autorização do Google</param>
    /// <returns>Redirecionamento para a aplicação frontend</returns>
    /// <remarks>
    /// Este endpoint é chamado pelo Google após o usuário conceder permissão.
    /// O usuário será redirecionado para a aplicação frontend.
    /// </remarks>
    /// <response code="302">Redirecionamento para frontend após conexão bem-sucedida</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="404">Criador não encontrado</response>
    /// <response code="500">Erro ao processar callback</response>
    [HttpGet("adsense-callback")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AdSenseCallback(
        [FromQuery] Guid creatorId,
        [FromQuery] string code)
    {
        try
        {
            // Validar parâmetros
            if (creatorId == Guid.Empty || string.IsNullOrEmpty(code))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "ID do criador e código de autorização são obrigatórios"
                });
            }

            // Finalizar a conexão com AdSense
            await _revenueService.ConnectAdSenseAsync(creatorId, code);

            // Redirecionar para a aplicação frontend
            return Redirect($"https://seuapp.com/revenue/adsense-connected?success=true");
        }
        catch (KeyNotFoundException)
        {
            return Redirect($"https://seuapp.com/revenue/adsense-connected?success=false&error=creator-not-found");
        }
        catch (Exception)
        {
            return Redirect($"https://seuapp.com/revenue/adsense-connected?success=false&error=internal-error");
        }
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
    [ProducesResponseType(typeof(ApiResponse<MicroSaaS.Shared.Models.RevenueSummary>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<MicroSaaS.Shared.Models.RevenueSummary>>> GetRevenueSummary(
        Guid creatorId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            // Validar parâmetros
            if (startDate >= endDate)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "A data de início deve ser anterior à data de fim"
                });
            }

            // Obter dados de receita
            var serviceRevenueSummary = await _revenueService.GetRevenueAsync(
                creatorId,
                startDate,
                endDate);
                
            // Converter para o modelo da camada Shared
            var revenueSummary = ConvertToSharedRevenueSummary(serviceRevenueSummary);

            return Ok(new ApiResponse<MicroSaaS.Shared.Models.RevenueSummary>
            {
                Success = true,
                Data = revenueSummary,
                Message = "Resumo de receitas recuperado com sucesso"
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Criador não encontrado"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Erro ao recuperar dados de receita"
            });
        }
    }

    /// <summary>
    /// Converte RevenueSummary do serviço para o modelo da camada Shared
    /// </summary>
    private MicroSaaS.Shared.Models.RevenueSummary ConvertToSharedRevenueSummary(
        MicroSaaS.Application.Interfaces.Services.RevenueSummary serviceSummary)
    {
        // Implementar conversão de propriedades
        return new MicroSaaS.Shared.Models.RevenueSummary
        {
            // Mapear somente as propriedades que existem no modelo Shared
            TotalRevenue = serviceSummary.TotalRevenue,
            MonthlyRecurringRevenue = serviceSummary.EstimatedMonthlyRevenue,
            AverageRevenuePerUser = serviceSummary.AverageRevenuePerView,
            // Definir valores padrão para as propriedades restantes
            AnnualRecurringRevenue = serviceSummary.EstimatedMonthlyRevenue * 12,
            TotalSubscribers = 0 // Valor padrão, pois não temos essa informação no modelo de serviço
        };
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
    [ProducesResponseType(typeof(ApiResponse<List<MicroSaaS.Shared.Models.PlatformRevenue>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<MicroSaaS.Shared.Models.PlatformRevenue>>>> GetRevenueByPlatform(
        Guid creatorId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            // Validar parâmetros
            if (startDate >= endDate)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "A data de início deve ser anterior à data de fim"
                });
            }

            // Obter dados de receita por plataforma
            var servicePlatformRevenue = await _revenueService.GetRevenueByPlatformAsync(
                creatorId,
                startDate,
                endDate);
                
            // Converter para o modelo da camada Shared
            var platformRevenue = ConvertToSharedPlatformRevenue(servicePlatformRevenue);

            return Ok(new ApiResponse<List<MicroSaaS.Shared.Models.PlatformRevenue>>
            {
                Success = true,
                Data = platformRevenue,
                Message = "Receitas por plataforma recuperadas com sucesso"
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Criador não encontrado"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Erro ao recuperar dados de receita por plataforma"
            });
        }
    }
    
    /// <summary>
    /// Converte lista de PlatformRevenue do serviço para o modelo da camada Shared
    /// </summary>
    private List<MicroSaaS.Shared.Models.PlatformRevenue> ConvertToSharedPlatformRevenue(
        List<MicroSaaS.Application.Interfaces.Services.PlatformRevenue> servicePlatformRevenues)
    {
        var result = new List<MicroSaaS.Shared.Models.PlatformRevenue>();
        
        foreach (var serviceRevenue in servicePlatformRevenues)
        {
            result.Add(new MicroSaaS.Shared.Models.PlatformRevenue
            {
                // Mapear somente as propriedades que existem no modelo Shared
                Platform = serviceRevenue.Platform,
                Amount = serviceRevenue.Revenue,
                Currency = "USD",
                Subscribers = 0 // Valor padrão
            });
        }
        
        return result;
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
    [ProducesResponseType(typeof(ApiResponse<List<MicroSaaS.Shared.Models.DailyRevenue>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<MicroSaaS.Shared.Models.DailyRevenue>>>> GetRevenueByDay(
        Guid creatorId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            // Validar parâmetros
            if (startDate >= endDate)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "A data de início deve ser anterior à data de fim"
                });
            }

            // Limitar o período para no máximo 90 dias
            var maxPeriod = TimeSpan.FromDays(90);
            if (endDate - startDate > maxPeriod)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "O período de análise não pode exceder 90 dias"
                });
            }

            // Obter dados de receita por dia
            var serviceDailyRevenue = await _revenueService.GetRevenueByDayAsync(
                creatorId,
                startDate,
                endDate);
                
            // Converter para o modelo da camada Shared
            var dailyRevenue = ConvertToSharedDailyRevenue(serviceDailyRevenue);

            return Ok(new ApiResponse<List<MicroSaaS.Shared.Models.DailyRevenue>>
            {
                Success = true,
                Data = dailyRevenue,
                Message = "Receitas diárias recuperadas com sucesso"
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Criador não encontrado"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Erro ao recuperar dados de receita diária"
            });
        }
    }
    
    /// <summary>
    /// Converte lista de DailyRevenue do serviço para o modelo da camada Shared
    /// </summary>
    private List<MicroSaaS.Shared.Models.DailyRevenue> ConvertToSharedDailyRevenue(
        List<MicroSaaS.Application.Interfaces.Services.DailyRevenue> serviceDailyRevenues)
    {
        var result = new List<MicroSaaS.Shared.Models.DailyRevenue>();
        
        foreach (var serviceRevenue in serviceDailyRevenues)
        {
            result.Add(new MicroSaaS.Shared.Models.DailyRevenue
            {
                // Mapear somente as propriedades que existem no modelo Shared
                Date = serviceRevenue.Date,
                Amount = serviceRevenue.Revenue,
                Currency = "USD"
            });
        }
        
        return result;
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
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "A data de início deve ser anterior à data de fim"
                });
            }

            // Obter métricas avançadas de monetização
            var metrics = await _revenueService.GetMonetizationMetricsAsync(
                creatorId,
                startDate,
                endDate);

            return Ok(new ApiResponse<MonetizationMetricsDto>
            {
                Success = true,
                Data = metrics,
                Message = "Métricas de monetização recuperadas com sucesso"
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Criador não encontrado"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    /// <summary>
    /// Atualiza os dados de receita do AdSense para um criador
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <returns>Status da atualização</returns>
    /// <remarks>
    /// Este endpoint força a atualização dos dados de receita do AdSense,
    /// buscando os dados mais recentes diretamente da API do Google.
    /// 
    /// Exemplo de resposta:
    /// 
    /// ```json
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "lastUpdated": "2023-06-15T14:30:00Z",
    ///     "status": "Completed"
    ///   },
    ///   "message": "Dados do AdSense atualizados com sucesso"
    /// }
    /// ```
    /// </remarks>
    /// <response code="200">Dados atualizados com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para atualizar estes dados</response>
    /// <response code="404">Criador não encontrado ou não tem conta AdSense conectada</response>
    /// <response code="429">Taxa de requisições excedida</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("adsense/refresh/{creatorId}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RefreshAdSenseData(Guid creatorId)
    {
        try
        {
            // Atualizar os dados do AdSense
            await _revenueService.RefreshRevenueMetricsAsync();
            
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { 
                    lastUpdated = DateTime.UtcNow,
                    status = "Completed"
                },
                Message = "Dados do AdSense atualizados com sucesso"
            });
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Criador não encontrado ou não tem conta AdSense conectada"
            });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("rate limit"))
        {
            return StatusCode(429, new ApiResponse<object>
            {
                Success = false,
                Message = "Taxa de requisições excedida. Tente novamente mais tarde."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }
}

/// <summary>
/// Modelo para requisição de conexão com AdSense
/// </summary>
public class ConnectAdSenseRequest
{
    /// <summary>
    /// Email associado à conta do Google AdSense
    /// </summary>
    public string Email { get; set; }
} 