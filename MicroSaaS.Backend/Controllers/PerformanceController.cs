using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PerformanceController : ControllerBase
{
    private readonly ISocialMediaIntegrationService _socialMediaService;

    public PerformanceController(ISocialMediaIntegrationService socialMediaService)
    {
        _socialMediaService = socialMediaService;
    }

    [HttpGet("insights/{creatorId}/{platform}")]
    public async Task<ActionResult<List<ContentPerformance>>> GetInsights(
        Guid creatorId, 
        SocialMediaPlatform platform,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        // Buscar dados de desempenho da plataforma
        var insights = await _socialMediaService.GetContentInsightsAsync(
            creatorId,
            platform,
            startDate,
            endDate);

        return Ok(insights);
    }

    [HttpGet("best-times/{creatorId}/{platform}")]
    public async Task<ActionResult<List<PostTimeRecommendation>>> GetBestPostTimes(
        Guid creatorId,
        SocialMediaPlatform platform)
    {
        // Analisar melhores horários com base em desempenho passado
        var recommendations = await _socialMediaService.GetBestPostTimesAsync(
            creatorId,
            platform);

        return Ok(recommendations);
    }

    [HttpGet("summary/{creatorId}")]
    public async Task<ActionResult<PerformanceSummary>> GetPerformanceSummary(
        Guid creatorId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        // Obter dados de todas as plataformas e agregar
        var instagram = await _socialMediaService.GetContentInsightsAsync(
            creatorId, 
            SocialMediaPlatform.Instagram, 
            startDate, 
            endDate);

        var youtube = await _socialMediaService.GetContentInsightsAsync(
            creatorId, 
            SocialMediaPlatform.YouTube, 
            startDate, 
            endDate);

        var tiktok = await _socialMediaService.GetContentInsightsAsync(
            creatorId, 
            SocialMediaPlatform.TikTok, 
            startDate, 
            endDate);

        // Calcular métricas agregadas
        var totalViews = instagram.Sum(i => i.Views) + 
                         youtube.Sum(i => i.Views) + 
                         tiktok.Sum(i => i.Views);

        var totalLikes = instagram.Sum(i => i.Likes) + 
                        youtube.Sum(i => i.Likes) + 
                        tiktok.Sum(i => i.Likes);

        var totalComments = instagram.Sum(i => i.Comments) + 
                           youtube.Sum(i => i.Comments) + 
                           tiktok.Sum(i => i.Comments);

        var totalShares = instagram.Sum(i => i.Shares) + 
                         youtube.Sum(i => i.Shares) + 
                         tiktok.Sum(i => i.Shares);

        // Calcular métricas por plataforma
        var summary = new PerformanceSummary
        {
            TotalViews = totalViews,
            TotalLikes = totalLikes,
            TotalComments = totalComments,
            TotalShares = totalShares,
            ByPlatform = new List<PlatformPerformance>
            {
                new PlatformPerformance
                {
                    Platform = SocialMediaPlatform.Instagram,
                    Views = instagram.Sum(i => i.Views),
                    Likes = instagram.Sum(i => i.Likes),
                    Comments = instagram.Sum(i => i.Comments),
                    Shares = instagram.Sum(i => i.Shares)
                },
                new PlatformPerformance
                {
                    Platform = SocialMediaPlatform.YouTube,
                    Views = youtube.Sum(i => i.Views),
                    Likes = youtube.Sum(i => i.Likes),
                    Comments = youtube.Sum(i => i.Comments),
                    Shares = youtube.Sum(i => i.Shares)
                },
                new PlatformPerformance
                {
                    Platform = SocialMediaPlatform.TikTok,
                    Views = tiktok.Sum(i => i.Views),
                    Likes = tiktok.Sum(i => i.Likes),
                    Comments = tiktok.Sum(i => i.Comments),
                    Shares = tiktok.Sum(i => i.Shares)
                }
            }
        };

        return Ok(summary);
    }
}

public class PerformanceSummary
{
    public long TotalViews { get; set; }
    public long TotalLikes { get; set; }
    public long TotalComments { get; set; }
    public long TotalShares { get; set; }
    public List<PlatformPerformance> ByPlatform { get; set; }
}

public class PlatformPerformance
{
    public SocialMediaPlatform Platform { get; set; }
    public long Views { get; set; }
    public long Likes { get; set; }
    public long Comments { get; set; }
    public long Shares { get; set; }
} 