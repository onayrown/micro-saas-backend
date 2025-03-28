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

    [HttpPost("{checklistId}/items")]
    public async Task<ActionResult<ContentChecklist>> AddItem(Guid checklistId, [FromBody] AddChecklistItemRequest request)
    {
        try
        {
            var updatedChecklist = await _contentPlanningService.AddChecklistItemAsync(
                checklistId, 
                request.Description, 
                request.IsRequired);
                
            return Ok(updatedChecklist);
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

    [HttpPut("items/{itemId}/duedate")]
    public async Task<ActionResult<ChecklistItem>> SetItemDueDate(Guid itemId, [FromBody] SetDueDateRequest request)
    {
        try
        {
            var item = await _contentPlanningService.SetItemDueDateAsync(itemId, request.DueDate);
            return Ok(item);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPut("items/{itemId}/reminder")]
    public async Task<ActionResult<ChecklistItem>> SetItemReminder(Guid itemId, [FromBody] SetReminderRequest request)
    {
        try
        {
            var item = await _contentPlanningService.SetItemReminderAsync(itemId, request.ReminderDate);
            return Ok(item);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpPut("items/{itemId}/priority")]
    public async Task<ActionResult<ChecklistItem>> SetItemPriority(Guid itemId, [FromBody] SetPriorityRequest request)
    {
        try
        {
            var item = await _contentPlanningService.SetItemPriorityAsync(itemId, request.Priority);
            return Ok(item);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("creator/{creatorId}/upcoming")]
    public async Task<ActionResult<IEnumerable<ChecklistItem>>> GetUpcomingDeadlines(Guid creatorId, [FromQuery] int daysAhead = 7)
    {
        try
        {
            var items = await _contentPlanningService.GetItemsWithUpcomingDeadlinesAsync(creatorId, daysAhead);
            return Ok(items);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("creator/{creatorId}/due")]
    public async Task<ActionResult<IEnumerable<ChecklistItem>>> GetDueItems(Guid creatorId)
    {
        try
        {
            var items = await _contentPlanningService.GetDueItemsAsync(creatorId);
            return Ok(items);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
    
    [HttpGet("creator/{creatorId}/overdue")]
    public async Task<ActionResult<IEnumerable<ChecklistItem>>> GetOverdueItems(Guid creatorId)
    {
        try
        {
            var items = await _contentPlanningService.GetOverdueItemsAsync(creatorId);
            return Ok(items);
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

public class AddChecklistItemRequest
{
    public string Description { get; set; }
    public bool IsRequired { get; set; }
}

public class UpdateItemRequest
{
    public bool IsCompleted { get; set; }
}

public class SetDueDateRequest
{
    public DateTime DueDate { get; set; }
}

public class SetReminderRequest
{
    public DateTime ReminderDate { get; set; }
}

public class SetPriorityRequest
{
    public TaskPriority Priority { get; set; }
} 