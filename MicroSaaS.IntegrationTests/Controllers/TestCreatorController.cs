using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Shared.DTOs;
using MicroSaaS.Shared.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace MicroSaaS.IntegrationTests.Controllers
{
    [ApiController]
    [Route("api/v1/creators")]
    public class TestCreatorController : ControllerBase
    {
        private readonly ILogger<TestCreatorController> _logger;

        public TestCreatorController(ILogger<TestCreatorController> logger)
        {
            _logger = logger;
        }

        [HttpGet("me")]
        public ActionResult<ApiResponse<ContentCreatorDto>> GetCurrentCreator()
        {
            _logger.LogInformation("TestCreatorController.GetCurrentCreator: Retornando informações do criador atual");
            
            var creator = new ContentCreatorDto
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), // ID fixo para facilitar testes
                Name = "Criador de Teste",
                Email = "teste@microsaas.com",
                Username = "testecriador",
                Bio = "Criador para testes de integração",
                ProfileImageUrl = "https://exemplo.com/imagem.jpg",
                TotalFollowers = 1000,
                CreatedAt = DateTime.UtcNow.AddMonths(-6),
                UpdatedAt = DateTime.UtcNow,
                SocialMediaAccounts = new List<SocialMediaAccountDto>
                {
                    new SocialMediaAccountDto
                    {
                        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        Platform = MicroSaaS.Shared.Enums.SocialMediaPlatform.Instagram,
                        Username = "testecriador_ig",
                        Followers = 500,
                        IsActive = true
                    },
                    new SocialMediaAccountDto
                    {
                        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        Platform = MicroSaaS.Shared.Enums.SocialMediaPlatform.Twitter,
                        Username = "testecriador_tw",
                        Followers = 300,
                        IsActive = true
                    },
                    new SocialMediaAccountDto
                    {
                        Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                        Platform = MicroSaaS.Shared.Enums.SocialMediaPlatform.Facebook,
                        Username = "testecriador_fb",
                        Followers = 200,
                        IsActive = true
                    }
                }
            };
            
            return Ok(new ApiResponse<ContentCreatorDto>
            {
                Success = true,
                Data = creator,
                Message = "Informações do criador recuperadas com sucesso"
            });
        }
    }
} 