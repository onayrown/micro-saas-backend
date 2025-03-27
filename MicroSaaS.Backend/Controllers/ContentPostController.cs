using MicroSaaS.Application.DTOs.ContentPost;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Validators;
using MicroSaaS.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContentPostController : ControllerBase
{
    private readonly IContentPostRepository _repository;
    private readonly IContentCreatorRepository _creatorRepository;
    private readonly IContentPlanningService _contentPlanningService;

    public ContentPostController(
        IContentPostRepository repository,
        IContentCreatorRepository creatorRepository,
        IContentPlanningService contentPlanningService)
    {
        _repository = repository;
        _creatorRepository = creatorRepository;
        _contentPlanningService = contentPlanningService;
    }

    [HttpGet("scheduled/{creatorId}")]
    public async Task<ActionResult<List<ContentPostDto>>> GetScheduledPosts(Guid creatorId)
    {
        var posts = await _repository.GetScheduledByCreatorIdAsync(creatorId);

        return Ok(posts.Select(post => new ContentPostDto
        {
            Id = post.Id,
            CreatorId = post.CreatorId,
            Title = post.Title,
            Content = post.Content,
            Platform = post.Platform,
            MediaUrl = post.MediaUrl,
            ScheduledTime = post.ScheduledTime,
            Status = post.Status,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        }).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<ContentPostDto>> Create([FromBody] CreatePostRequest request)
    {
        var creator = await _creatorRepository.GetByIdAsync(request.CreatorId);
        if (creator == null)
            return BadRequest(new { message = "Criador de conteúdo não encontrado" });

        var post = await _contentPlanningService.CreatePostAsync(new Domain.Entities.ContentPost
        {
            Id = Guid.NewGuid(),
            CreatorId = request.CreatorId,
            Creator = creator,
            Title = request.Title ?? "Novo Post",
            Content = request.Content,
            Platform = request.Platform,
            MediaUrl = request.MediaUrl,
            ScheduledTime = request.ScheduledTime ?? DateTime.UtcNow.AddDays(1),
            Status = PostStatus.Scheduled,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        return CreatedAtAction(nameof(GetById), new { id = post.Id }, new ContentPostDto
        {
            Id = post.Id,
            CreatorId = post.CreatorId,
            Title = post.Title,
            Content = post.Content,
            Platform = post.Platform,
            MediaUrl = post.MediaUrl,
            ScheduledTime = post.ScheduledTime,
            Status = post.Status,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContentPostDto>> GetById(Guid id)
    {
        var post = await _repository.GetByIdAsync(id);

        if (post == null)
            return NotFound();

        return Ok(new ContentPostDto
        {
            Id = post.Id,
            CreatorId = post.CreatorId,
            Title = post.Title,
            Content = post.Content,
            Platform = post.Platform,
            MediaUrl = post.MediaUrl,
            ScheduledTime = post.ScheduledTime,
            Status = post.Status,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePostRequest request)
    {
        var existingPost = await _repository.GetByIdAsync(id);

        if (existingPost == null)
            return NotFound();

        request.Platform = existingPost.Platform;

        existingPost.Title = request.Title ?? existingPost.Title;
        existingPost.Content = request.Content;
        existingPost.MediaUrl = request.MediaUrl;
        existingPost.ScheduledTime = request.ScheduledTime ?? existingPost.ScheduledTime;
        existingPost.Status = request.Status;
        existingPost.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(existingPost);
        return NoContent();
    }
}

public class CreatePostRequest
{
    public Guid CreatorId { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public SocialMediaPlatform Platform { get; set; }
    public string MediaUrl { get; set; }
    public DateTime? ScheduledTime { get; set; }
}

public class UpdatePostRequest
{
    public string Title { get; set; }
    public string Content { get; set; }
    public string MediaUrl { get; set; }
    public DateTime? ScheduledTime { get; set; }
    public PostStatus Status { get; set; }
    public SocialMediaPlatform Platform { get; set; }
}
