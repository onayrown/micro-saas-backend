using MicroSaaS.Application.DTOs.ContentPost;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Application.Mappers;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Validators;
using MicroSaaS.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Backend.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
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
        var postsDto = posts.Select(ContentPostMapper.ToDto).ToList();

        return Ok(postsDto);
    }

    [HttpPost]
    public async Task<ActionResult<ContentPostDto>> Create([FromBody] CreatePostRequest request)
    {
        var creator = await _creatorRepository.GetByIdAsync(request.CreatorId);
        if (creator == null)
            return BadRequest(new { message = "Criador de conteúdo não encontrado" });

        var postEntity = ContentPostMapper.ToEntity(request);
        postEntity.Creator = creator;

        var post = await _contentPlanningService.CreatePostAsync(postEntity);
        var postDto = ContentPostMapper.ToDto(post);

        return CreatedAtAction(nameof(GetById), new { id = post.Id }, postDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContentPostDto>> GetById(Guid id)
    {
        var post = await _repository.GetByIdAsync(id);

        if (post == null)
            return NotFound();

        var postDto = ContentPostMapper.ToDto(post);
        return Ok(postDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePostRequest request)
    {
        var existingPost = await _repository.GetByIdAsync(id);

        if (existingPost == null)
            return NotFound();

        ContentPostMapper.UpdateEntity(existingPost, request);
        await _repository.UpdateAsync(existingPost);
        
        return NoContent();
    }
}
