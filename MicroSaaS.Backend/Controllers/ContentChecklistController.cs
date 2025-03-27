using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContentChecklistController : ControllerBase
{
    private readonly IContentCreatorRepository _creatorRepository;

    public ContentChecklistController(IContentCreatorRepository creatorRepository)
    {
        _creatorRepository = creatorRepository;
    }

    [HttpGet("{creatorId}")]
    public async Task<ActionResult<List<ContentChecklist>>> GetChecklists(Guid creatorId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            return NotFound();

        // Aqui você precisaria implementar o acesso aos checklists
        // Como estamos usando MongoDB, você pode adicionar um repositório específico para checklists
        return Ok(new List<ContentChecklist>());
    }

    [HttpPost("{creatorId}")]
    public async Task<ActionResult<ContentChecklist>> CreateChecklist(
        Guid creatorId,
        [FromBody] CreateChecklistRequest request)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            return NotFound();

        var checklist = new ContentChecklist
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
            Items = request.Items?.Select(i => new ChecklistItem
            {
                Id = Guid.NewGuid(),
                Title = i.Title,
                IsCompleted = false
            }).ToList() ?? new List<ChecklistItem>()
        };

        // Aqui você precisaria implementar a persistência do checklist
        return CreatedAtAction(nameof(GetChecklist), new { creatorId, checklistId = checklist.Id }, checklist);
    }

    [HttpGet("{creatorId}/{checklistId}")]
    public async Task<ActionResult<ContentChecklist>> GetChecklist(Guid creatorId, Guid checklistId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            return NotFound();

        // Aqui você precisaria recuperar o checklist específico
        return Ok(new ContentChecklist());
    }

    [HttpPut("{creatorId}/{checklistId}")]
    public async Task<IActionResult> UpdateChecklist(
        Guid creatorId,
        Guid checklistId,
        [FromBody] UpdateChecklistRequest request)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            return NotFound();

        // Implementar a atualização do checklist
        return NoContent();
    }

    [HttpDelete("{creatorId}/{checklistId}")]
    public async Task<IActionResult> DeleteChecklist(Guid creatorId, Guid checklistId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            return NotFound();

        // Implementar a exclusão do checklist
        return NoContent();
    }

    [HttpPost("{creatorId}/{checklistId}/items")]
    public async Task<ActionResult<ChecklistItem>> AddChecklistItem(
        Guid creatorId,
        Guid checklistId,
        [FromBody] AddChecklistItemRequest request)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            return NotFound();

        var item = new ChecklistItem
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            IsCompleted = false
        };

        // Implementar a adição do item ao checklist
        return CreatedAtAction(nameof(GetChecklist), new { creatorId, checklistId }, item);
    }

    [HttpPut("{creatorId}/{checklistId}/items/{itemId}")]
    public async Task<IActionResult> UpdateChecklistItem(
        Guid creatorId,
        Guid checklistId,
        Guid itemId,
        [FromBody] UpdateChecklistItemRequest request)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            return NotFound();

        // Implementar a atualização do item
        return NoContent();
    }

    [HttpDelete("{creatorId}/{checklistId}/items/{itemId}")]
    public async Task<IActionResult> DeleteChecklistItem(
        Guid creatorId,
        Guid checklistId,
        Guid itemId)
    {
        var creator = await _creatorRepository.GetByIdAsync(creatorId);
        if (creator == null)
            return NotFound();

        // Implementar a exclusão do item
        return NoContent();
    }
}

public class CreateChecklistRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public List<ChecklistItemRequest> Items { get; set; }
}

public class UpdateChecklistRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
}

public class ChecklistItemRequest
{
    public string Title { get; set; }
}

public class AddChecklistItemRequest
{
    public string Title { get; set; }
}

public class UpdateChecklistItemRequest
{
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
} 