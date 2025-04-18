using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Application.DTOs.Checklist;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class ContentChecklistController : ControllerBase
{
    private readonly IContentPlanningService _contentPlanningService;

    public ContentChecklistController(
        IContentPlanningService contentPlanningService)
    {
        _contentPlanningService = contentPlanningService;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ContentChecklistDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ContentChecklistDto>> GetById(Guid id)
    {
        var checklistDto = await _contentPlanningService.GetChecklistByIdAsync(id);
        if (checklistDto == null)
            return NotFound();

        return Ok(checklistDto);
    }

    [HttpGet("creator/{creatorId}")]
    [ProducesResponseType(typeof(List<ContentChecklistDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ContentChecklistDto>>> GetByCreatorId(Guid creatorId)
    {
        var checklistsDto = await _contentPlanningService.GetChecklistsByCreatorIdAsync(creatorId);
        return Ok(checklistsDto);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ContentChecklistDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ContentChecklistDto>> Create([FromBody] CreateChecklistRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var checklistDto = await _contentPlanningService.CreateChecklistAsync(request.CreatorId, request.Title, request.Description);
            return CreatedAtAction(nameof(GetById), new { id = checklistDto.Id }, checklistDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao criar checklist.");
        }
    }

    [HttpPost("{checklistId}/items")]
    [ProducesResponseType(typeof(ContentChecklistDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ContentChecklistDto>> AddItem(Guid checklistId, [FromBody] AddChecklistItemRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var updatedChecklistDto = await _contentPlanningService.AddChecklistItemAsync(
                checklistId,
                request.Description,
                request.IsRequired);

            if (updatedChecklistDto == null)
                return NotFound("Checklist não encontrada para adicionar item.");

            return Ok(updatedChecklistDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao adicionar item ao checklist.");
        }
    }

    [HttpPut("{checklistId}/items/{itemId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItem(Guid checklistId, Guid itemId, [FromBody] UpdateItemRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            await _contentPlanningService.UpdateChecklistItemAsync(checklistId, itemId, request.IsCompleted);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao atualizar item do checklist.");
        }
    }

    [HttpPut("{checklistId}/items/{itemId}/duedate")]
    [ProducesResponseType(typeof(ChecklistItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChecklistItemDto>> SetItemDueDate(Guid checklistId, Guid itemId, [FromBody] SetDueDateRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var itemDto = await _contentPlanningService.SetItemDueDateAsync(checklistId, itemId, request.DueDate);
            return Ok(itemDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao definir data do item.");
        }
    }

    [HttpPut("{checklistId}/items/{itemId}/reminder")]
    [ProducesResponseType(typeof(ChecklistItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChecklistItemDto>> SetItemReminder(Guid checklistId, Guid itemId, [FromBody] SetReminderRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var itemDto = await _contentPlanningService.SetItemReminderAsync(checklistId, itemId, request.ReminderDate);
            return Ok(itemDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao definir lembrete do item.");
        }
    }

    [HttpPut("{checklistId}/items/{itemId}/priority")]
    [ProducesResponseType(typeof(ChecklistItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ChecklistItemDto>> SetItemPriority(Guid checklistId, Guid itemId, [FromBody] SetPriorityRequestDto request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var itemDto = await _contentPlanningService.SetItemPriorityAsync(checklistId, itemId, request.Priority);
            return Ok(itemDto);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao definir prioridade do item.");
        }
    }

    [HttpGet("creator/{creatorId}/upcoming")]
    [ProducesResponseType(typeof(IEnumerable<ChecklistItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ChecklistItemDto>>> GetUpcomingDeadlines(Guid creatorId, [FromQuery] int daysAhead = 7)
    {
        try
        {
            var itemsDto = await _contentPlanningService.GetItemsWithUpcomingDeadlinesAsync(creatorId, daysAhead);
            return Ok(itemsDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao buscar itens futuros.");
        }
    }

    [HttpGet("creator/{creatorId}/due")]
    [ProducesResponseType(typeof(IEnumerable<ChecklistItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ChecklistItemDto>>> GetDueItems(Guid creatorId)
    {
        try
        {
            var itemsDto = await _contentPlanningService.GetDueItemsAsync(creatorId);
            return Ok(itemsDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao buscar itens do dia.");
        }
    }

    [HttpGet("creator/{creatorId}/overdue")]
    [ProducesResponseType(typeof(IEnumerable<ChecklistItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ChecklistItemDto>>> GetOverdueItems(Guid creatorId)
    {
        try
        {
            var itemsDto = await _contentPlanningService.GetOverdueItemsAsync(creatorId);
            return Ok(itemsDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno ao buscar itens atrasados.");
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _contentPlanningService.DeleteChecklistAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}

