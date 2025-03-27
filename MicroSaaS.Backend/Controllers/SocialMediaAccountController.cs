using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SocialMediaAccountController : ControllerBase
{
    private readonly IContentCreatorRepository _creatorRepository;
    private readonly ISocialMediaIntegrationService _socialMediaService;

    public SocialMediaAccountController(
        IContentCreatorRepository creatorRepository,
        ISocialMediaIntegrationService socialMediaService)
    {
        _creatorRepository = creatorRepository;
        _socialMediaService = socialMediaService;
    }

    [HttpGet("{creatorId}")]
    public async Task<ActionResult<List<SocialMediaAccount>>> GetAccounts(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            return NotFound();

        return Ok(creator.SocialMediaAccounts);
    }

    [HttpPost("{creatorId}")]
    public async Task<ActionResult<SocialMediaAccount>> AddAccount(
        Guid creatorId, 
        [FromBody] AddSocialMediaRequest request)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            return NotFound();

        // Iniciar o processo de autenticação com a plataforma de mídia social
        var authUrl = _socialMediaService.GetAuthorizationUrl(
            request.Platform, 
            Url.Action(nameof(HandleCallback), new { creatorId, platform = request.Platform })
        );

        return Ok(new { authorizationUrl = authUrl });
    }

    [HttpGet("callback")]
    [AllowAnonymous]
    public async Task<IActionResult> HandleCallback(
        [FromQuery] Guid creatorId, 
        [FromQuery] SocialMediaPlatform platform,
        [FromQuery] string code)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            return NotFound();

        // Trocar o código de autorização por um token de acesso
        var accessToken = await _socialMediaService.ExchangeCodeForToken(platform, code);

        // Adicionar a conta à lista de contas do criador
        if (creator.SocialMediaAccounts == null)
            creator.SocialMediaAccounts = new List<SocialMediaAccount>();

        creator.SocialMediaAccounts.Add(new SocialMediaAccount
        {
            Id = Guid.NewGuid(),
            Platform = platform,
            AccessToken = accessToken,
            CreatedAt = DateTime.UtcNow
        });

        await _creatorRepository.UpdateAsync(creator);

        // Redirecionar para a aplicação frontend
        return Redirect($"https://seuapp.com/connect/success?platform={platform}");
    }

    [HttpDelete("{creatorId}/{accountId}")]
    public async Task<IActionResult> RemoveAccount(Guid creatorId, Guid accountId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null || creator.SocialMediaAccounts == null)
            return NotFound();

        var account = creator.SocialMediaAccounts.FirstOrDefault(a => a.Id == accountId);
        if (account == null)
            return NotFound();

        creator.SocialMediaAccounts.Remove(account);
        await _creatorRepository.UpdateAsync(creator);

        return NoContent();
    }
}

public class AddSocialMediaRequest
{
    public SocialMediaPlatform Platform { get; set; }
} 