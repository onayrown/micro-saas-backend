using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentCreatorController : ControllerBase
{
    private readonly IContentCreatorRepository _repository;

    public ContentCreatorController(IContentCreatorRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContentCreator>> GetById(Guid id)
    {
        var creator = await _repository.GetByIdAsync(id);

        if (creator == null)
            return NotFound();

        return Ok(creator);
    }

    [HttpPost]
    public async Task<ActionResult<ContentCreator>> Create([FromBody] ContentCreator creator)
    {
        creator.Id = Guid.NewGuid();
        await _repository.AddAsync(creator);

        return CreatedAtAction(nameof(GetById), new { id = creator.Id }, creator);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ContentCreator creator)
    {
        var existingCreator = await _repository.GetByIdAsync(id);

        if (existingCreator == null)
            return NotFound();

        creator.Id = id;
        await _repository.UpdateAsync(creator);

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
}
