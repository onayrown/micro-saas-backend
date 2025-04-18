using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/creators")]
[Authorize]
public class ContentCreatorController : ControllerBase
{
    private readonly IContentCreatorService _contentCreatorService;
    private readonly ILoggingService _loggingService;

    public ContentCreatorController(IContentCreatorService contentCreatorService, ILoggingService loggingService)
    {
        _contentCreatorService = contentCreatorService;
        _loggingService = loggingService;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ContentCreatorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContentCreatorDto>> GetById(Guid id)
    {
        var creatorDto = await _contentCreatorService.GetByIdAsync(id);

        if (creatorDto == null)
            return NotFound();

        return Ok(creatorDto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContentCreatorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ContentCreatorDto>> Create([FromBody] ContentCreatorDto creatorDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var createdDto = await _contentCreatorService.CreateAsync(creatorDto);
            return CreatedAtAction(nameof(GetById), new { id = createdDto.Id }, createdDto);
        }
        catch (Exception ex)
        {
            _loggingService?.LogError(ex, "Erro ao criar ContentCreator");
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao criar criador.");
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ContentCreatorDto creatorDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var success = await _contentCreatorService.UpdateAsync(id, creatorDto);

        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _contentCreatorService.DeleteAsync(id);

        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<ContentCreatorDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<ContentCreatorDto>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<ContentCreatorDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<ContentCreatorDto>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<ContentCreatorDto>>> GetCurrentCreator()
    {
        _loggingService?.LogInformation("GetCurrentCreatorController: Tentando obter ClaimTypes.NameIdentifier.");
        var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        _loggingService?.LogInformation("GetCurrentCreatorController: Valor da claim NameIdentifier obtido: '{UserIdClaim}'", userIdClaim);

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            _loggingService?.LogWarning("GetCurrentCreatorController: Não foi possível encontrar ou parsear o ID do usuário da claim NameIdentifier. Valor bruto: '{UserIdClaim}'", userIdClaim);
            return Unauthorized(new ApiResponse<ContentCreatorDto> { Success = false, Message = "Usuário não autenticado ou token inválido." });
        }

        _loggingService?.LogInformation("GetCurrentCreatorController: UserId parseado com sucesso: {UserId}. Chamando serviço...", userId);

        var response = await _contentCreatorService.GetCurrentCreatorAsync(userId);

        if (!response.Success)
        {
            if (response.Message.Contains("não encontrado"))
                return NotFound(response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response);
        }

        _loggingService?.LogInformation("GetCurrentCreatorController: Resposta do serviço recebida com sucesso para UserId: {UserId}", userId);
        return Ok(response);
    }
}
