using MicroSaaS.Application.DTOs.ContentPost;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
// Alias para o namespace específico do ContentPost DTO
using PostDTOs = MicroSaaS.Application.DTOs.ContentPost;

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
    private readonly IContentPostService _contentPostService;
    private readonly ILogger<ContentPostController> _logger;

    public ContentPostController(IContentPostService contentPostService, ILogger<ContentPostController> logger)
    {
        _contentPostService = contentPostService;
        _logger = logger;
    }

    private Guid GetRequestingUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
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
    [ProducesResponseType(typeof(ApiResponse<List<PostDTOs.ContentPostDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<PostDTOs.ContentPostDto>>>> GetScheduledPosts(Guid creatorId)
    {
        var requestingUserId = GetRequestingUserId();

        _logger.LogInformation("Buscando posts agendados para CreatorId: {CreatorId}", creatorId);
        var response = await _contentPostService.GetScheduledPostsAsync(creatorId);

        if (!response.Success)
        {
            _logger.LogWarning("Falha ao buscar posts agendados para {CreatorId}: {Message}", creatorId, response.Message);
            var statusCode = response.Message?.Contains("não encontrado") ?? false ? 404 :
                             response.Message?.Contains("permissão") ?? false ? 403 : 500;
            return StatusCode(statusCode, response);
        }

        _logger.LogInformation("Posts agendados para {CreatorId} recuperados com sucesso.", creatorId);
        return Ok(response);
    }

    /// <summary>
    /// Obtém posts de um criador, opcionalmente filtrados por status.
    /// </summary>
    /// <param name="creatorId">ID do criador de conteúdo</param>
    /// <param name="status">Status do post para filtrar (opcional: Draft, Scheduled, Published, Failed)</param>
    /// <returns>Lista de posts</returns>
    /// <response code="200">Posts recuperados com sucesso</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para acessar posts deste criador</response>
    /// <response code="404">Criador não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<PostDTOs.ContentPostDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<List<PostDTOs.ContentPostDto>>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<List<PostDTOs.ContentPostDto>>>> GetPosts([FromQuery] Guid creatorId, [FromQuery] string? status = null)
    {
        var requestingUserId = GetRequestingUserId();

        _logger.LogInformation("Buscando posts para CreatorId: {CreatorId} com Status: {Status}", creatorId, status ?? "Todos");
        var response = await _contentPostService.GetPostsAsync(creatorId, status);

        if (!response.Success)
        {
            _logger.LogWarning("Falha ao buscar posts para {CreatorId} com Status {Status}: {Message}", creatorId, status ?? "Todos", response.Message);
            var statusCode = response.Message?.Contains("não encontrado") ?? false ? 404 :
                             response.Message?.Contains("permissão") ?? false ? 403 :
                             response.Message?.Contains("inválido") ?? false ? 400 : 500;
            return StatusCode(statusCode, response);
        }

        _logger.LogInformation("Posts para {CreatorId} recuperados com sucesso.", creatorId);
        return Ok(response);
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
    [ProducesResponseType(typeof(ApiResponse<PostDTOs.ContentPostDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<PostDTOs.ContentPostDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PostDTOs.ContentPostDto>>> Create([FromBody] PostDTOs.CreatePostRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(new ApiResponse<ContentPostDto> { Success = false, Message = "Dados inválidos" });
        
        var requestingUserId = GetRequestingUserId();

        _logger.LogInformation("Tentando criar post para CreatorId: {CreatorId} por User: {RequestingUserId}", request.CreatorId, requestingUserId);
        var response = await _contentPostService.CreatePostAsync(request, requestingUserId);

        if (!response.Success)
        {
            _logger.LogWarning("Falha ao criar post para {CreatorId}: {Message}", request.CreatorId, response.Message);
            var statusCode = response.Message?.Contains("não encontrado") ?? false ? 404 :
                             response.Message?.Contains("permissão") ?? false ? 403 : 500;
            return StatusCode(statusCode, response);
        }

        _logger.LogInformation("Post {PostId} criado com sucesso para {CreatorId}", response.Data.Id, request.CreatorId);
        return CreatedAtAction(nameof(GetById), new { id = response.Data.Id }, response);
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
    [ProducesResponseType(typeof(ApiResponse<PostDTOs.ContentPostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PostDTOs.ContentPostDto>>> GetById(Guid id)
    {
        var requestingUserId = GetRequestingUserId();
        
        _logger.LogInformation("Buscando post com ID: {PostId}", id);
        var response = await _contentPostService.GetPostByIdAsync(id, requestingUserId);

        if (!response.Success)
        {
            _logger.LogWarning("Falha ao buscar post {PostId}: {Message}", id, response.Message);
            var statusCode = response.Message?.Contains("não encontrado") ?? false ? 404 :
                             response.Message?.Contains("permissão") ?? false ? 403 : 500;
            return StatusCode(statusCode, response);
        }

        _logger.LogInformation("Post {PostId} recuperado com sucesso.", id);
        return Ok(response);
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
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] PostDTOs.UpdatePostRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(new ApiResponse<bool> { Success = false, Message = "Dados inválidos" });

        var requestingUserId = GetRequestingUserId();

        _logger.LogInformation("Tentando atualizar post {PostId} por User: {RequestingUserId}", id, requestingUserId);
        var response = await _contentPostService.UpdatePostAsync(id, request, requestingUserId);

        if (!response.Success)
        {
            _logger.LogWarning("Falha ao atualizar post {PostId}: {Message}", id, response.Message);
            var statusCode = response.Message?.Contains("não encontrado") ?? false ? 404 :
                             response.Message?.Contains("permissão") ?? false ? 403 :
                             response.Message?.Contains("conflito") ?? false ? 409 : 500;
            return StatusCode(statusCode, response);
        }

        _logger.LogInformation("Post {PostId} atualizado com sucesso.", id);
        return Ok(response);
    }

    /// <summary>
    /// Deleta um post de conteúdo.
    /// </summary>
    /// <param name="id">ID do post a ser deletado</param>
    /// <returns>Confirmação da deleção</returns>
    /// <response code="200">Post deletado com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para deletar este post</response>
    /// <response code="404">Post não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> DeletePost(Guid id)
    {
        var requestingUserId = GetRequestingUserId();

        _logger.LogInformation("Tentando deletar post {PostId} por User: {RequestingUserId}", id, requestingUserId);
        var response = await _contentPostService.DeletePostAsync(id, requestingUserId);

        if (!response.Success)
        {
            _logger.LogWarning("Falha ao deletar post {PostId}: {Message}", id, response.Message);
            var statusCode = response.Message?.Contains("não encontrado") ?? false ? 404 :
                             response.Message?.Contains("permissão") ?? false ? 403 : 500;
            return StatusCode(statusCode, response);
        }

        _logger.LogInformation("Post {PostId} deletado com sucesso.", id);
        return Ok(response);
    }

    /// <summary>
    /// Publica um post de conteúdo imediatamente.
    /// </summary>
    /// <param name="id">ID do post a ser publicado</param>
    /// <returns>O post atualizado com status "Published"</returns>
    /// <response code="200">Post publicado com sucesso</response>
    /// <response code="400">Post já publicado ou em estado inválido para publicação</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão para publicar este post</response>
    /// <response code="404">Post não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("{id}/publish")]
    [ProducesResponseType(typeof(ApiResponse<PostDTOs.ContentPostDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<PostDTOs.ContentPostDto>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<PostDTOs.ContentPostDto>>> PublishPost(Guid id)
    {
        var requestingUserId = GetRequestingUserId();

        _logger.LogInformation("Tentando publicar post {PostId} por User: {RequestingUserId}", id, requestingUserId);
        var response = await _contentPostService.PublishPostAsync(id, requestingUserId);

        if (!response.Success)
        {
            _logger.LogWarning("Falha ao publicar post {PostId}: {Message}", id, response.Message);
            var statusCode = response.Message?.Contains("não encontrado") ?? false ? 404 :
                             response.Message?.Contains("permissão") ?? false ? 403 :
                             response.Message?.Contains("inválido") ?? false ? 400 : 500;
            return StatusCode(statusCode, response);
        }

        _logger.LogInformation("Post {PostId} publicado com sucesso.", id);
        return Ok(response);
    }

    /// <summary>
    /// Obtém todos os posts de um criador (Endpoint alternativo/legado?).
    /// </summary>
    [HttpGet("creator/{creatorId}")]
    [ProducesResponseType(typeof(ApiResponse<List<PostDTOs.ContentPostDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<List<PostDTOs.ContentPostDto>>>> GetByCreator(Guid creatorId)
    {
        var requestingUserId = GetRequestingUserId();

        _logger.LogInformation("Buscando todos os posts para CreatorId: {CreatorId}", creatorId);
        var response = await _contentPostService.GetPostsByCreatorAsync(creatorId, requestingUserId);

        if (!response.Success)
        {
            _logger.LogWarning("Falha ao buscar posts para {CreatorId} via GetByCreator: {Message}", creatorId, response.Message);
            var statusCode = response.Message?.Contains("não encontrado") ?? false ? 404 :
                             response.Message?.Contains("permissão") ?? false ? 403 : 500;
            return StatusCode(statusCode, response);
        }
        
        _logger.LogInformation("Todos os posts para {CreatorId} recuperados com sucesso via GetByCreator.", creatorId);
        return Ok(response);
    }
}
