using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using MicroSaaS.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AnalyticsController : ControllerBase
{
    private readonly ISocialMediaIntegrationService _socialMediaService;
    private readonly ISocialMediaAccountRepository _accountRepository;
    private readonly IContentPostRepository _postRepository;

    public AnalyticsController(
        ISocialMediaIntegrationService socialMediaService,
        ISocialMediaAccountRepository accountRepository,
        IContentPostRepository postRepository)
    {
        _socialMediaService = socialMediaService;
        _accountRepository = accountRepository;
        _postRepository = postRepository;
    }

    [HttpGet("post/{postId}")]
    public async Task<ActionResult<List<ContentPerformanceDto>>> GetPostAnalytics(string postId)
    {
        try
        {
            var analytics = await _socialMediaService.GetPostPerformanceAsync(postId);
            return Ok(analytics);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("creator/{creatorId}")]
    public async Task<ActionResult<List<ContentPerformanceDto>>> GetCreatorAnalytics(
        Guid creatorId,
        [FromQuery] SocialMediaPlatform platform,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        try
        {
            // Buscar a conta de mídia social
            var accounts = await _accountRepository.GetByPlatformAsync(creatorId, platform);
            var account = accounts.FirstOrDefault();
            
            if (account == null)
                return NotFound("Nenhuma conta encontrada para a plataforma especificada");

            var analytics = await _socialMediaService.GetAccountPerformanceAsync(
                account.Id,
                startDate,
                endDate);
                
            return Ok(analytics);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Erro ao buscar analytics" });
        }
    }
} 