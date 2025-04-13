using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Application.Services;
using MicroSaaS.Application.DTOs.ContentPost;

namespace MicroSaaS.IntegrationTests.Utils
{
    [ApiController]
    [Route("api/ContentPost")]
    [Route("api/v{version:apiVersion}/ContentPost")]
    public class TestContentPostController : ControllerBase
    {
        private readonly ILogger<TestContentPostController> _logger;
        private static readonly List<ContentPost> _posts = new List<ContentPost>();
        private static readonly List<ContentCreator> _creators = new List<ContentCreator>();
        private static readonly object _lock = new object();
        
        // IDs fixos para testes
        private static readonly Guid CREATOR_ID_1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid CREATOR_ID_2 = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
        private static readonly Guid POST_ID_1 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid POST_ID_2 = Guid.Parse("33333333-3333-3333-3333-333333333333");

        public TestContentPostController(ILogger<TestContentPostController> logger)
        {
            _logger = logger;
            
            // Inicializar dados de exemplo se as listas estiverem vazias
            lock (_lock)
            {
                if (!_posts.Any() || !_creators.Any())
                {
                    InitializeTestData();
                }
            }
        }

        [HttpGet("scheduled/{creatorId}")]
        public async Task<ActionResult<List<ContentPostDto>>> GetScheduledPosts(Guid creatorId)
        {
            _logger.LogInformation("TestContentPostController.GetScheduledPosts: Buscando posts agendados para criador {CreatorId}", creatorId);
            
            // Verificação do token de autenticação
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.Contains("invalid-token"))
            {
                return StatusCode(403);
            }

            if (creatorId == Guid.Empty)
            {
                return BadRequest(new { message = "ID do criador inválido" });
            }
            
            var scheduledPosts = _posts
                .Where(p => p.CreatorId == creatorId && p.Status == PostStatus.Scheduled)
                .Select(p => MapToDto(p))
                .ToList();
                
            return Ok(scheduledPosts);
        }

        [HttpGet]
        public async Task<ActionResult<List<ContentPostDto>>> GetAll()
        {
            var posts = _posts.ToList();
            return posts.Select(MapToDto).ToList();
        }

        [HttpGet("get-by-creator/{creatorId}")]
        public async Task<ActionResult<List<ContentPostDto>>> GetByCreator(string creatorId)
        {
            if (!Guid.TryParse(creatorId, out Guid creatorGuid))
            {
                return BadRequest(new { message = "ID do criador inválido" });
            }
            
            var posts = _posts.Where(p => p.CreatorId == creatorGuid).ToList();
            return posts.Select(MapToDto).ToList();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContentPostRequest request)
        {
            _logger.LogInformation("TestContentPostController.Create: Criando novo post");

            // Verificação do token de autenticação
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.Contains("invalid-token"))
            {
                return StatusCode(403);
            }

            if (!Guid.TryParse(request.CreatorId, out Guid creatorId))
            {
                return BadRequest(new { message = "ID do criador inválido" });
            }

            var newPost = new ContentPost
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                CreatorId = creatorId,
                Status = request.Status,
                Platform = request.Platform,
                MediaUrl = request.MediaUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            lock (_lock)
            {
                _posts.Add(newPost);
            }
            
            return Created($"/api/ContentPost/{newPost.Id}", MapToDto(newPost));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContentPostDto>> GetById(string id)
        {
            _logger.LogInformation("TestContentPostController.GetById: Buscando post {PostId}", id);
            
            // Verificação do token de autenticação
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.Contains("invalid-token"))
            {
                return StatusCode(403);
            }

            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid postId))
            {
                return BadRequest(new { message = "ID do post inválido" });
            }
            
            var post = _posts.FirstOrDefault(p => p.Id == postId);
            if (post == null)
            {
                return NotFound();
            }
            
            var dto = MapToDto(post);
            return Ok(dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePostRequest request)
        {
            _logger.LogInformation("TestContentPostController.Update: Atualizando post {PostId}", id);
            
            // Verificação do token de autenticação
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.Contains("invalid-token"))
            {
                return StatusCode(403);
            }

            if (id == Guid.Empty)
            {
                return BadRequest(new { message = "ID do post inválido" });
            }
            
            var post = _posts.FirstOrDefault(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            
            // Atualizar propriedades do post
            post.Title = request.Title ?? post.Title;
            post.Content = request.Content ?? post.Content;
            post.Status = request.Status;
            post.Platform = request.Platform;
            post.MediaUrl = request.MediaUrl ?? post.MediaUrl;
            post.UpdatedAt = DateTime.UtcNow;
            
            return NoContent();
        }
        
        private void InitializeTestData()
        {
            // Criar alguns criadores de conteúdo de exemplo
            var creator1 = new ContentCreator
            {
                Id = CREATOR_ID_1,
                Name = "Criador de Teste 1",
                Email = "criador1@exemplo.com",
                Username = "testcreator1",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            };
            
            var creator2 = new ContentCreator
            {
                Id = CREATOR_ID_2,
                Name = "Criador de Teste 2",
                Email = "criador2@exemplo.com",
                Username = "testcreator2",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            };
            
            _creators.Add(creator1);
            _creators.Add(creator2);
            
            // Criar alguns posts de exemplo
            var post1 = new ContentPost
            {
                Id = POST_ID_1,
                CreatorId = CREATOR_ID_1,
                Title = "Post de Teste 1",
                Content = "Este é um post de teste para integração",
                Platform = SocialMediaPlatform.Instagram,
                Status = PostStatus.Published,
                MediaUrl = "https://exemplo.com/imagem1.jpg",
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-10)
            };
            
            var post2 = new ContentPost
            {
                Id = POST_ID_2,
                CreatorId = CREATOR_ID_1,
                Title = "Post de Teste 2",
                Content = "Este é outro post de teste para integração",
                Platform = SocialMediaPlatform.Twitter,
                Status = PostStatus.Scheduled,
                MediaUrl = "https://exemplo.com/imagem2.jpg",
                ScheduledTime = DateTime.UtcNow.AddDays(1),
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            };
            
            var post3 = new ContentPost
            {
                Id = Guid.NewGuid(),
                CreatorId = CREATOR_ID_2,
                Title = "Post de Teste 3",
                Content = "Este é um post do segundo criador de teste",
                Platform = SocialMediaPlatform.Facebook,
                Status = PostStatus.Draft,
                MediaUrl = "https://exemplo.com/imagem3.jpg",
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            };
            
            _posts.Add(post1);
            _posts.Add(post2);
            _posts.Add(post3);
        }
        
        private ContentPostDto MapToDto(ContentPost post)
        {
            return new ContentPostDto
            {
                Id = post.Id,
                CreatorId = post.CreatorId,
                Title = post.Title,
                Content = post.Content,
                Platform = post.Platform,
                Status = post.Status,
                MediaUrl = post.MediaUrl,
                ScheduledTime = post.ScheduledTime?.TimeOfDay,
                ScheduledFor = post.ScheduledTime,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };
        }
    }

    public class CreateContentPostRequest
    {
        public string CreatorId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public SocialMediaPlatform Platform { get; set; }
        public PostStatus Status { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
    }

    public class UpdatePostRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public PostStatus Status { get; set; }
        public SocialMediaPlatform Platform { get; set; }
        public string MediaUrl { get; set; } = string.Empty;
    }
} 