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

        public TestContentCreatorController(ILogger<TestContentCreatorController> logger)
        {
            _logger = logger;
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

        [HttpGet("current")]
        public async Task<ActionResult<ApiResponse<ContentCreatorDto>>> GetCurrentCreator()
        {
            _logger.LogInformation("TestContentCreatorController.GetCurrentCreator: Obtendo criador atual");
            
            // Simular criador atual
            var creator = new ContentCreatorDto
            {
                Id = Guid.NewGuid(),
                Name = "Creator Teste",
                Email = "teste@microsaas.com",
                Bio = "Criador de conteúdo para testes",
                ProfileImageUrl = "https://example.com/avatar.jpg",
                WebsiteUrl = "https://example.com/creator",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-1),
                TotalFollowers = 5000,
                TotalPosts = 120,
                Platforms = new List<SocialMediaPlatform> { SocialMediaPlatform.Instagram, SocialMediaPlatform.Twitter },
                SocialMediaAccounts = new List<SocialMediaAccountDto>
                {
                    new SocialMediaAccountDto
                    {
                        Id = Guid.NewGuid(),
                        Platform = SocialMediaPlatform.Instagram,
                        Username = "testecreator",
                        Followers = 1000,
                        IsVerified = true
                    }
                }
            };

            return Ok(new ApiResponse<ContentCreatorDto>
            {
                Success = true,
                Data = creator
            });
        }
    }
} 