using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
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
    private readonly ILoggingService _loggingService;

    public RecommendationController(
        IRecommendationService recommendationService,
        ILoggingService loggingService)
    {
        _recommendationService = recommendationService;
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
    [ProducesResponseType(typeof(List<PostTimeRecommendation>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<PostTimeRecommendation>>> GetBestTimeToPost(
        Guid creatorId, 
        [FromQuery] SocialMediaPlatform platform)
    {
        try
        {
            var recommendations = await _recommendationService.GetBestTimeToPostAsync(creatorId, platform);
            return Ok(recommendations);
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
    [ProducesResponseType(typeof(Dictionary<SocialMediaPlatform, List<PostTimeRecommendation>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Dictionary<SocialMediaPlatform, List<PostTimeRecommendation>>>> GetBestTimeToPostAllPlatforms(
        Guid creatorId)
    {
        try
        {
            var recommendations = await _recommendationService.GetBestTimeToPostAllPlatformsAsync(creatorId);
            return Ok(recommendations);
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
    [ProducesResponseType(typeof(List<ContentRecommendation>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ContentRecommendation>>> GetContentRecommendations(Guid creatorId)
    {
        try
        {
            var recommendations = await _recommendationService.GetContentRecommendationsAsync(creatorId);
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter recomendações de conteúdo");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("topics/{creatorId}")]
    public async Task<ActionResult<List<ContentRecommendation>>> GetTopicRecommendations(Guid creatorId)
    {
        try
        {
            var recommendations = await _recommendationService.GetTopicRecommendationsAsync(creatorId);
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter recomendações de tópicos");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("formats/{creatorId}")]
    public async Task<ActionResult<List<ContentRecommendation>>> GetFormatRecommendations(Guid creatorId)
    {
        try
        {
            var recommendations = await _recommendationService.GetFormatRecommendationsAsync(creatorId);
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter recomendações de formatos");
            return BadRequest(ex.Message);
        }
    }

    // Recomendações de hashtags
    [HttpGet("hashtags/{creatorId}")]
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
    public async Task<ActionResult<List<TrendTopic>>> GetTrendingTopics([FromQuery] SocialMediaPlatform platform)
    {
        try
        {
            var trends = await _recommendationService.GetTrendingTopicsAsync(platform);
            return Ok(trends);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter tópicos em tendência");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("trends/{creatorId}/niche")]
    public async Task<ActionResult<List<TrendTopic>>> GetNicheTrendingTopics(Guid creatorId)
    {
        try
        {
            var trends = await _recommendationService.GetNicheTrendingTopicsAsync(creatorId);
            return Ok(trends);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter tópicos em tendência para o nicho");
            return BadRequest(ex.Message);
        }
    }

    // Recomendações estratégicas
    [HttpGet("monetization/{creatorId}")]
    public async Task<ActionResult<List<ContentRecommendation>>> GetMonetizationRecommendations(Guid creatorId)
    {
        try
        {
            var recommendations = await _recommendationService.GetMonetizationRecommendationsAsync(creatorId);
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter recomendações de monetização");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("audience-growth/{creatorId}")]
    public async Task<ActionResult<List<ContentRecommendation>>> GetAudienceGrowthRecommendations(Guid creatorId)
    {
        try
        {
            var recommendations = await _recommendationService.GetAudienceGrowthRecommendationsAsync(creatorId);
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter recomendações de crescimento de audiência");
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("engagement/{creatorId}")]
    public async Task<ActionResult<List<ContentRecommendation>>> GetEngagementRecommendations(Guid creatorId)
    {
        try
        {
            var recommendations = await _recommendationService.GetEngagementImprovementRecommendationsAsync(creatorId);
            return Ok(recommendations);
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter recomendações de engajamento");
            return BadRequest(ex.Message);
        }
    }

    // Análise de conteúdo
    [HttpGet("analyze/{contentId}")]
    public async Task<ActionResult<ContentAnalysis>> AnalyzeContent(Guid contentId)
    {
        try
        {
            var analysis = await _recommendationService.AnalyzeContentPerformanceAsync(contentId);
            return Ok(analysis);
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
} 