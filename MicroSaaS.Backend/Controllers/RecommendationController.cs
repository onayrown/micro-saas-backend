using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Application.DTOs.ContentAnalysis;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;
    private readonly IContentAnalysisService _contentAnalysisService;
    private readonly ILoggingService _loggingService;

    public RecommendationController(
        IRecommendationService recommendationService,
        IContentAnalysisService contentAnalysisService,
        ILoggingService loggingService)
    {
        _recommendationService = recommendationService;
        _contentAnalysisService = contentAnalysisService;
        _loggingService = loggingService;
    }

    /// <summary>
    /// Obtém recomendações de melhores horários para postagem em uma plataforma específica
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <param name="platform">Plataforma de mídia social</param>
    /// <returns>Lista de recomendações de horários</returns>
    /// <response code="200">Recomendações obtidas com sucesso</response>
    /// <response code="400">Erro ao obter recomendações</response>
    [HttpGet("best-times/{creatorId}")]
    [ProducesResponseType(typeof(List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>>> GetBestTimeToPost(
        Guid creatorId, 
        [FromQuery] SocialMediaPlatform platform)
    {
        try
        {
            var recommendationsDto = await _recommendationService.GetBestTimeToPostAsync(creatorId, platform);
            return Ok(recommendationsDto);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter melhores horários para postagem");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtém recomendações de melhores horários para postagem em todas as plataformas
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <returns>Dicionário com recomendações de horários por plataforma</returns>
    /// <response code="200">Recomendações obtidas com sucesso</response>
    /// <response code="400">Erro ao obter recomendações</response>
    [HttpGet("best-times/{creatorId}/all-platforms")]
    [ProducesResponseType(typeof(Dictionary<SocialMediaPlatform, List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Dictionary<SocialMediaPlatform, List<MicroSaaS.Shared.DTOs.BestTimeSlotDto>>>> GetBestTimeToPostAllPlatforms(
        Guid creatorId)
    {
        try
        {
            var recommendationsDto = await _recommendationService.GetBestTimeToPostAllPlatformsAsync(creatorId);
            return Ok(recommendationsDto);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter melhores horários para postagem em todas plataformas");
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtém recomendações de conteúdo personalizadas para o criador
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <returns>Lista de recomendações de conteúdo</returns>
    /// <response code="200">Recomendações obtidas com sucesso</response>
    /// <response code="400">Erro ao obter recomendações</response>
    [HttpGet("content/{creatorId}")]
    [ProducesResponseType(typeof(List<ContentRecommendationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ContentRecommendationDto>>> GetContentRecommendations(Guid creatorId)
    {
        try
        {
            var recommendationsDto = await _recommendationService.GetContentRecommendationsAsync(creatorId);
            return Ok(recommendationsDto);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter recomendações de conteúdo");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("topics/{creatorId}")]
    [ProducesResponseType(typeof(List<ContentRecommendationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ContentRecommendationDto>>> GetTopicRecommendations(Guid creatorId)
    {
        try
        {
            var recommendationsDto = await _recommendationService.GetTopicRecommendationsAsync(creatorId);
            return Ok(recommendationsDto);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter recomendações de tópicos");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("formats/{creatorId}")]
    [ProducesResponseType(typeof(List<ContentRecommendationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ContentRecommendationDto>>> GetFormatRecommendations(Guid creatorId)
    {
        try
        {
            var recommendationsDto = await _recommendationService.GetFormatRecommendationsAsync(creatorId);
            return Ok(recommendationsDto);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter recomendações de formatos");
            return BadRequest(ex.Message);
        }
    }

    // Recomendações de hashtags
    [HttpGet("hashtags/{creatorId}")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<string>>> GetHashtagRecommendations(
        Guid creatorId, 
        [FromQuery] string contentDescription, 
        [FromQuery] SocialMediaPlatform platform)
    {
        try
        {
            var hashtags = await _recommendationService.GetHashtagRecommendationsAsync(creatorId, contentDescription, platform);
            return Ok(hashtags);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter recomendações de hashtags");
            return BadRequest(ex.Message);
        }
    }

    // Tendências
    [HttpGet("trends")]
    [ProducesResponseType(typeof(List<TrendTopicDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<TrendTopicDto>>> GetTrendingTopics([FromQuery] SocialMediaPlatform platform)
    {
        try
        {
            var trendsDto = await _recommendationService.GetTrendingTopicsAsync(platform);
            return Ok(trendsDto);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter tópicos em tendência");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("trends/{creatorId}/niche")]
    [ProducesResponseType(typeof(List<TrendTopicDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<TrendTopicDto>>> GetNicheTrendingTopics(Guid creatorId)
    {
        try
        {
            var trendsDto = await _recommendationService.GetNicheTrendingTopicsAsync(creatorId);
            return Ok(trendsDto);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter tópicos em tendência para o nicho");
            return BadRequest(ex.Message);
        }
    }

    // Recomendações estratégicas
    [HttpGet("monetization/{creatorId}")]
    [ProducesResponseType(typeof(List<ContentRecommendationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ContentRecommendationDto>>> GetMonetizationRecommendations(Guid creatorId)
    {
        try
        {
            var recommendationsDto = await _recommendationService.GetMonetizationRecommendationsAsync(creatorId);
            return Ok(recommendationsDto);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter recomendações de monetização");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("audience-growth/{creatorId}")]
    [ProducesResponseType(typeof(List<ContentRecommendationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ContentRecommendationDto>>> GetAudienceGrowthRecommendations(Guid creatorId)
    {
        try
        {
            var recommendationsDto = await _recommendationService.GetAudienceGrowthRecommendationsAsync(creatorId);
            return Ok(recommendationsDto);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter recomendações de crescimento de audiência");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("engagement/{creatorId}")]
    [ProducesResponseType(typeof(List<ContentRecommendationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ContentRecommendationDto>>> GetEngagementRecommendations(Guid creatorId)
    {
        try
        {
            var recommendationsDto = await _recommendationService.GetEngagementImprovementRecommendationsAsync(creatorId);
            return Ok(recommendationsDto);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter recomendações de engajamento");
            return BadRequest(ex.Message);
        }
    }

    // Análise de conteúdo
    [HttpGet("analyze/{contentId}")]
    [ProducesResponseType(typeof(MicroSaaS.Application.DTOs.ContentAnalysis.ContentAnalysisDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MicroSaaS.Application.DTOs.ContentAnalysis.ContentAnalysisDto>> AnalyzeContent(Guid contentId)
    {
        try
        {
            var analysisDto = await _recommendationService.AnalyzeContentPerformanceAsync(contentId);
            return Ok(analysisDto);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao analisar performance do conteúdo");
            return BadRequest(ex.Message);
        }
    }

    // Atualização de recomendações
    [HttpPost("refresh/{creatorId}")]
    public async Task<ActionResult> RefreshRecommendations(Guid creatorId)
    {
        try
        {
            await _recommendationService.RefreshRecommendationsAsync(creatorId);
            return Ok(new { message = "Recomendações atualizadas com sucesso" });
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao atualizar recomendações");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("content-insights/{contentId}")]
    public async Task<IActionResult> GetContentInsights(Guid contentId)
    {
        try
        {
            var insights = await _contentAnalysisService.GetContentInsightsAsync(contentId);
            return Ok(insights);
        }
        catch (ArgumentException aex)
        {
            _loggingService.LogWarning($"Solicitação inválida para insights de conteúdo: {aex.Message}");
            return BadRequest(aex.Message);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, $"Erro ao obter insights de conteúdo: {ex.Message}");
            return StatusCode(500, "Erro ao processar a solicitação de insights de conteúdo.");
        }
    }

    [HttpGet("high-performance-patterns/{creatorId}")]
    public async Task<IActionResult> GetHighPerformancePatterns(Guid creatorId, [FromQuery] int topPostsCount = 20)
    {
        try
        {
            var patterns = await _contentAnalysisService.AnalyzeHighPerformancePatternsAsync(creatorId, topPostsCount);
            return Ok(patterns);
        }
        catch (ArgumentException aex)
        {
            _loggingService.LogWarning($"Solicitação inválida para padrões de alto desempenho: {aex.Message}");
            return BadRequest(aex.Message);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, $"Erro ao obter padrões de alto desempenho: {ex.Message}");
            return StatusCode(500, "Erro ao processar a solicitação de padrões de alto desempenho.");
        }
    }

    [HttpGet("content-recommendations/{creatorId}")]
    public async Task<IActionResult> GenerateContentRecommendations(Guid creatorId)
    {
        try
        {
            var recommendations = await _contentAnalysisService.GenerateContentRecommendationsAsync(creatorId);
            return Ok(recommendations);
        }
        catch (ArgumentException aex)
        {
            _loggingService.LogWarning($"Solicitação inválida para recomendações de conteúdo: {aex.Message}");
            return BadRequest(aex.Message);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, $"Erro ao obter recomendações de conteúdo: {ex.Message}");
            return StatusCode(500, "Erro ao processar a solicitação de recomendações de conteúdo.");
        }
    }

    [HttpGet("topic-suggestions/{creatorId}")]
    public async Task<IActionResult> GetTopicSuggestions(Guid creatorId)
    {
        try
        {
            var topics = await _recommendationService.GetTopicRecommendationsAsync(creatorId);
            return Ok(topics);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, $"Erro ao obter sugestões de tópicos: {ex.Message}");
            return StatusCode(500, "Erro ao processar a solicitação de sugestões de tópicos.");
        }
    }

    [HttpGet("format-suggestions/{creatorId}")]
    public async Task<IActionResult> GetFormatSuggestions(Guid creatorId)
    {
        try
        {
            var formats = await _recommendationService.GetFormatRecommendationsAsync(creatorId);
            return Ok(formats);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, $"Erro ao obter sugestões de formatos: {ex.Message}");
            return StatusCode(500, "Erro ao processar a solicitação de sugestões de formatos.");
        }
    }
} 