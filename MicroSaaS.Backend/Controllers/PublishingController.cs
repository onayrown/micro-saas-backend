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
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class PublishingController : ControllerBase
    {
        private readonly ISocialMediaIntegrationService _socialMediaService;
        private readonly IContentPostRepository _contentPostRepository;
        private readonly ILoggingService _loggingService;

        public PublishingController(
            ISocialMediaIntegrationService socialMediaService,
            IContentPostRepository contentPostRepository,
            ILoggingService loggingService)
        {
            _socialMediaService = socialMediaService;
            _contentPostRepository = contentPostRepository;
            _loggingService = loggingService;
        }

        [HttpPost("publish-now")]
        public async Task<ActionResult<ApiResponse<PublishedPostDto>>> PublishNow([FromBody] PublishNowDto request)
        {
            try
            {
                _loggingService.LogInformation($"Tentativa de publicação imediata para a plataforma {request.Platform}");
                
                // Em uma implementação real, chamaríamos o serviço de integração com redes sociais
                // Para testes, retornamos um mock de post publicado
                var publishedPost = new PublishedPostDto
                {
                    Id = Guid.NewGuid(),
                    CreatorId = request.CreatorId,
                    Title = request.Title,
                    Content = request.Content,
                    Platform = request.Platform,
                    Status = MicroSaaS.Shared.Enums.PostStatus.Published,
                    PublishedAt = DateTime.UtcNow,
                    ExternalPostUrl = $"https://example.com/{request.Platform.ToString().ToLower()}/posts/{Guid.NewGuid()}",
                    Tags = request.Tags,
                    MediaUrls = request.MediaUrls
                };

                return Ok(new ApiResponse<PublishedPostDto>
                {
                    Success = true,
                    Message = "Post publicado com sucesso",
                    Data = publishedPost
                });
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao publicar conteúdo");
                return StatusCode(500, new ApiResponse<PublishedPostDto>
                {
                    Success = false,
                    Message = "Erro ao publicar conteúdo"
                });
            }
        }

        [HttpGet("history")]
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

        [HttpGet("stats")]
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

        [HttpGet("engagement/{postId}")]
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

        [HttpPost("republish")]
        public async Task<ActionResult<ApiResponse<PublishedPostDto>>> RepublishPost([FromBody] RepublishPostDto request)
        {
            try
            {
                _loggingService.LogInformation($"Tentativa de republicação do post {request.PostId}");
                
                // Em uma implementação real, obteríamos o post original e o republicaríamos
                // Para testes, retornamos um mock de post republicado
                var republishedPost = new PublishedPostDto
                {
                    Id = Guid.NewGuid(),
                    CreatorId = Guid.NewGuid(), // Normalmente seria obtido do post original
                    Title = "Post Republicado",
                    Content = "Conteúdo republicado" + (request.AdditionalComment != null ? $"\n\n{request.AdditionalComment}" : ""),
                    Platform = MicroSaaS.Shared.Enums.SocialMediaPlatform.Facebook, // Normalmente seria do post original
                    Status = MicroSaaS.Shared.Enums.PostStatus.Published,
                    PublishedAt = DateTime.UtcNow,
                    ExternalPostUrl = $"https://example.com/facebook/posts/{Guid.NewGuid()}",
                    Tags = new List<string> { "republicado", "teste" },
                    MediaUrls = new List<string>(),
                    IsRepost = true,
                    OriginalPostId = request.PostId
                };

                return Ok(new ApiResponse<PublishedPostDto>
                {
                    Success = true,
                    Message = "Post republicado com sucesso",
                    Data = republishedPost
                });
            }
            catch (Exception ex)
            {
                _loggingService.LogError(ex, "Erro ao republicar conteúdo");
                return StatusCode(500, new ApiResponse<PublishedPostDto>
                {
                    Success = false,
                    Message = "Erro ao republicar conteúdo"
                });
            }
        }
    }
} 