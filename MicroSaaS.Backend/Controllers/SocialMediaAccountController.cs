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
    private readonly ISocialMediaAccountRepository _socialMediaAccountRepository;

    public SocialMediaAccountController(
        IContentCreatorRepository creatorRepository,
        ISocialMediaIntegrationService socialMediaService,
        ISocialMediaAccountRepository socialMediaAccountRepository)
    {
        _creatorRepository = creatorRepository;
        _socialMediaService = socialMediaService;
        _socialMediaAccountRepository = socialMediaAccountRepository;
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
        var authUrl = await _socialMediaService.GetAuthUrlAsync(request.Platform);

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
        var account = await _socialMediaService.HandleAuthCallbackAsync(platform, code);
        
        // Definir o creatorId para a conta
        account.CreatorId = creatorId;

        // Adicionar a conta ao repositório
        await _socialMediaAccountRepository.AddAsync(account);

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