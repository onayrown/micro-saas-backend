using MicroSaaS.Application.DTOs.ContentPost;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Application.Mappers;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Domain.Validators;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicroSaaS.Backend.Controllers;

/// <summary>
/// Controlador responsável por gerenciar posts de conteúdo dos criadores
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class ContentPostController : ControllerBase
{
    private readonly IContentPostRepository _repository;
    private readonly IContentCreatorRepository _creatorRepository;
    private readonly IContentPlanningService _contentPlanningService;

    /// <summary>
    /// Construtor do ContentPostController
    /// </summary>
    /// <param name="repository">Repositório de posts de conteúdo</param>
    /// <param name="creatorRepository">Repositório de criadores de conteúdo</param>
    /// <param name="contentPlanningService">Serviço de planejamento de conteúdo</param>
    public ContentPostController(
        IContentPostRepository repository,
        IContentCreatorRepository creatorRepository,
        IContentPlanningService contentPlanningService)
    {
        _repository = repository;
        _creatorRepository = creatorRepository;
        _contentPlanningService = contentPlanningService;
    }

    /// <summary>
    /// Obtém todos os posts agendados para um criador específico
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <returns>Lista de posts agendados</returns>
    /// <remarks>
    /// Exemplo de resposta:
    /// 
    /// ```json
    /// [
    ///   {
    ///     "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///     "title": "Post Agendado para Instagram",
    ///     "content": "Conteúdo do post #hashtag",
    ///     "platform": "Instagram",
    ///     "scheduledDate": "2023-06-15T14:30:00Z",
    ///     "status": "Scheduled",
    ///     "mediaUrls": [
    ///       "https://exemplo.com/imagem1.jpg"
    ///     ],
    ///     "tags": [
    ///       "lifestyle", "dicas"
    ///     ],
    ///     "creatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///     "createdAt": "2023-06-10T08:15:00Z",
    ///     "updatedAt": "2023-06-10T08:15:00Z"
    ///   }
    /// ]
    /// ```
    /// </remarks>
    /// <response code="200">Lista de posts agendados recuperada com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para acessar posts deste criador</response>
    /// <response code="404">Criador não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("scheduled/{creatorId}")]
    [ProducesResponseType(typeof(List<ContentPostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ContentPostDto>>> GetScheduledPosts(Guid creatorId)
    {
        try
        {
            var creator = await _creatorRepository.GetByIdAsync(creatorId);
            if (creator == null)
                return NotFound(new ApiResponse<List<ContentPostDto>>
                {
                    Success = false,
                    Message = "Criador de conteúdo não encontrado"
                });

            var posts = await _repository.GetScheduledByCreatorIdAsync(creatorId);
            var postsDto = posts.Select(ContentPostMapper.ToDto).ToList();

            return Ok(new ApiResponse<List<ContentPostDto>>
            {
                Success = true,
                Data = postsDto,
                Message = "Posts agendados recuperados com sucesso"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<List<ContentPostDto>>
            {
                Success = false,
                Message = "Erro ao recuperar posts agendados"
            });
        }
    }

    /// <summary>
    /// Cria um novo post de conteúdo
    /// </summary>
    /// <param name="request">Dados do novo post</param>
    /// <returns>Post criado</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    /// ```json
    /// {
    ///   "creatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///   "title": "Meu novo post",
    ///   "content": "Conteúdo do meu post #hashtag",
    ///   "platform": "Instagram",
    ///   "scheduledDate": "2023-06-15T14:30:00Z",
    ///   "mediaUrls": [
    ///     "https://exemplo.com/imagem1.jpg"
    ///   ],
    ///   "tags": [
    ///     "lifestyle", "dicas"
    ///   ]
    /// }
    /// ```
    /// </remarks>
    /// <response code="201">Post criado com sucesso</response>
    /// <response code="400">Dados inválidos para criação do post</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para criar posts para este criador</response>
    /// <response code="404">Criador de conteúdo não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ContentPostDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<ContentPostDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ContentPostDto>> Create([FromBody] CreatePostRequest request)
    {
        try
        {
            var creator = await _creatorRepository.GetByIdAsync(request.CreatorId);
            if (creator == null)
                return BadRequest(new ApiResponse<ContentPostDto>
                {
                    Success = false,
                    Message = "Criador de conteúdo não encontrado"
                });

            // Validação de conteúdo
            if (string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Content))
            {
                return BadRequest(new ApiResponse<ContentPostDto>
                {
                    Success = false,
                    Message = "Título e conteúdo são obrigatórios"
                });
            }

            var postEntity = ContentPostMapper.ToEntity(request);
            postEntity.Creator = creator;

            var post = await _contentPlanningService.CreatePostAsync(postEntity);
            var postDto = ContentPostMapper.ToDto(post);

            return CreatedAtAction(
                nameof(GetById), 
                new { id = post.Id }, 
                new ApiResponse<ContentPostDto>
                {
                    Success = true,
                    Data = postDto,
                    Message = "Post criado com sucesso"
                });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ContentPostDto>
            {
                Success = false,
                Message = "Erro ao criar post"
            });
        }
    }

    /// <summary>
    /// Obtém um post específico pelo ID
    /// </summary>
    /// <param name="id">ID do post</param>
    /// <returns>Detalhes do post</returns>
    /// <remarks>
    /// Exemplo de resposta:
    /// 
    /// ```json
    /// {
    ///   "success": true,
    ///   "data": {
    ///     "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///     "title": "Meu post",
    ///     "content": "Conteúdo do post #hashtag",
    ///     "platform": "Instagram",
    ///     "scheduledDate": "2023-06-15T14:30:00Z",
    ///     "status": "Scheduled",
    ///     "mediaUrls": [
    ///       "https://exemplo.com/imagem1.jpg"
    ///     ],
    ///     "tags": [
    ///       "lifestyle", "dicas"
    ///     ],
    ///     "creatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///     "createdAt": "2023-06-10T08:15:00Z",
    ///     "updatedAt": "2023-06-10T08:15:00Z"
    ///   },
    ///   "message": "Post recuperado com sucesso"
    /// }
    /// ```
    /// </remarks>
    /// <response code="200">Post recuperado com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para acessar este post</response>
    /// <response code="404">Post não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ContentPostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ContentPostDto>> GetById(Guid id)
    {
        try
        {
            var post = await _repository.GetByIdAsync(id);

            if (post == null)
                return NotFound(new ApiResponse<ContentPostDto>
                {
                    Success = false,
                    Message = "Post não encontrado"
                });

            var postDto = ContentPostMapper.ToDto(post);
            return Ok(new ApiResponse<ContentPostDto>
            {
                Success = true,
                Data = postDto,
                Message = "Post recuperado com sucesso"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<ContentPostDto>
            {
                Success = false,
                Message = "Erro ao recuperar post"
            });
        }
    }

    /// <summary>
    /// Atualiza um post existente
    /// </summary>
    /// <param name="id">ID do post a ser atualizado</param>
    /// <param name="request">Dados atualizados do post</param>
    /// <returns>Sem conteúdo</returns>
    /// <remarks>
    /// Exemplo de requisição:
    /// 
    /// ```json
    /// {
    ///   "title": "Título atualizado",
    ///   "content": "Conteúdo atualizado #hashtag",
    ///   "platform": "Instagram",
    ///   "scheduledDate": "2023-06-16T14:30:00Z",
    ///   "mediaUrls": [
    ///     "https://exemplo.com/imagem1.jpg",
    ///     "https://exemplo.com/imagem2.jpg"
    ///   ],
    ///   "tags": [
    ///     "lifestyle", "dicas", "novidade"
    ///   ]
    /// }
    /// ```
    /// </remarks>
    /// <response code="204">Post atualizado com sucesso</response>
    /// <response code="400">Dados inválidos para atualização</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para atualizar este post</response>
    /// <response code="404">Post não encontrado</response>
    /// <response code="409">Conflito ao atualizar post (já publicado ou em processamento)</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePostRequest request)
    {
        try
        {
            var existingPost = await _repository.GetByIdAsync(id);

            if (existingPost == null)
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Post não encontrado"
                });

            // Verificação se o post já foi publicado ou está em processo
            if (existingPost.Status == PostStatus.Published || existingPost.Status == PostStatus.Processing)
            {
                return StatusCode(409, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Não é possível atualizar um post que já foi publicado ou está em processamento"
                });
            }

            ContentPostMapper.UpdateEntity(existingPost, request);
            await _repository.UpdateAsync(existingPost);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Erro ao atualizar post"
            });
        }
    }
}
