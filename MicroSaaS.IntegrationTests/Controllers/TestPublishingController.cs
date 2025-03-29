using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Models;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MicroSaaS.IntegrationTests.Controllers
{
    [ApiController]
    [Route("api/v1/publishing")]
    public class TestPublishingController : ControllerBase
    {
        private readonly ILogger<TestPublishingController> _logger;
        private static readonly List<PublishedPostDto> _publishedPosts = new List<PublishedPostDto>();

        public TestPublishingController(ILogger<TestPublishingController> logger)
        {
            _logger = logger;
        }

        [HttpGet("history")]
        public ActionResult<List<PublishedPostDto>> GetPublishedPosts()
        {
            _logger.LogInformation("TestPublishingController.GetPublishedPosts: Retornando histórico de publicações");
            return Ok(_publishedPosts);
        }

        [HttpPost("publish-now")]
        public ActionResult<ApiResponse<PublishedPostDto>> PublishNow([FromBody] PublishNowDto request)
        {
            _logger.LogInformation("TestPublishingController.PublishNow: Processando solicitação de publicação imediata");

            var post = new PublishedPostDto
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

            _publishedPosts.Add(post);

            return Ok(new ApiResponse<PublishedPostDto>
            {
                Success = true,
                Data = post,
                Message = "Publicação realizada com sucesso"
            });
        }

        [HttpGet("stats")]
        public ActionResult<ApiResponse<PublishingStatsDto>> GetPublishingStats([FromQuery] Guid creatorId)
        {
            _logger.LogInformation("TestPublishingController.GetPublishingStats: Retornando estatísticas para o criador {CreatorId}", creatorId);

            var stats = new PublishingStatsDto
            {
                CreatorId = creatorId,
                TotalPublished = 42,
                TotalScheduled = 15,
                PublishedByPlatform = new Dictionary<SocialMediaPlatform, int>
                {
                    { SocialMediaPlatform.Instagram, 15 },
                    { SocialMediaPlatform.Twitter, 20 },
                    { SocialMediaPlatform.Facebook, 7 }
                },
                MostEngagedPosts = new List<PostEngagementSummaryDto>
                {
                    new PostEngagementSummaryDto
                    {
                        PostId = Guid.NewGuid(),
                        Title = "Post mais engajado 1",
                        Platform = SocialMediaPlatform.Instagram,
                        PublishedAt = DateTime.UtcNow.AddDays(-5),
                        TotalEngagement = 450,
                        EngagementRate = 12.5
                    },
                    new PostEngagementSummaryDto
                    {
                        PostId = Guid.NewGuid(),
                        Title = "Post mais engajado 2",
                        Platform = SocialMediaPlatform.Twitter,
                        PublishedAt = DateTime.UtcNow.AddDays(-2),
                        TotalEngagement = 320,
                        EngagementRate = 8.7
                    }
                }
            };

            return Ok(new ApiResponse<PublishingStatsDto>
            {
                Success = true,
                Data = stats,
                Message = "Estatísticas de publicação recuperadas com sucesso"
            });
        }

        [HttpGet("engagement/{postId}")]
        public ActionResult<ApiResponse<PostEngagementDto>> GetPostEngagement(Guid postId)
        {
            _logger.LogInformation("TestPublishingController.GetPostEngagement: Retornando dados de engajamento para o post {PostId}", postId);

            var engagement = new PostEngagementDto
            {
                PostId = postId,
                Platform = SocialMediaPlatform.Instagram,
                Likes = 125,
                Comments = 43,
                Shares = 18,
                Views = 2750,
                EngagementRate = 6.8,
                LastUpdated = DateTime.UtcNow
            };

            return Ok(new ApiResponse<PostEngagementDto>
            {
                Success = true,
                Data = engagement,
                Message = "Dados de engajamento recuperados com sucesso"
            });
        }

        [HttpPost("republish")]
        public ActionResult<ApiResponse<PublishedPostDto>> RepublishPost([FromBody] RepublishPostDto request)
        {
            _logger.LogInformation("TestPublishingController.RepublishPost: Republicando post {PostId}", request.PostId);

            var republishedPost = new PublishedPostDto
            {
                Id = Guid.NewGuid(),
                CreatorId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // ID fixo do criador de teste
                Title = "Republicação de Post",
                Content = "Este é um post republicado via teste de integração",
                Platform = SocialMediaPlatform.Twitter,
                MediaUrls = new List<string>(),
                Tags = new List<string> { "republicação", "teste" },
                Status = PostStatus.Published,
                PublishedAt = DateTime.UtcNow,
                OriginalPostId = request.PostId,
                IsRepost = true,
                CreatedAt = DateTime.UtcNow,
                ExternalPostId = null,
                ExternalPostUrl = null
            };

            _publishedPosts.Add(republishedPost);

            return Ok(new ApiResponse<PublishedPostDto>
            {
                Success = true,
                Data = republishedPost,
                Message = "Post republicado com sucesso"
            });
        }
    }
} 