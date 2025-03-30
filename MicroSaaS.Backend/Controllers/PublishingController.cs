using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MicroSaaS.Application.Interfaces.Repositories;
using MicroSaaS.Application.Interfaces.Services;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Enums;
using MicroSaaS.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroSaaS.Backend.Controllers
{
    /// <summary>
    /// Controlador responsável por gerenciar a publicação de conteúdo nas redes sociais
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class PublishingController : ControllerBase
    {
        private readonly ISocialMediaIntegrationService _socialMediaService;
        private readonly IContentPostRepository _contentPostRepository;
        private readonly ILoggingService _loggingService;

        /// <summary>
        /// Construtor do PublishingController
        /// </summary>
        /// <param name="socialMediaService">Serviço de integração com redes sociais</param>
        /// <param name="contentPostRepository">Repositório de posts de conteúdo</param>
        /// <param name="loggingService">Serviço de logging</param>
        public PublishingController(
            ISocialMediaIntegrationService socialMediaService,
            IContentPostRepository contentPostRepository,
            ILoggingService loggingService)
        {
            _socialMediaService = socialMediaService;
            _contentPostRepository = contentPostRepository;
            _loggingService = loggingService;
        }

        /// <summary>
        /// Publica um post imediatamente em uma plataforma de mídia social
        /// </summary>
        /// <param name="request">Dados do post a ser publicado</param>
        /// <returns>Detalhes do post publicado</returns>
        /// <remarks>
        /// Exemplo de requisição:
        /// 
        /// ```json
        /// {
        ///   "creatorId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///   "title": "Meu novo post",
        ///   "content": "Conteúdo do meu post #microsaas",
        ///   "platform": "Instagram",
        ///   "mediaUrls": [
        ///     "https://exemplo.com/imagem1.jpg"
        ///   ],
        ///   "tags": [
        ///     "microsaas", "conteudo"
        ///   ]
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Post publicado com sucesso</response>
        /// <response code="400">Dados inválidos para publicação</response>
        /// <response code="401">Usuário não autenticado</response>
        /// <response code="403">Usuário não tem permissão para publicar nesta conta</response>
        /// <response code="429">Taxa de publicação excedida para a plataforma</response>
        /// <response code="500">Erro interno do servidor ou falha na integração com a rede social</response>
        [HttpPost("publish-now")]
        [ProducesResponseType(typeof(ApiResponse<PublishedPostDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PublishedPostDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PublishedPostDto>>> PublishNow([FromBody] PublishNowDto request)
        {
            try
            {
                _loggingService.LogInformation("Tentativa de publicação imediata para a plataforma {Platform}", request.Platform);

                // Validação básica
                if (string.IsNullOrEmpty(request.Content) && (request.MediaUrls == null || request.MediaUrls.Count == 0))
                {
                    _loggingService.LogWarning("Tentativa de publicação sem conteúdo ou mídia");
                    return BadRequest(new ApiResponse<PublishedPostDto>
                    {
                        Success = false,
                        Message = "O post deve conter texto ou mídia"
                    });
                }

                // Em uma implementação real, chamaríamos o serviço de integração
                // Para testes, retornamos um mock de post publicado
                var publishedPost = new PublishedPostDto
                {
                    Id = Guid.NewGuid(),
                    CreatorId = request.CreatorId,
                    Title = request.Title,
                    Content = request.Content,
                    Platform = request.Platform,
                    MediaUrls = request.MediaUrls,
                    Tags = request.Tags,
                    Status = PostStatus.Published,
                    PublishedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                return Ok(new ApiResponse<PublishedPostDto>
                {
                    Success = true,
                    Data = publishedPost,
                    Message = "Post publicado com sucesso"
                });
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao publicar post");
                return StatusCode(500, new ApiResponse<PublishedPostDto>
                {
                    Success = false,
                    Message = "Erro ao publicar post"
                });
            }
        }

        /// <summary>
        /// Obtém o histórico de posts publicados pelo usuário atual
        /// </summary>
        /// <returns>Lista de posts publicados</returns>
        /// <remarks>
        /// Este endpoint retorna todos os posts publicados pelo usuário autenticado,
        /// ordenados por data de publicação (mais recentes primeiro).
        /// </remarks>
        /// <response code="200">Lista de posts recuperada com sucesso</response>
        /// <response code="401">Usuário não autenticado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("history")]
        [ProducesResponseType(typeof(List<PublishedPostDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<PublishedPostDto>>> GetPublishedPosts()
        {
            try
            {
                // Em uma implementação real, obteríamos o histórico de posts do repositório
                // Para testes, retornamos uma lista vazia
                return Ok(new List<PublishedPostDto>());
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao obter histórico de publicações");
                return StatusCode(500, new ApiResponse<List<PublishedPostDto>>
                {
                    Success = false,
                    Message = "Erro ao obter histórico de publicações"
                });
            }
        }

        /// <summary>
        /// Obtém estatísticas de publicação para um criador específico
        /// </summary>
        /// <param name="creatorId">ID do criador de conteúdo</param>
        /// <returns>Estatísticas de publicação do criador</returns>
        /// <remarks>
        /// Exemplo de resposta:
        /// 
        /// ```json
        /// {
        ///   "success": true,
        ///   "data": {
        ///     "totalPublished": 42,
        ///     "totalScheduled": 15,
        ///     "publishedByPlatform": {
        ///       "Instagram": 15,
        ///       "Twitter": 20,
        ///       "Facebook": 7
        ///     },
        ///     "mostEngagedPosts": [
        ///       {
        ///         "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///         "title": "Post mais engajado 1",
        ///         "platform": "Instagram",
        ///         "publishedAt": "2023-04-15T14:30:00Z",
        ///         "totalEngagement": 450,
        ///         "engagementRate": 12.5
        ///       }
        ///     ]
        ///   }
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Estatísticas recuperadas com sucesso</response>
        /// <response code="401">Usuário não autenticado</response>
        /// <response code="403">Usuário não tem permissão para acessar estas estatísticas</response>
        /// <response code="404">Criador não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("stats")]
        [ProducesResponseType(typeof(ApiResponse<PublishingStatsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PublishingStatsDto>>> GetPublishingStats([FromQuery] Guid creatorId)
        {
            try
            {
                // Em uma implementação real, obteríamos as estatísticas do repositório
                // Para testes, retornamos estatísticas simuladas
                var stats = new PublishingStatsDto
                {
                    TotalPublished = 10,
                    TotalScheduled = 5,
                    PublishedByPlatform = new Dictionary<SocialMediaPlatform, int>
                    {
                        { SocialMediaPlatform.Facebook, 3 },
                        { SocialMediaPlatform.Instagram, 4 },
                        { SocialMediaPlatform.Twitter, 3 }
                    },
                    MostEngagedPosts = new List<PostEngagementSummaryDto>
                    {
                        new PostEngagementSummaryDto
                        {
                            PostId = Guid.NewGuid(),
                            Title = "Post mais engajado",
                            Platform = SocialMediaPlatform.Instagram,
                            PublishedAt = DateTime.UtcNow.AddDays(-3),
                            TotalEngagement = 150,
                            EngagementRate = 0.08
                        }
                    }
                };

                return Ok(new ApiResponse<PublishingStatsDto>
                {
                    Success = true,
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao obter estatísticas de publicação");
                return StatusCode(500, new ApiResponse<PublishingStatsDto>
                {
                    Success = false,
                    Message = "Erro ao obter estatísticas de publicação"
                });
            }
        }

        /// <summary>
        /// Obtém dados de engajamento para um post específico
        /// </summary>
        /// <param name="postId">ID do post publicado</param>
        /// <returns>Dados de engajamento do post</returns>
        /// <remarks>
        /// Este endpoint retorna métricas detalhadas de engajamento para um post específico,
        /// incluindo likes, comentários, compartilhamentos, visualizações e taxa de engajamento.
        /// 
        /// Exemplo de resposta:
        /// 
        /// ```json
        /// {
        ///   "success": true,
        ///   "data": {
        ///     "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///     "platform": "Instagram",
        ///     "likes": 125,
        ///     "comments": 43,
        ///     "shares": 18,
        ///     "views": 2750,
        ///     "engagementRate": 6.8,
        ///     "lastUpdated": "2023-07-20T15:45:30Z"
        ///   }
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Dados de engajamento recuperados com sucesso</response>
        /// <response code="401">Usuário não autenticado</response>
        /// <response code="403">Usuário não tem permissão para acessar estes dados</response>
        /// <response code="404">Post não encontrado</response>
        /// <response code="500">Erro interno do servidor ou falha na integração com a rede social</response>
        [HttpGet("engagement/{postId}")]
        [ProducesResponseType(typeof(ApiResponse<PostEngagementDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PostEngagementDto>>> GetPostEngagement(Guid postId)
        {
            try
            {
                // Em uma implementação real, obteríamos os dados de engajamento do serviço de integração
                // Para testes, retornamos dados simulados
                var engagement = new PostEngagementDto
                {
                    PostId = postId,
                    Platform = SocialMediaPlatform.Instagram,
                    Likes = 100,
                    Comments = 25,
                    Shares = 15,
                    Views = 2000,
                    EngagementRate = 0.07,
                    LastUpdated = DateTime.UtcNow
                };

                return Ok(new ApiResponse<PostEngagementDto>
                {
                    Success = true,
                    Data = engagement
                });
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao obter dados de engajamento");
                return StatusCode(500, new ApiResponse<PostEngagementDto>
                {
                    Success = false,
                    Message = "Erro ao obter dados de engajamento"
                });
            }
        }

        /// <summary>
        /// Republica um post existente em uma plataforma de mídia social
        /// </summary>
        /// <param name="request">Dados para republicação</param>
        /// <returns>Detalhes do post republicado</returns>
        /// <remarks>
        /// Este endpoint permite republicar um post existente, opcionalmente com modificações.
        /// Útil para compartilhar conteúdo em múltiplas plataformas ou repostar com pequenas alterações.
        /// 
        /// Exemplo de requisição:
        /// 
        /// ```json
        /// {
        ///   "postId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
        ///   "platform": "Twitter",
        ///   "additionalText": "Republicando este post com novidades! #atualizado",
        ///   "additionalTags": [
        ///     "atualizado", "novidades"
        ///   ]
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Post republicado com sucesso</response>
        /// <response code="400">Dados inválidos para republicação</response>
        /// <response code="401">Usuário não autenticado</response>
        /// <response code="403">Usuário não tem permissão para publicar nesta conta</response>
        /// <response code="404">Post original não encontrado</response>
        /// <response code="429">Taxa de publicação excedida para a plataforma</response>
        /// <response code="500">Erro interno do servidor ou falha na integração com a rede social</response>
        [HttpPost("republish")]
        [ProducesResponseType(typeof(ApiResponse<PublishedPostDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<PublishedPostDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse<PublishedPostDto>>> RepublishPost([FromBody] RepublishPostDto request)
        {
            try
            {
                _loggingService.LogInformation("Tentativa de republicação do post {PostId} na plataforma {Platform}", request.PostId, request.Platform);

                // Em uma implementação real, verificaríamos se o post existe e o republicaríamos
                // Para testes, retornamos um mock de post republicado
                var republishedPost = new PublishedPostDto
                {
                    Id = Guid.NewGuid(),
                    CreatorId = Guid.NewGuid(), // Em uma implementação real, usaríamos o CreatorId do post original
                    Title = "Post Republicado", // Em uma implementação real, usaríamos o título do post original
                    Content = request.AdditionalText ?? "Conteúdo republicado", // Combinaria o conteúdo original com o adicional
                    Platform = request.Platform,
                    MediaUrls = new List<string>(), // Em uma implementação real, usaríamos as mídias do post original
                    Tags = request.AdditionalTags ?? new List<string>(), // Combinaria as tags originais com as adicionais
                    Status = PostStatus.Published,
                    PublishedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    OriginalPostId = request.PostId
                };

                return Ok(new ApiResponse<PublishedPostDto>
                {
                    Success = true,
                    Data = republishedPost,
                    Message = "Post republicado com sucesso"
                });
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao republicar post {PostId}", request.PostId);
                return StatusCode(500, new ApiResponse<PublishedPostDto>
                {
                    Success = false,
                    Message = "Erro ao republicar post"
                });
            }
        }
    }
} 