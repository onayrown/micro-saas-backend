using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Models;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace MicroSaaS.IntegrationTests.Utils
{
    [ApiController]
    [Route("api/v1/creators")]
    public class TestContentCreatorController : ControllerBase
    {
        private readonly ILogger<TestContentCreatorController> _logger;
        private static readonly List<ContentCreatorDto> _creators = new List<ContentCreatorDto>();
        
        // IDs fixos para testes
        private static readonly Guid CREATOR_ID_1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid CREATOR_ID_2 = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
        private static readonly Guid SOCIAL_ACCOUNT_ID_1 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid SOCIAL_ACCOUNT_ID_2 = Guid.Parse("33333333-3333-3333-3333-333333333333");
        
        public TestContentCreatorController(ILogger<TestContentCreatorController> logger)
        {
            _logger = logger;
            
            // Inicializar criadores de teste se a lista estiver vazia
            if (!_creators.Any())
            {
                InitializeTestCreators();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContentCreatorDto>> GetById(Guid id)
        {
            _logger.LogInformation("TestContentCreatorController.GetById: Buscando criador {Id}", id);
            
            var creator = _creators.FirstOrDefault(c => c.Id == id);
            if (creator == null)
                return NotFound();

            return Ok(creator);
        }

        [HttpPost]
        public async Task<ActionResult<ContentCreatorDto>> Create([FromBody] ContentCreatorDto creatorDto)
        {
            _logger.LogInformation("TestContentCreatorController.Create: Criando criador {Name}", creatorDto.Name);
            
            creatorDto.Id = Guid.NewGuid();
            creatorDto.CreatedAt = DateTime.UtcNow;
            creatorDto.UpdatedAt = DateTime.UtcNow;
            
            if (creatorDto.SocialMediaAccounts == null)
                creatorDto.SocialMediaAccounts = new List<SocialMediaAccountDto>();
                
            _creators.Add(creatorDto);
            
            return CreatedAtAction(nameof(GetById), new { id = creatorDto.Id }, creatorDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] ContentCreatorDto creatorDto)
        {
            _logger.LogInformation("TestContentCreatorController.Update: Atualizando criador {Id}", id);
            
            var existingCreator = _creators.FirstOrDefault(c => c.Id == id);
            if (existingCreator == null)
                return NotFound();

            existingCreator.Name = creatorDto.Name;
            existingCreator.Email = creatorDto.Email;
            existingCreator.Username = creatorDto.Username;
            existingCreator.Bio = creatorDto.Bio;
            existingCreator.ProfileImageUrl = creatorDto.ProfileImageUrl;
            existingCreator.WebsiteUrl = creatorDto.WebsiteUrl;
            existingCreator.UpdatedAt = DateTime.UtcNow;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("TestContentCreatorController.Delete: Excluindo criador {Id}", id);
            
            var creator = _creators.FirstOrDefault(c => c.Id == id);
            if (creator == null)
                return NotFound();

            _creators.Remove(creator);
            return NoContent();
        }

        private void InitializeTestCreators()
        {
            // Criar criadores de teste com IDs fixos
            var creator1 = new ContentCreatorDto
            {
                Id = CREATOR_ID_1,
                Name = "Criador de Teste 1",
                Email = "criador1@exemplo.com",
                Username = "testcreator1",
                Bio = "Criador de conteúdo para testes de integração",
                ProfileImageUrl = "https://example.com/avatar1.jpg",
                WebsiteUrl = "https://example.com/creator1",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-5),
                TotalFollowers = 5000,
                TotalPosts = 120,
                Platforms = new List<SocialMediaPlatform> { SocialMediaPlatform.Instagram, SocialMediaPlatform.Twitter },
                SocialMediaAccounts = new List<SocialMediaAccountDto>
                {
                    new SocialMediaAccountDto
                    {
                        Id = SOCIAL_ACCOUNT_ID_1,
                        Platform = SocialMediaPlatform.Instagram,
                        Username = "testcreator1_instagram",
                        Followers = 1000,
                        IsVerified = true
                    }
                }
            };
            
            var creator2 = new ContentCreatorDto
            {
                Id = CREATOR_ID_2,
                Name = "Criador de Teste 2",
                Email = "criador2@exemplo.com",
                Username = "testcreator2",
                Bio = "Outro criador para testes",
                ProfileImageUrl = "https://example.com/avatar2.jpg",
                WebsiteUrl = "https://example.com/creator2",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-2),
                TotalFollowers = 2500,
                TotalPosts = 75,
                Platforms = new List<SocialMediaPlatform> { SocialMediaPlatform.YouTube },
                SocialMediaAccounts = new List<SocialMediaAccountDto>
                {
                    new SocialMediaAccountDto
                    {
                        Id = SOCIAL_ACCOUNT_ID_2,
                        Platform = SocialMediaPlatform.YouTube,
                        Username = "testcreator2_youtube",
                        Followers = 2500,
                        IsVerified = false
                    }
                }
            };
            
            _creators.Add(creator1);
            _creators.Add(creator2);
        }

        [HttpGet("current")]
        public async Task<ActionResult<ApiResponse<ContentCreatorDto>>> GetCurrentCreator()
        {
            _logger.LogInformation("TestContentCreatorController.GetCurrentCreator: Obtendo criador atual");
            
            // Retornar o primeiro criador como criador atual
            var creator = _creators.FirstOrDefault() ?? new ContentCreatorDto
            {
                Id = CREATOR_ID_1,
                Name = "Creator Padrão",
                Email = "default@microsaas.com",
                Bio = "Criador padrão para testes",
                ProfileImageUrl = "https://example.com/default.jpg",
                WebsiteUrl = "https://example.com/default",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            };

            return Ok(new ApiResponse<ContentCreatorDto>
            {
                Success = true,
                Data = creator
            });
        }
    }
} 