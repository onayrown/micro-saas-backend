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
            // Em uma implementação real, obteríamos o ID do usuário a partir do token
            // Para testes, retornamos um creator simulado
            var creator = new ContentCreatorDto
            {
                Id = Guid.NewGuid(),
                Name = "Creator Teste",
                Email = "teste@microsaas.com",
                Bio = "Criador de conteúdo para testes",
                ProfileImageUrl = "https://example.com/avatar.jpg",
                WebsiteUrl = "https://example.com/creator",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                TotalFollowers = 5000,
                TotalPosts = 120,
                Platforms = new List<SocialMediaPlatform> { SocialMediaPlatform.Instagram, SocialMediaPlatform.Twitter },
                SocialMediaAccounts = new List<SocialMediaAccountDto>
                {
                    new SocialMediaAccountDto
                    {
                        Id = Guid.NewGuid(),
                        Platform = SocialMediaPlatform.Instagram,
                        Username = "testecreator",
                        Followers = 1000,
                        IsVerified = true
                    }
                }
            };

            return Ok(new ApiResponse<ContentCreatorDto>
            {
                Success = true,
                Data = creator
            });
        }
        catch (Exception ex)
        {
            _loggingService.LogError(ex, "Erro ao obter criador de conteúdo atual");
            return StatusCode(500, new ApiResponse<ContentCreatorDto>
            {
                Success = false,
                Message = "Erro ao obter criador de conteúdo atual"
            });
        }
    }
}
