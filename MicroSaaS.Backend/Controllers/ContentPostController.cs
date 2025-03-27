using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using Microsoft.AspNetCore.Mvc;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContentPostController : ControllerBase
{
    private readonly IContentPostRepository _repository;

    public ContentPostController(IContentPostRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("scheduled/{creatorId}")]
    public async Task<ActionResult<List<ContentPost>>> GetScheduledPosts(Guid creatorId)
    {
        var posts = await _repository.GetScheduledPostsAsync(creatorId);
        return Ok(posts);
    }

    [HttpPost]
    public async Task<ActionResult<ContentPost>> Create([FromBody] ContentPost post)
    {
        post.Id = Guid.NewGuid();
        post.Status = PostStatus.Scheduled;
        await _repository.AddAsync(post);

        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContentPost>> GetById(Guid id)
    {
        var post = await _repository.GetByIdAsync(id);

        if (post == null)
            return NotFound();

        return Ok(post);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] ContentPost post)
    {
        var existingPost = await _repository.GetByIdAsync(id);

        if (existingPost == null)
            return NotFound();

        post.Id = id;
        await _repository.UpdateAsync(post);

        return NoContent();
    }
}
