using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MicroSaaS.IntegrationTests.Controllers
{
    [ApiController]
    [Route("api/v1/recommendation")]
    public class TestRecommendationController : ControllerBase
    {
        private readonly IRecommendationService _recommendationService;
        private readonly ILogger<TestRecommendationController> _logger;

        public TestRecommendationController(
            IRecommendationService recommendationService,
            ILogger<TestRecommendationController> logger)
        {
            _recommendationService = recommendationService;
            _logger = logger;
        }

        [HttpGet("best-times/{creatorId}")]
        public async Task<ActionResult<List<PostTimeRecommendation>>> GetBestTimeToPost(
            Guid creatorId, 
            [FromQuery] SocialMediaPlatform platform)
        {
            _logger.LogInformation($"Obtendo melhores horários para criador {creatorId} na plataforma {platform}");
            
            try
            {
                var recommendations = await _recommendationService.GetBestTimeToPostAsync(creatorId, platform);
                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter recomendações de horário: {ex.Message}");
                return StatusCode(500, "Erro interno ao processar recomendações");
            }
        }

        [HttpGet("content/{creatorId}")]
        public async Task<ActionResult<List<ContentRecommendation>>> GetContentRecommendations(Guid creatorId)
        {
            _logger.LogInformation($"Obtendo recomendações de conteúdo para criador {creatorId}");
            
            try
            {
                var recommendations = await _recommendationService.GetContentRecommendationsAsync(creatorId);
                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter recomendações de conteúdo: {ex.Message}");
                return StatusCode(500, "Erro interno ao processar recomendações");
            }
        }
    }
} 