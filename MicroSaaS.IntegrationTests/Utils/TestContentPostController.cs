using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
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
                .Where(p => p.Creator?.Id == creatorId && p.Status == PostStatus.Scheduled)
                .Select(p => MapToDto(p))
                .ToList();
                
            return Ok(scheduledPosts);
        }

        [HttpPost]
        public async Task<ActionResult<ContentPostDto>> Create([FromBody] CreatePostRequest request)
        {
            _logger.LogInformation("TestContentPostController.Create: Criando novo post para criador {CreatorId}", request.CreatorId);
            
            // Verificação do token de autenticação
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.Contains("invalid-token"))
            {
                return StatusCode(403);
            }

            if (request.CreatorId == Guid.Empty)
            {
                return BadRequest(new { message = "ID do criador inválido" });
            }
            
            ContentCreator creator;
            
            // Use lock para evitar problemas de concorrência
            lock (_lock)
            {
                // Verificar se o criador existe, ou criar um novo para testes
                creator = _creators.FirstOrDefault(c => c.Id == request.CreatorId);
                if (creator == null)
                {
                    // Para testes, criamos um criador fictício quando o ID não for encontrado
                    creator = new ContentCreator
                    {
                        Id = request.CreatorId,
                        Name = "Criador Autocriado para Teste",
                        Email = $"teste_{request.CreatorId.ToString().Substring(0, 8)}@exemplo.com",
                        Username = $"teste_{request.CreatorId.ToString().Substring(0, 8)}",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };
                    _creators.Add(creator);
                }
            }
            
            // Criar novo post
            var newPost = new ContentPost
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                CreatorId = request.CreatorId,
                Creator = creator,
                Status = PostStatus.Draft,
                Platform = request.Platform,
                MediaUrl = request.MediaUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            // Use lock novamente para adicionar à coleção
            lock (_lock)
            {
                _posts.Add(newPost);
            }
            
            var dto = MapToDto(newPost);
            return CreatedAtAction(nameof(GetById), new { id = newPost.Id, version = "1" }, dto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContentPostDto>> GetById(Guid id)
        {
            _logger.LogInformation("TestContentPostController.GetById: Buscando post {PostId}", id);
            
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
                Id = Guid.NewGuid(),
                Name = "Criador de Teste 1",
                Email = "criador1@exemplo.com",
                Username = "testcreator1",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            };
            
            var creator2 = new ContentCreator
            {
                Id = Guid.NewGuid(),
                Name = "Criador de Teste 2",
                Email = "criador2@exemplo.com",
                Username = "testcreator2",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            };
            
            _creators.Add(creator1);
            _creators.Add(creator2);
            
            // Criar alguns posts de exemplo
            _posts.Add(new ContentPost
            {
                Id = Guid.NewGuid(),
                Title = "Post Agendado 1",
                Content = "Conteúdo do post agendado 1",
                CreatorId = creator1.Id,
                Creator = creator1,
                Status = PostStatus.Scheduled,
                Platform = SocialMediaPlatform.Instagram,
                MediaUrl = "https://exemplo.com/imagem1.jpg",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            });
            
            _posts.Add(new ContentPost
            {
                Id = Guid.NewGuid(),
                Title = "Post Publicado 1",
                Content = "Conteúdo do post publicado 1",
                CreatorId = creator1.Id,
                Creator = creator1,
                Status = PostStatus.Published,
                Platform = SocialMediaPlatform.YouTube,
                MediaUrl = "https://exemplo.com/video1.mp4",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-4)
            });
            
            _posts.Add(new ContentPost
            {
                Id = Guid.NewGuid(),
                Title = "Post Agendado 2",
                Content = "Conteúdo do post agendado 2",
                CreatorId = creator2.Id,
                Creator = creator2,
                Status = PostStatus.Scheduled,
                Platform = SocialMediaPlatform.TikTok,
                MediaUrl = "https://exemplo.com/video2.mp4",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow
            });
        }
        
        private ContentPostDto MapToDto(ContentPost post)
        {
            return new ContentPostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatorId = post.CreatorId,
                Status = post.Status,
                Platform = post.Platform,
                MediaUrl = post.MediaUrl,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };
        }
    }
} 