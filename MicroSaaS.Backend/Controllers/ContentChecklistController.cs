using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContentChecklistController : ControllerBase
{
    private readonly IContentPlanningService _contentPlanningService;
    private readonly IContentChecklistRepository _checklistRepository;

    public ContentChecklistController(
        IContentPlanningService contentPlanningService,
        IContentChecklistRepository checklistRepository)
    {
        _contentPlanningService = contentPlanningService;
        _checklistRepository = checklistRepository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContentChecklist>> GetById(Guid id)
    {
        var checklist = await _checklistRepository.GetByIdAsync(id);
        if (checklist == null)
            return NotFound();

        return Ok(checklist);
    }

    [HttpGet("creator/{creatorId}")]
    public async Task<ActionResult<List<ContentChecklist>>> GetByCreatorId(Guid creatorId)
    {
        var checklists = await _checklistRepository.GetByCreatorIdAsync(creatorId);
        return Ok(checklists);
    }

    [HttpPost]
    public async Task<ActionResult<ContentChecklist>> Create([FromBody] CreateChecklistRequest request)
    {
        try
        {
            var checklist = await _contentPlanningService.CreateChecklistAsync(request.CreatorId, request.Title);
            return CreatedAtAction(nameof(GetById), new { id = checklist.Id }, checklist);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{checklistId}/items/{itemId}")]
    public async Task<IActionResult> UpdateItem(Guid checklistId, Guid itemId, [FromBody] UpdateItemRequest request)
    {
        try
        {
            await _contentPlanningService.UpdateChecklistItemAsync(itemId, request.IsCompleted);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var checklist = await _checklistRepository.GetByIdAsync(id);
        if (checklist == null)
            return NotFound();

        await _checklistRepository.DeleteAsync(id);
        return NoContent();
    }
}

public class CreateChecklistRequest
{
    public Guid CreatorId { get; set; }
    public string Title { get; set; }
}

public class UpdateItemRequest
{
    public bool IsCompleted { get; set; }
} 