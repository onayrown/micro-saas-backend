using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using MicroSaaS.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace MicroSaaS.Backend.Controllers;

/// <summary>
/// Controlador responsável por fornecer análises de desempenho de conteúdo nas redes sociais
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly ISocialMediaIntegrationService _socialMediaService;
    private readonly ISocialMediaAccountRepository _accountRepository;
    private readonly IContentPostRepository _postRepository;

    /// <summary>
    /// Construtor do AnalyticsController
    /// </summary>
    /// <param name="socialMediaService">Serviço de integração com redes sociais</param>
    /// <param name="accountRepository">Repositório de contas de mídia social</param>
    /// <param name="postRepository">Repositório de posts de conteúdo</param>
    public AnalyticsController(
        ISocialMediaIntegrationService socialMediaService,
        ISocialMediaAccountRepository accountRepository,
        IContentPostRepository postRepository)
    {
        _socialMediaService = socialMediaService;
        _accountRepository = accountRepository;
        _postRepository = postRepository;
    }

    /// <summary>
    /// Obtém métricas de desempenho para um post específico
    /// </summary>
    /// <param name="postId">ID do post para análise</param>
    /// <returns>Métricas de desempenho do post</returns>
    /// <remarks>
    /// Exemplo de resposta:
    /// 
    /// ```json
    /// {
    ///   "success": true,
    ///   "data": [
    ///     {
    ///       "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///       "platform": "Instagram",
    ///       "dateRecorded": "2023-06-14T10:00:00Z",
    ///       "likes": 245,
    ///       "comments": 31,
    ///       "shares": 18,
    ///       "views": 1250,
    ///       "impressions": 2150,
    ///       "engagementRate": 12.5,
    ///       "reach": 1850,
    ///       "saves": 42,
    ///       "clickThroughs": 75
    ///     }
    ///   ],
    ///   "message": "Métricas de desempenho recuperadas com sucesso"
    /// }
    /// ```
    /// </remarks>
    /// <response code="200">Análises recuperadas com sucesso</response>
    /// <response code="400">ID de post inválido</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para acessar estas métricas</response>
    /// <response code="404">Post não encontrado</response>
    /// <response code="500">Erro interno do servidor ou falha na integração com a rede social</response>
    [HttpGet("post/{postId}")]
    [ProducesResponseType(typeof(ApiResponse<List<ContentPerformanceDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<ContentPerformanceDto>>>> GetPostAnalytics(string postId)
    {
        try
        {
            // Validar se o ID é válido
            if (string.IsNullOrEmpty(postId))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "ID do post não pode ser vazio"
                });
                
            // Verificar se o post existe
            var post = await _postRepository.GetByIdAsync(Guid.Parse(postId));
            if (post == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Post não encontrado"
                });

            var analyticsResult = await _socialMediaService.GetPostPerformanceAsync(postId);
            // Convertendo IEnumerable para List explicitamente
            var analytics = analyticsResult.ToList();
            
            return Ok(new ApiResponse<List<ContentPerformanceDto>>
            {
                Success = true,
                Data = analytics,
                Message = "Métricas de desempenho recuperadas com sucesso"
            });
        }
        catch (FormatException)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Formato de ID inválido. O ID deve ser um GUID válido."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Erro ao recuperar métricas de desempenho"
            });
        }
    }

    /// <summary>
    /// Obtém análises de desempenho para um criador de conteúdo em uma plataforma específica
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <param name="platform">Plataforma de mídia social (Instagram, YouTube, TikTok, etc)</param>
    /// <param name="startDate">Data de início do período de análise</param>
    /// <param name="endDate">Data de fim do período de análise</param>
    /// <returns>Métricas de desempenho do criador na plataforma</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    /// ```
    /// GET /api/v1/Analytics/creator/3fa85f64-5717-4562-b3fc-2c963f66afa6?platform=Instagram&amp;startDate=2023-06-01T00:00:00Z&amp;endDate=2023-06-30T23:59:59Z
    /// ```
    /// 
    /// Exemplo de resposta:
    /// 
    /// ```json
    /// {
    ///   "success": true,
    ///   "data": [
    ///     {
    ///       "date": "2023-06-01T00:00:00Z",
    ///       "platform": "Instagram",
    ///       "followers": 5250,
    ///       "followersGrowth": 125,
    ///       "averageEngagement": 8.5,
    ///       "totalPosts": 12,
    ///       "totalLikes": 3450,
    ///       "totalComments": 628,
    ///       "totalShares": 245,
    ///       "totalViews": 28500,
    ///       "impressions": 42000,
    ///       "reach": 35000
    ///     },
    ///     {
    ///       "date": "2023-06-15T00:00:00Z",
    ///       "platform": "Instagram",
    ///       "followers": 5420,
    ///       "followersGrowth": 170,
    ///       "averageEngagement": 9.2,
    ///       "totalPosts": 15,
    ///       "totalLikes": 4125,
    ///       "totalComments": 735,
    ///       "totalShares": 312,
    ///       "totalViews": 32400,
    ///       "impressions": 48500,
    ///       "reach": 39500
    ///     }
    ///   ],
    ///   "message": "Métricas de desempenho recuperadas com sucesso"
    /// }
    /// ```
    /// </remarks>
    /// <response code="200">Análises recuperadas com sucesso</response>
    /// <response code="400">Parâmetros de consulta inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para acessar estas métricas</response>
    /// <response code="404">Criador não encontrado ou conta não encontrada para a plataforma especificada</response>
    /// <response code="429">Taxa de requisições à API da plataforma excedida</response>
    /// <response code="500">Erro interno do servidor ou falha na integração com a rede social</response>
    [HttpGet("creator/{creatorId}")]
    [ProducesResponseType(typeof(ApiResponse<List<ContentPerformanceDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<ContentPerformanceDto>>>> GetCreatorAnalytics(
        Guid creatorId,
        [FromQuery] SocialMediaPlatform platform,
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

            // Limitar período de análise para no máximo 90 dias
            var maxPeriod = TimeSpan.FromDays(90);
            if (endDate - startDate > maxPeriod)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "O período de análise não pode exceder 90 dias"
                });
            }

            // Buscar as contas de mídia social
            var accounts = await _accountRepository.GetByPlatformAsync(creatorId, platform);
            var account = accounts.FirstOrDefault();
            
            if (account == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Nenhuma conta encontrada para a plataforma especificada"
                });

            var analyticsResult = await _socialMediaService.GetAccountPerformanceAsync(
                account.Id,
                startDate,
                endDate);
                
            // Convertendo IEnumerable para List explicitamente
            var analytics = analyticsResult.ToList();
            
            return Ok(new ApiResponse<List<ContentPerformanceDto>>
            {
                Success = true,
                Data = analytics,
                Message = "Métricas de desempenho recuperadas com sucesso"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            // Verificar se é um erro de rate limiting da API da plataforma
            if (ex.Message.Contains("rate limit") || ex.Message.Contains("quota exceeded"))
            {
                return StatusCode(429, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Taxa de requisições à API da plataforma excedida. Tente novamente mais tarde."
                });
            }
            
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Erro ao buscar métricas de desempenho"
            });
        }
    }
    
    /// <summary>
    /// Obtém um resumo das métricas de desempenho consolidadas do criador em todas as plataformas
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <param name="period">Período de análise (Last7Days, Last30Days, Last90Days)</param>
    /// <returns>Resumo consolidado de métricas</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    /// ```
    /// GET /api/v1/Analytics/summary/3fa85f64-5717-4562-b3fc-2c963f66afa6?period=Last30Days
    /// ```
    /// 
    /// Exemplo de resposta:
    /// 
    /// ```json
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "totalFollowers": 12500,
    ///     "followersGrowth": 580,
    ///     "followersGrowthPercentage": 4.86,
    ///     "averageEngagementRate": 9.8,
    ///     "totalPosts": 42,
    ///     "totalEngagements": 8750,
    ///     "platformBreakdown": {
    ///       "Instagram": {
    ///         "followers": 5420,
    ///         "engagement": 10.2,
    ///         "posts": 18
    ///       },
    ///       "YouTube": {
    ///         "followers": 4850,
    ///         "engagement": 7.5,
    ///         "posts": 12
    ///       },
    ///       "TikTok": {
    ///         "followers": 2230,
    ///         "engagement": 12.8,
    ///         "posts": 12
    ///       }
    ///     },
    ///     "bestPerformingPlatform": "TikTok",
    ///     "fastestGrowingPlatform": "Instagram"
    ///   },
    ///   "message": "Resumo de métricas recuperado com sucesso"
    /// }
    /// ```
    /// </remarks>
    /// <response code="200">Resumo recuperado com sucesso</response>
    /// <response code="400">Período de análise inválido</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para acessar estas métricas</response>
    /// <response code="404">Criador não encontrado</response>
    /// <response code="500">Erro interno do servidor ou falha na integração com a rede social</response>
    [HttpGet("summary/{creatorId}")]
    [ProducesResponseType(typeof(ApiResponse<AnalyticsSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<AnalyticsSummaryDto>>> GetAnalyticsSummary(
        Guid creatorId,
        [FromQuery] AnalyticsPeriod period = AnalyticsPeriod.Last30Days)
    {
        try
        {
            // Validar o criador existe
            var creatorExists = await _accountRepository.CreatorExistsAsync(creatorId);
            if (!creatorExists)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Criador de conteúdo não encontrado"
                });

            // Calcular datas com base no período
            var endDate = DateTime.UtcNow;
            var startDate = period switch
            {
                AnalyticsPeriod.Last7Days => endDate.AddDays(-7),
                AnalyticsPeriod.Last30Days => endDate.AddDays(-30),
                AnalyticsPeriod.Last90Days => endDate.AddDays(-90),
                _ => throw new ArgumentException("Período de análise inválido")
            };

            // Em uma implementação real, você chamaria um serviço para buscar e calcular o resumo
            // Para fins de exemplo, retornamos um modelo fictício
            var summary = new AnalyticsSummaryDto
            {
                TotalFollowers = 12500,
                FollowersGrowth = 580,
                FollowersGrowthPercentage = 4.86,
                AverageEngagementRate = 9.8,
                TotalPosts = 42,
                TotalEngagements = 8750,
                PlatformBreakdown = new Dictionary<string, PlatformStatsDto>
                {
                    ["Instagram"] = new PlatformStatsDto { Followers = 5420, Engagement = 10.2, Posts = 18 },
                    ["YouTube"] = new PlatformStatsDto { Followers = 4850, Engagement = 7.5, Posts = 12 },
                    ["TikTok"] = new PlatformStatsDto { Followers = 2230, Engagement = 12.8, Posts = 12 }
                },
                BestPerformingPlatform = "TikTok",
                FastestGrowingPlatform = "Instagram"
            };

            return Ok(new ApiResponse<AnalyticsSummaryDto>
            {
                Success = true,
                Data = summary,
                Message = "Resumo de métricas recuperado com sucesso"
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Erro ao recuperar resumo de métricas"
            });
        }
    }
} 