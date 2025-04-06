using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/creators")]
[Authorize]
public class ContentCreatorController : ControllerBase
{
    private readonly IContentCreatorRepository _repository;
    private readonly ILoggingService _loggingService;

    public ContentCreatorController(IContentCreatorRepository repository, ILoggingService loggingService)
    {
        _repository = repository;
        _loggingService = loggingService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContentCreatorDto>> GetById(Guid id)
    {
        var creator = await _repository.GetByIdAsync(id);

        if (creator == null)
            return NotFound();

        // Converter a entidade de domínio para DTO
        var creatorDto = new ContentCreatorDto
        {
            Id = creator.Id,
            Name = creator.Name,
            Email = creator.Email,
            Username = creator.Username,
            Bio = creator.Bio,
            ProfileImageUrl = creator.ProfileImageUrl,
            WebsiteUrl = creator.WebsiteUrl,
            CreatedAt = creator.CreatedAt,
            UpdatedAt = creator.UpdatedAt,
            TotalFollowers = creator.TotalFollowers,
            TotalPosts = creator.TotalPosts,
            SocialMediaAccounts = creator.SocialMediaAccounts?.Select(a => new SocialMediaAccountDto
            {
                Id = a.Id,
                Platform = a.Platform,
                Username = a.Username,
                Followers = a.FollowersCount,
                IsVerified = false
            }).ToList() ?? new List<SocialMediaAccountDto>()
        };

        return Ok(creatorDto);
    }

    [HttpPost]
    public async Task<ActionResult<ContentCreatorDto>> Create([FromBody] ContentCreatorDto creatorDto)
    {
        var creator = new ContentCreator
        {
            Id = Guid.NewGuid(),
            Name = creatorDto.Name,
            Email = creatorDto.Email,
            Username = creatorDto.Username,
            Bio = creatorDto.Bio,
            ProfileImageUrl = creatorDto.ProfileImageUrl,
            WebsiteUrl = creatorDto.WebsiteUrl
        };

        await _repository.AddAsync(creator);

        creatorDto.Id = creator.Id;
        return CreatedAtAction(nameof(GetById), new { id = creator.Id }, creatorDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ContentCreatorDto creatorDto)
    {
        var existingCreator = await _repository.GetByIdAsync(id);

        if (existingCreator == null)
            return NotFound();

        existingCreator.Name = creatorDto.Name;
        existingCreator.Email = creatorDto.Email;
        existingCreator.Username = creatorDto.Username;
        existingCreator.Bio = creatorDto.Bio;
        existingCreator.ProfileImageUrl = creatorDto.ProfileImageUrl;
        existingCreator.WebsiteUrl = creatorDto.WebsiteUrl;
        existingCreator.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(existingCreator);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var creator = await _repository.GetByIdAsync(id);

        if (creator == null)
            return NotFound();

        await _repository.DeleteAsync(id);

        return NoContent();
    }

    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<ContentCreatorDto>>> GetCurrentCreator()
    {
        try
        {
            // Log 1: Tentando obter a claim
            _loggingService.LogInformation("GetCurrentCreator: Tentando obter ClaimTypes.NameIdentifier.");
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            _loggingService.LogInformation("GetCurrentCreator: Valor da claim NameIdentifier obtido: '{UserIdClaim}'", userIdClaim); // Logar o valor bruto

            // Log 2: Verificando e parseando a claim
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                // Log 2a: Falha na obtenção/parse
                _loggingService.LogWarning("GetCurrentCreator: Não foi possível encontrar ou parsear o ID do usuário da claim NameIdentifier. Valor bruto: '{UserIdClaim}'", userIdClaim);
                return Unauthorized(new ApiResponse<ContentCreatorDto> { Success = false, Message = "Usuário não autenticado ou token inválido." });
            }

            // Log 3: Sucesso no parse, logando o Guid
            _loggingService.LogInformation("GetCurrentCreator: UserId parseado com sucesso: {UserId}. Buscando no repositório...", userId);

            // Buscar o ContentCreator pelo UserId
            // Assumindo que o repositório tem um método como GetByUserIdAsync ou similar.
            // Se não tiver, precisaremos adaptar ou buscar pelo ID principal se UserId == CreatorId.
            // Vamos assumir por enquanto que o ID do Claim é o ID do ContentCreator.
            var creator = await _repository.GetByIdAsync(userId); 

            // Log 4: Resultado da busca no repositório
            if (creator == null)
            {
                // Log 4a: Não encontrado
                _loggingService.LogWarning("GetCurrentCreator: ContentCreator NÃO encontrado no repositório para UserId: {UserId}", userId);
                return NotFound(new ApiResponse<ContentCreatorDto> { Success = false, Message = "Perfil do criador de conteúdo não encontrado." });
            }

             // Log 5: Encontrado (implícito ao não entrar no if acima)
             _loggingService.LogInformation("GetCurrentCreator: ContentCreator ENCONTRADO no repositório para UserId: {UserId}. Mapeando para DTO.", userId);

            // Converter a entidade real para DTO
            var creatorDto = new ContentCreatorDto
            {
                Id = creator.Id,
                Name = creator.Name,
                Email = creator.Email, // Certifique-se de que o Email está na entidade ContentCreator
                Username = creator.Username, // Certifique-se de que o Username está na entidade ContentCreator
                Bio = creator.Bio,
                ProfileImageUrl = creator.ProfileImageUrl,
                WebsiteUrl = creator.WebsiteUrl,
                CreatedAt = creator.CreatedAt,
                UpdatedAt = creator.UpdatedAt,
                TotalFollowers = creator.TotalFollowers, // Adicionar se existir na entidade
                TotalPosts = creator.TotalPosts,       // Adicionar se existir na entidade
                SocialMediaAccounts = creator.SocialMediaAccounts?.Select(a => new SocialMediaAccountDto
                {
                    Id = a.Id,
                    Platform = a.Platform,
                    Username = a.Username,
                    Followers = a.FollowersCount,
                    IsVerified = false // Ajustar se a entidade tiver essa informação
                }).ToList() ?? new List<SocialMediaAccountDto>()
                // Platforms = ??? // Mapear se necessário e se existir na entidade
            };

            _loggingService.LogInformation("GetCurrentCreator: Dados do criador encontrados para UserId: {UserId}", userId);
            return Ok(new ApiResponse<ContentCreatorDto>
            {
                Success = true,
                Data = creatorDto // Retornar o DTO real
            });
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter criador de conteúdo atual");
            return StatusCode(500, new ApiResponse<ContentCreatorDto>
            {
                Success = false,
                Message = "Erro interno ao obter perfil do criador de conteúdo atual"
            });
        }
    }
}
