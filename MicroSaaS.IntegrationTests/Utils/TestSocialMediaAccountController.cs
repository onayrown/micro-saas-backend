using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace MicroSaaS.IntegrationTests.Utils
{
    [ApiController]
    [Route("api/SocialMediaAccount")]
    public class TestSocialMediaAccountController : ControllerBase
    {
        private readonly ILogger<TestSocialMediaAccountController> _logger;
        
        // Usamos listas estáticas com dados mock para manter o estado entre requisições
        private static readonly List<ContentCreator> _creators = new List<ContentCreator>();
        private static readonly List<SocialMediaAccount> _accounts = new List<SocialMediaAccount>();
        private static readonly object _lock = new object();

        // IDs fixos para testes de integração - definidos como constantes para reutilização
        private static readonly Guid CREATOR_ID_1 = Guid.Parse("11111111-1111-1111-1111-111111111111");
        private static readonly Guid CREATOR_ID_2 = Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA");
        private static readonly Guid ACCOUNT_ID_1 = Guid.Parse("22222222-2222-2222-2222-222222222222");
        private static readonly Guid ACCOUNT_ID_2 = Guid.Parse("33333333-3333-3333-3333-333333333333");
        
        // ID para testar cenários de não-existência
        private static readonly Guid NON_EXISTENT_CREATOR_ID = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");

        public TestSocialMediaAccountController(ILogger<TestSocialMediaAccountController> logger)
        {
            _logger = logger;
            
            // Inicializar dados de exemplo se as listas estiverem vazias
            lock (_lock)
            {
                if (!_creators.Any() || !_accounts.Any())
                {
                    InitializeTestData();
                }
            }
        }

        [HttpGet("{creatorId}")]
        public async Task<ActionResult<List<SocialMediaAccount>>> GetAccounts(Guid creatorId)
        {
            _logger.LogInformation("TestSocialMediaAccountController.GetAccounts: Buscando contas para criador {CreatorId}", creatorId);
            
            // Verificação do token de autenticação
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.Contains("invalid-token"))
            {
                return StatusCode(403);
            }

            if (creatorId == Guid.Empty)
            {
                return NotFound(new { message = "Criador não encontrado" });
            }
            
            ContentCreator creator;
            List<SocialMediaAccount> accounts;
            
            lock (_lock)
            {
                creator = _creators.FirstOrDefault(c => c.Id == creatorId);
                
                if (creator == null)
                {
                    return NotFound(new { message = "Criador não encontrado" });
                }
                
                accounts = _accounts.Where(a => a.CreatorId == creatorId).ToList();
            }
            
            return Ok(accounts);
        }

        [HttpPost("{creatorId}")]
        public async Task<ActionResult<SocialMediaAccount>> AddAccount(
            Guid creatorId, 
            [FromBody] AddSocialMediaRequest request)
        {
            _logger.LogInformation("TestSocialMediaAccountController.AddAccount: Adicionando conta {Platform} para criador {CreatorId}", request.Platform, creatorId);
            
            // Verificação do token de autenticação
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.Contains("invalid-token"))
            {
                return StatusCode(403);
            }

            if (creatorId == Guid.Empty)
            {
                return NotFound(new { message = "Criador não encontrado" });
            }
            
            ContentCreator creator;
            
            lock (_lock)
            {
                creator = _creators.FirstOrDefault(c => c.Id == creatorId);
            }
            
            if (creator == null)
            {
                return NotFound(new { message = "Criador não encontrado" });
            }
            
            // Simular URL de autorização
            var authUrl = $"https://api.{request.Platform.ToString().ToLower()}.com/oauth?client_id=test_client_id&redirect_uri=https://localhost/callback&state={creatorId}";
            
            return Ok(new { authorizationUrl = authUrl });
        }

        [HttpGet("callback/{creatorId}/{platform}")]
        [HttpGet("callback")]
        public async Task<IActionResult> HandleCallback([FromRoute] Guid? creatorId, [FromRoute] SocialMediaPlatform? platform, [FromQuery] string code, [FromQuery] Guid? creatorIdQuery = null, [FromQuery] SocialMediaPlatform? platformQuery = null)
        {
            var effectiveCreatorId = creatorId ?? creatorIdQuery ?? Guid.Empty;
            var effectivePlatform = platform ?? platformQuery ?? SocialMediaPlatform.Instagram;
            
            _logger.LogInformation("TestSocialMediaAccountController.HandleCallback: Processando callback para criador {CreatorId} na plataforma {Platform}", effectiveCreatorId, effectivePlatform);
            
            if (effectiveCreatorId == Guid.Empty)
            {
                return BadRequest(new { message = "ID do criador inválido" });
            }
            
            // Para testes específicos onde estamos testando criador não existente
            if (effectiveCreatorId == NON_EXISTENT_CREATOR_ID)
            {
                return NotFound(new { message = "Criador não encontrado" });
            }
            
            ContentCreator creator;
            
            lock (_lock)
            {
                creator = _creators.FirstOrDefault(c => c.Id == effectiveCreatorId);
            }
            
            if (creator == null)
            {
                return NotFound(new { message = "Criador não encontrado" });
            }
            
            // Criar nova conta
            var account = new SocialMediaAccount
            {
                Id = Guid.NewGuid(),
                CreatorId = effectiveCreatorId,
                Platform = effectivePlatform,
                Username = $"user_{Guid.NewGuid().ToString().Substring(0, 8)}",
                AccessToken = $"token_{Guid.NewGuid()}",
                RefreshToken = $"refresh_{Guid.NewGuid()}",
                TokenExpiresAt = DateTime.UtcNow.AddDays(60),
                IsActive = true,
                ProfileUrl = $"profile_{Guid.NewGuid().ToString().Substring(0, 8)}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            lock (_lock)
            {
                _accounts.Add(account);
            }
            
            // Redirecionar para a aplicação frontend
            return Redirect($"https://seuapp.com/connect/success?platform={effectivePlatform}");
        }

        [HttpDelete("{creatorId}/{accountId}")]
        public async Task<IActionResult> RemoveAccount(Guid creatorId, Guid accountId)
        {
            _logger.LogInformation("TestSocialMediaAccountController.RemoveAccount: Removendo conta {AccountId} do criador {CreatorId}", accountId, creatorId);
            
            // Verificação do token de autenticação
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.Contains("invalid-token"))
            {
                return StatusCode(403);
            }

            if (creatorId == Guid.Empty || accountId == Guid.Empty)
            {
                return NotFound(new { message = "Recurso não encontrado" });
            }
            
            ContentCreator creator;
            SocialMediaAccount account;
            
            lock (_lock)
            {
                creator = _creators.FirstOrDefault(c => c.Id == creatorId);
                if (creator == null)
                {
                    return NotFound(new { message = "Criador não encontrado" });
                }
                
                account = _accounts.FirstOrDefault(a => a.Id == accountId && a.CreatorId == creatorId);
                if (account == null)
                {
                    return NotFound(new { message = "Conta não encontrada" });
                }
                
                _accounts.Remove(account);
            }
            
            return NoContent();
        }
        
        private void InitializeTestData()
        {
            // Criar alguns criadores de conteúdo com IDs fixos para os testes
            var creator1 = new ContentCreator
            {
                Id = CREATOR_ID_1, // Usando constante para garantir consistência
                Name = "Criador de Teste 1",
                Email = "criador1@exemplo.com",
                Username = "testcreator1",
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            };
            
            var creator2 = new ContentCreator
            {
                Id = CREATOR_ID_2, // Usando constante para garantir consistência
                Name = "Criador de Teste 2",
                Email = "criador2@exemplo.com",
                Username = "testcreator2",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            };
            
            _creators.Add(creator1);
            _creators.Add(creator2);
            
            // Criar algumas contas de mídia social com ID fixo para o primeiro criador
            _accounts.Add(new SocialMediaAccount
            {
                Id = ACCOUNT_ID_1, // Usando constante para garantir consistência
                CreatorId = creator1.Id,
                Platform = SocialMediaPlatform.Instagram,
                Username = "testcreator1_instagram",
                AccessToken = "token_instagram_1",
                RefreshToken = "refresh_instagram_1",
                TokenExpiresAt = DateTime.UtcNow.AddDays(60),
                IsActive = true,
                ProfileUrl = "profile_instagram_1",
                CreatedAt = DateTime.UtcNow.AddDays(-25),
                UpdatedAt = DateTime.UtcNow.AddDays(-25)
            });
            
            _accounts.Add(new SocialMediaAccount
            {
                Id = ACCOUNT_ID_2, // Usando constante para garantir consistência
                CreatorId = creator1.Id,
                Platform = SocialMediaPlatform.YouTube,
                Username = "testcreator1_youtube",
                AccessToken = "token_youtube_1",
                RefreshToken = "refresh_youtube_1",
                TokenExpiresAt = DateTime.UtcNow.AddDays(60),
                IsActive = true,
                ProfileUrl = "profile_youtube_1",
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-20)
            });
            
            _accounts.Add(new SocialMediaAccount
            {
                Id = Guid.NewGuid(),
                CreatorId = creator2.Id,
                Platform = SocialMediaPlatform.TikTok,
                Username = "testcreator2_tiktok",
                AccessToken = "token_tiktok_1",
                RefreshToken = "refresh_tiktok_1",
                TokenExpiresAt = DateTime.UtcNow.AddDays(60),
                IsActive = true,
                ProfileUrl = "profile_tiktok_1",
                CreatedAt = DateTime.UtcNow.AddDays(-15),
                UpdatedAt = DateTime.UtcNow.AddDays(-15)
            });
        }
    }
    
    public class AddSocialMediaRequest
    {
        public SocialMediaPlatform Platform { get; set; }
    }
} 