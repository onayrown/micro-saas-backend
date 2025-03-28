using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using MicroSaaS.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PerformanceController : ControllerBase
{
    private readonly ISocialMediaIntegrationService _socialMediaService;
    private readonly IContentCreatorRepository _creatorRepository;
    private readonly ISocialMediaAccountRepository _accountRepository;

    public PerformanceController(
        ISocialMediaIntegrationService socialMediaService,
        IContentCreatorRepository creatorRepository,
        ISocialMediaAccountRepository accountRepository)
    {
        _socialMediaService = socialMediaService;
        _creatorRepository = creatorRepository;
        _accountRepository = accountRepository;
    }

    [HttpGet("insights/{creatorId}/{platform}")]
    public async Task<ActionResult<List<ContentPerformanceDto>>> GetInsights(
        Guid creatorId, 
        SocialMediaPlatform platform,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        // Buscar a conta de mídia social
        var accounts = await _accountRepository.GetByPlatformAsync(creatorId, platform);
        var account = accounts.FirstOrDefault();
        
        if (account == null)
            return NotFound("Nenhuma conta encontrada para a plataforma especificada");

        // Buscar dados de desempenho da plataforma
        var insights = await _socialMediaService.GetAccountPerformanceAsync(
            account.Id,
            startDate,
            endDate);

        return Ok(insights);
    }

    [HttpGet("best-times/{creatorId}/{platform}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<ActionResult<List<MicroSaaS.Shared.Models.PostTimeRecommendation>>> GetBestPostTimes(
        Guid creatorId,
        SocialMediaPlatform platform)
    {
        // Buscar a conta de mídia social
        var accounts = await _accountRepository.GetByPlatformAsync(creatorId, platform);
        var account = accounts.FirstOrDefault();
        
        if (account == null)
            return NotFound("Nenhuma conta encontrada para a plataforma especificada");

        // Analisar melhores horários com base em desempenho passado
        var recommendations = await _socialMediaService.GetBestPostingTimesAsync(account.Id);

        return Ok(recommendations);
    }

    [HttpGet("summary/{creatorId}")]
    public async Task<ActionResult<PerformanceSummary>> GetPerformanceSummary(
        Guid creatorId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        // Buscar todas as contas do criador
        var accounts = await _accountRepository.GetByCreatorIdAsync(creatorId);
        
        if (!accounts.Any())
            return NotFound("Nenhuma conta de mídia social encontrada");
            
        var instagramAccount = accounts.FirstOrDefault(a => a.Platform == SocialMediaPlatform.Instagram);
        var youtubeAccount = accounts.FirstOrDefault(a => a.Platform == SocialMediaPlatform.YouTube);
        var tiktokAccount = accounts.FirstOrDefault(a => a.Platform == SocialMediaPlatform.TikTok);
        
        // Inicializar listas vazias
        var instagram = new List<ContentPerformanceDto>();
        var youtube = new List<ContentPerformanceDto>();
        var tiktok = new List<ContentPerformanceDto>();
        
        // Buscar dados de todas as plataformas
        if (instagramAccount != null)
        {
            instagram = (await _socialMediaService.GetAccountPerformanceAsync(
                instagramAccount.Id, startDate, endDate)).ToList();
        }
        
        if (youtubeAccount != null)
        {
            youtube = (await _socialMediaService.GetAccountPerformanceAsync(
                youtubeAccount.Id, startDate, endDate)).ToList();
        }
        
        if (tiktokAccount != null)
        {
            tiktok = (await _socialMediaService.GetAccountPerformanceAsync(
                tiktokAccount.Id, startDate, endDate)).ToList();
        }

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
            ByPlatform = new List<PlatformPerformance>()
        };
        
        if (instagram.Any())
        {
            summary.ByPlatform.Add(new PlatformPerformance
            {
                Platform = SocialMediaPlatform.Instagram,
                Views = instagram.Sum(i => i.Views),
                Likes = instagram.Sum(i => i.Likes),
                Comments = instagram.Sum(i => i.Comments),
                Shares = instagram.Sum(i => i.Shares)
            });
        }
        
        if (youtube.Any())
        {
            summary.ByPlatform.Add(new PlatformPerformance
            {
                Platform = SocialMediaPlatform.YouTube,
                Views = youtube.Sum(i => i.Views),
                Likes = youtube.Sum(i => i.Likes),
                Comments = youtube.Sum(i => i.Comments),
                Shares = youtube.Sum(i => i.Shares)
            });
        }
        
        if (tiktok.Any())
        {
            summary.ByPlatform.Add(new PlatformPerformance
            {
                Platform = SocialMediaPlatform.TikTok,
                Views = tiktok.Sum(i => i.Views),
                Likes = tiktok.Sum(i => i.Likes),
                Comments = tiktok.Sum(i => i.Comments),
                Shares = tiktok.Sum(i => i.Shares)
            });
        }

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