using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace MicroSaaS.IntegrationTests.Utils
{
    [ApiController]
    [Route("api/Analytics")]
    public class TestAnalyticsController : ControllerBase
    {
        private readonly ILogger<TestAnalyticsController> _logger;
        private static readonly List<ContentPerformanceDto> _performanceData = new List<ContentPerformanceDto>();
        private static readonly List<Guid> _accountIds = new List<Guid>();

        public TestAnalyticsController(ILogger<TestAnalyticsController> logger)
        {
            _logger = logger;
            
            // Inicializar dados de exemplo se a lista estiver vazia
            if (!_performanceData.Any())
            {
                InitializeTestData();
            }
        }

        [HttpGet("post/{postId}")]
        public async Task<ActionResult<List<ContentPerformanceDto>>> GetPostAnalytics(Guid postId)
        {
            _logger.LogInformation("TestAnalyticsController.GetPostAnalytics: Buscando analytics para post {PostId}", postId);
            
            // Verificação do token de autenticação
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.Contains("invalid-token"))
            {
                return StatusCode(403);
            }
            
            if (postId == Guid.Empty)
            {
                return BadRequest(new { message = "ID do post inválido" });
            }

            // Filtrar dados de performance para o post específico
            var performance = _performanceData.Where(p => p.PostId == postId).ToList();
            if (!performance.Any())
            {
                // Se não encontrar dados para o ID específico, cria dados de exemplo
                var newPerformance = CreateSamplePerformanceData(postId);
                _performanceData.Add(newPerformance);
                performance = new List<ContentPerformanceDto> { newPerformance };
            }

            return Ok(performance);
        }

        [HttpGet("creator/{creatorId}")]
        public async Task<ActionResult<List<ContentPerformanceDto>>> GetCreatorAnalytics(
            Guid creatorId,
            [FromQuery] SocialMediaPlatform platform,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            _logger.LogInformation("TestAnalyticsController.GetCreatorAnalytics: Buscando analytics para criador {CreatorId}, plataforma {Platform}", creatorId, platform);
            
            // Verificação do token de autenticação
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.Contains("invalid-token"))
            {
                return StatusCode(403);
            }

            if (creatorId == Guid.Empty)
            {
                return BadRequest(new { message = "ID do criador inválido" });
            }

            // Verificar se existe uma conta para a plataforma
            var accountExists = _accountIds.Any(id => id.ToString().StartsWith(creatorId.ToString().Substring(0, 8)));
            
            if (!accountExists)
            {
                // Se não encontrar conta para o criador, cria uma
                var accountId = Guid.NewGuid();
                _accountIds.Add(accountId);

                // Se for um ID de teste para caso de não encontrado
                if (creatorId == Guid.Parse("00000000-0000-0000-0000-000000000001"))
                {
                    return NotFound("Nenhuma conta encontrada para a plataforma especificada");
                }
            }

            // Filtrar dados por plataforma e período
            var performance = _performanceData
                .Where(p => p.Platform == platform && 
                           p.Date >= startDate && 
                           p.Date <= endDate)
                .ToList();

            if (!performance.Any())
            {
                // Se não encontrar dados para o filtro, cria dados de exemplo
                var result = new List<ContentPerformanceDto>();
                for (int i = 0; i < 5; i++)
                {
                    var date = startDate.AddDays(i * ((endDate - startDate).Days / 5));
                    var postId = Guid.NewGuid();
                    result.Add(CreateSamplePerformanceData(postId, date, platform));
                }
                
                foreach (var item in result)
                {
                    if (!_performanceData.Any(p => p.PostId == item.PostId))
                    {
                        _performanceData.Add(item);
                    }
                }
                
                return Ok(result);
            }

            return Ok(performance);
        }

        private void InitializeTestData()
        {
            // Dados de exemplo para diferentes plataformas
            _performanceData.Add(CreateSamplePerformanceData(Guid.NewGuid(), DateTime.Now.AddDays(-5), SocialMediaPlatform.Instagram));
            _performanceData.Add(CreateSamplePerformanceData(Guid.NewGuid(), DateTime.Now.AddDays(-10), SocialMediaPlatform.YouTube));
            _performanceData.Add(CreateSamplePerformanceData(Guid.NewGuid(), DateTime.Now.AddDays(-15), SocialMediaPlatform.TikTok));
            
            // Adicionar alguns IDs de conta de exemplo
            _accountIds.Add(Guid.NewGuid());
            _accountIds.Add(Guid.NewGuid());
        }

        private ContentPerformanceDto CreateSamplePerformanceData(Guid postId, DateTime? date = null, SocialMediaPlatform? platform = null)
        {
            var random = new Random();
            var postDate = date ?? DateTime.Now.AddDays(-random.Next(1, 30));
            var postPlatform = platform ?? (SocialMediaPlatform)random.Next(0, 3);
            
            return new ContentPerformanceDto
            {
                Id = Guid.NewGuid(),
                PostId = postId,
                AccountId = Guid.NewGuid(),
                Platform = postPlatform,
                Date = postDate,
                Views = random.Next(100, 10000),
                Likes = random.Next(10, 1000),
                Comments = random.Next(5, 200),
                Shares = random.Next(1, 100),
                EstimatedRevenue = random.Next(1, 100),
                CollectedAt = DateTime.Now
            };
        }
    }
} 