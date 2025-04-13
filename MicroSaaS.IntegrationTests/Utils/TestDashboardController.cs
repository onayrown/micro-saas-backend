using Microsoft.AspNetCore.Mvc;
using MicroSaaS.Domain.Entities;
using MicroSaaS.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using MicroSaaS.IntegrationTests.Models;

namespace MicroSaaS.IntegrationTests.Utils
{
    [ApiController]
    [Route("api/Dashboard")]
    public class TestDashboardController : ControllerBase
    {
        private readonly ILogger<TestDashboardController> _logger;
        private static readonly List<DashboardInsights> _insights = new List<DashboardInsights>();
        private static readonly List<PerformanceMetrics> _metrics = new List<PerformanceMetrics>();
        private static readonly List<ContentPost> _contentPosts = new List<ContentPost>();
        private static readonly List<ContentPerformance> _contentPerformances = new List<ContentPerformance>();

        public TestDashboardController(ILogger<TestDashboardController> logger)
        {
            _logger = logger;
        }

        [HttpGet("insights/{creatorId}")]
        public async Task<ActionResult<DashboardInsights>> GetLatestInsights(string creatorId)
        {
            _logger.LogInformation("TestDashboardController.GetLatestInsights: Buscando insights para criador {CreatorId}", creatorId);
            
            // Verificar se o ID do criador é válido
            if (string.IsNullOrEmpty(creatorId) || !Guid.TryParse(creatorId, out Guid creatorGuid) || creatorGuid == Guid.Empty)
            {
                return BadRequest("ID do criador inválido");
            }

            // Verificar se o criador existe (normalmente isso seria feito pelo serviço)
            var creatorExists = _insights.Any(i => i.CreatorId.ToString() == creatorId);
            
            var insight = _insights.FirstOrDefault(i => i.CreatorId.ToString() == creatorId);
            
            if (insight == null)
            {
                // Criar insights de exemplo
                insight = new DashboardInsights
                {
                    Id = Guid.NewGuid(),
                    CreatorId = Guid.Parse(creatorId),
                    PlatformsPerformance = new List<PlatformInsight>
                    {
                        new PlatformInsight
                        {
                            Platform = SocialMediaPlatform.Instagram,
                            FollowerCount = 12500,
                            FollowerGrowth = 340,
                            EngagementRate = 4.7m,
                            ReachGrowth = 8.2m,
                            TopPerformingContent = "Foto de lifestyle mostrando setup de gravação"
                        },
                        new PlatformInsight
                        {
                            Platform = SocialMediaPlatform.YouTube,
                            FollowerCount = 25800,
                            FollowerGrowth = 520,
                            EngagementRate = 5.3m,
                            ReachGrowth = 12.1m,
                            TopPerformingContent = "Tutorial de edição de vídeo em 10 minutos"
                        }
                    },
                    TotalRevenue = 2850.00m,
                    RevenueGrowth = 15.5m,
                    PostingConsistency = 85,
                    AudionceDemographics = new AudienceDemographics
                    {
                        AgeGroups = new Dictionary<string, decimal>
                        {
                            { "18-24", 32.5m },
                            { "25-34", 45.8m },
                            { "35-44", 15.2m },
                            { "45+", 6.5m }
                        },
                        TopCountries = new Dictionary<string, decimal>
                        {
                            { "Brasil", 72.8m },
                            { "Portugal", 12.5m },
                            { "Estados Unidos", 6.3m },
                            { "Outros", 8.4m }
                        },
                        Gender = new Dictionary<string, decimal>
                        {
                            { "Feminino", 68.2m },
                            { "Masculino", 31.8m }
                        }
                    },
                    ContentTypePerformance = new Dictionary<string, decimal>
                    {
                        { "Tutorial", 9.3m },
                        { "Review", 7.8m },
                        { "Vlog", 6.5m },
                        { "Lifestyle", 8.2m }
                    },
                    KeyInsights = new List<string>
                    {
                        "Seus tutoriais têm 25% mais engajamento que outros formatos",
                        "Publicações às quintas-feiras têm melhor desempenho",
                        "Conteúdos sobre produtividade geram mais conversões"
                    },
                    ActionRecommendations = new List<string>
                    {
                        "Aumente a frequência de tutoriais para 2x por semana",
                        "Experimente lives aos domingos às 20h",
                        "Crie uma série sobre ferramentas de produtividade"
                    },
                    GrowthOpportunities = new List<GrowthOpportunity>
                    {
                        new GrowthOpportunity
                        {
                            Title = "Parcerias com marcas de tecnologia",
                            Description = "Existe potencial para parcerias com 3 marcas baseado no seu perfil de audiência",
                            PotentialRevenueIncrease = 1500.00m
                        },
                        new GrowthOpportunity
                        {
                            Title = "Criação de curso online",
                            Description = "Um mini-curso sobre edição tem potencial baseado nas suas métricas de engajamento",
                            PotentialRevenueIncrease = 3200.00m
                        }
                    },
                    CompetitorBenchmark = new List<CompetitorComparison>
                    {
                        new CompetitorComparison
                        {
                            Metric = "Engajamento",
                            YourValue = 4.8m,
                            AverageInNiche = 3.2m,
                            Difference = 1.6m,
                            IsPositive = true
                        },
                        new CompetitorComparison
                        {
                            Metric = "Frequência de postagem",
                            YourValue = 3.5m,
                            AverageInNiche = 4.7m,
                            Difference = -1.2m,
                            IsPositive = false
                        }
                    },
                    AnalysisPeriod = new DateRange
                    {
                        StartDate = DateTime.UtcNow.AddDays(-30),
                        EndDate = DateTime.UtcNow
                    },
                    GeneratedAt = DateTime.UtcNow
                };
                
                _insights.Add(insight);
            }
            
            return Ok(insight);
        }

        [HttpGet("insights/{creatorId}/generate")]
        public async Task<ActionResult<DashboardInsights>> GenerateInsights(
            string creatorId, 
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            _logger.LogInformation("TestDashboardController.GenerateInsights: Gerando insights para criador {CreatorId}", creatorId);
            
            // Verificar se o ID do criador é válido
            if (string.IsNullOrEmpty(creatorId) || !Guid.TryParse(creatorId, out Guid creatorGuid) || creatorGuid == Guid.Empty)
            {
                return BadRequest("ID do criador inválido");
            }

            // Verificar se o criador existe (normalmente isso seria feito pelo serviço)
            var creatorExists = _insights.Any(i => i.CreatorId.ToString() == creatorId);
            
            // Remover insights existentes
            var existingInsight = _insights.FirstOrDefault(i => i.CreatorId.ToString() == creatorId);
            if (existingInsight != null)
            {
                _insights.Remove(existingInsight);
            }
            
            // Criar insights de exemplo
            var insight = new DashboardInsights
            {
                Id = Guid.NewGuid(),
                CreatorId = Guid.Parse(creatorId),
                PlatformsPerformance = new List<PlatformInsight>
                {
                    new PlatformInsight
                    {
                        Platform = SocialMediaPlatform.Instagram,
                        FollowerCount = 12500,
                        FollowerGrowth = 340,
                        EngagementRate = 4.7m,
                        ReachGrowth = 8.2m,
                        TopPerformingContent = "Foto de lifestyle mostrando setup de gravação"
                    },
                    new PlatformInsight
                    {
                        Platform = SocialMediaPlatform.TikTok,
                        FollowerCount = 8200,
                        FollowerGrowth = 620,
                        EngagementRate = 6.1m,
                        ReachGrowth = 15.3m,
                        TopPerformingContent = "Dicas rápidas de produtividade em 60 segundos"
                    }
                },
                TotalRevenue = 3200.00m,
                RevenueGrowth = 17.8m,
                PostingConsistency = 90,
                AudionceDemographics = new AudienceDemographics
                {
                    AgeGroups = new Dictionary<string, decimal>
                    {
                        { "18-24", 35.2m },
                        { "25-34", 42.5m },
                        { "35-44", 16.8m },
                        { "45+", 5.5m }
                    },
                    TopCountries = new Dictionary<string, decimal>
                    {
                        { "Brasil", 75.3m },
                        { "Portugal", 10.2m },
                        { "Estados Unidos", 7.1m },
                        { "Outros", 7.4m }
                    },
                    Gender = new Dictionary<string, decimal>
                    {
                        { "Feminino", 65.7m },
                        { "Masculino", 34.3m }
                    }
                },
                ContentTypePerformance = new Dictionary<string, decimal>
                {
                    { "Tutorial", 9.5m },
                    { "Review", 8.1m },
                    { "Vlog", 6.7m },
                    { "Lifestyle", 8.5m }
                },
                KeyInsights = new List<string>
                {
                    "Conteúdo no TikTok teve crescimento 35% maior que outras plataformas",
                    "Vídeos curtos de 60 segundos têm melhor taxa de finalização",
                    "Público de 25-34 anos tem maior probabilidade de conversão para compras"
                },
                ActionRecommendations = new List<string>
                {
                    "Foque em vídeos curtos para crescimento no TikTok",
                    "Desenvolva uma oferta específica para público de 25-34 anos",
                    "Aumente frequência de colaborações com outras contas"
                },
                GrowthOpportunities = new List<GrowthOpportunity>
                {
                    new GrowthOpportunity
                    {
                        Title = "Criação de e-book sobre produtividade",
                        Description = "Sua audiência demonstra interesse em conteúdo educacional sobre este tema",
                        PotentialRevenueIncrease = 2200.00m
                    },
                    new GrowthOpportunity
                    {
                        Title = "Membros premium com conteúdo exclusivo",
                        Description = "12% dos seguidores engajam profundamente e têm potencial para assinatura",
                        PotentialRevenueIncrease = 4800.00m
                    }
                },
                CompetitorBenchmark = new List<CompetitorComparison>
                {
                    new CompetitorComparison
                    {
                        Metric = "Crescimento de seguidores",
                        YourValue = 7.5m,
                        AverageInNiche = 4.3m,
                        Difference = 3.2m,
                        IsPositive = true
                    },
                    new CompetitorComparison
                    {
                        Metric = "Conversão de vendas",
                        YourValue = 2.1m,
                        AverageInNiche = 1.8m,
                        Difference = 0.3m,
                        IsPositive = true
                    }
                },
                AnalysisPeriod = new DateRange
                {
                    StartDate = startDate ?? DateTime.UtcNow.AddDays(-30),
                    EndDate = endDate ?? DateTime.UtcNow
                },
                GeneratedAt = DateTime.UtcNow
            };
            
            _insights.Add(insight);
            
            return Ok(insight);
        }

        [HttpGet("metrics/{creatorId}")]
        public async Task<ActionResult<IEnumerable<PerformanceMetrics>>> GetMetrics(
            string creatorId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] SocialMediaPlatform? platform = null)
        {
            _logger.LogInformation("TestDashboardController.GetMetrics: Buscando métricas para criador {CreatorId}", creatorId);
            
            if (string.IsNullOrEmpty(creatorId) || !Guid.TryParse(creatorId, out Guid creatorGuid))
            {
                return BadRequest("ID do criador inválido");
            }

            if (!_metrics.Any(m => m.CreatorId == creatorId))
            {
                // Criar métricas de exemplo
                for (int i = 0; i < 30; i++)
                {
                    var date = DateTime.UtcNow.AddDays(-i);
                    
                    _metrics.Add(new PerformanceMetrics
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreatorId = creatorId,
                        Date = date,
                        Platform = i % 3 == 0 ? SocialMediaPlatform.Instagram : (i % 3 == 1 ? SocialMediaPlatform.YouTube : SocialMediaPlatform.TikTok),
                        FollowerCount = 10000 + (i * 100),
                        NewFollowers = 100 - (i % 30),
                        Views = 5000 - (i * 50),
                        Likes = 1000 - (i * 15),
                        Comments = 200 - (i * 3),
                        Shares = 150 - (i * 2),
                        Saves = 80 - i,
                        EngagementRate = 4.5m - (i * 0.05m),
                        Reach = 8000 - (i * 80),
                        Impressions = 12000 - (i * 100),
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
            
            // Filtrar por período e plataforma
            var filteredMetrics = _metrics
                .Where(m => m.CreatorId == creatorId)
                .Where(m => !startDate.HasValue || m.Date >= startDate.Value)
                .Where(m => !endDate.HasValue || m.Date <= endDate.Value)
                .Where(m => !platform.HasValue || m.Platform == platform.Value)
                .OrderByDescending(m => m.Date)
                .ToList();
                
            return Ok(filteredMetrics);
        }

        [HttpGet("metrics/{creatorId}/daily")]
        public async Task<ActionResult<PerformanceMetrics>> GetDailyMetrics(
            string creatorId,
            [FromQuery] DateTime? date = null,
            [FromQuery] SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
        {
            _logger.LogInformation("TestDashboardController.GetDailyMetrics: Buscando métricas diárias para criador {CreatorId}", creatorId);
            
            var targetDate = date ?? DateTime.UtcNow.Date;
            
            var metric = _metrics.FirstOrDefault(m => 
                m.CreatorId == creatorId && 
                m.Date.Date == targetDate.Date &&
                m.Platform == platform);
                
            if (metric == null)
            {
                // Criar métricas de exemplo
                metric = new PerformanceMetrics
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatorId = creatorId,
                    Date = targetDate,
                    Platform = platform,
                    FollowerCount = 10000,
                    NewFollowers = 85,
                    Views = 4800,
                    Likes = 950,
                    Comments = 180,
                    Shares = 145,
                    Saves = 75,
                    EngagementRate = 4.3m,
                    Reach = 7850,
                    Impressions = 11800,
                    CreatedAt = DateTime.UtcNow
                };
                
                _metrics.Add(metric);
            }

            return Ok(metric);
        }

        [HttpGet("content/{creatorId}/top")]
        public async Task<ActionResult<List<ContentPost>>> GetTopContent(
            string creatorId,
            [FromQuery] int limit = 5)
        {
            _logger.LogInformation("TestDashboardController.GetTopContent: Buscando top conteúdos para criador {CreatorId}", creatorId);
            
            var guidCreatorId = Guid.Parse(creatorId);
            if (!_contentPosts.Any(p => p.CreatorId == guidCreatorId))
            {
                // Criar posts de exemplo
                for (int i = 0; i < 10; i++)
                {
                    _contentPosts.Add(new ContentPost
                    {
                        Id = Guid.NewGuid(),
                        CreatorId = guidCreatorId,
                        Title = $"Post {i + 1} - Conteúdo de exemplo",
                        Content = $"Este é o conteúdo do post de exemplo {i + 1}",
                        MediaUrl = $"https://example.com/media/{i + 1}",
                        Platform = i % 3 == 0 ? SocialMediaPlatform.Instagram : (i % 3 == 1 ? SocialMediaPlatform.YouTube : SocialMediaPlatform.TikTok),
                        Status = PostStatus.Published,
                        PublishedAt = DateTime.UtcNow.AddDays(-i),
                        Views = 1000 - (i * 90),
                        Likes = 500 - (i * 45),
                        Comments = 100 - (i * 9),
                        Shares = 50 - (i * 4),
                        EngagementRate = 5.0m - (i * 0.3m),
                        CreatedAt = DateTime.UtcNow.AddDays(-i - 1),
                        UpdatedAt = DateTime.UtcNow.AddDays(-i)
                    });
                }
            }
            
            var topPosts = _contentPosts
                .Where(p => p.CreatorId == guidCreatorId)
                .OrderByDescending(p => p.EngagementRate)
                .Take(limit)
                .ToList();
                
            return Ok(topPosts);
        }

        [HttpGet("recommendations/{creatorId}/posting-times")]
        public async Task<ActionResult<List<PostTimeRecommendation>>> GetBestTimeToPost(
            string creatorId,
            [FromQuery] SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
        {
            _logger.LogInformation("TestDashboardController.GetBestTimeToPost: Buscando melhores horários para criador {CreatorId}", creatorId);
            
            if (string.IsNullOrEmpty(creatorId) || !Guid.TryParse(creatorId, out _))
            {
                return BadRequest("ID do criador inválido");
            }

            var recommendations = new List<PostTimeRecommendation>
            {
                new PostTimeRecommendation
                {
                    DayOfWeek = DayOfWeek.Tuesday,
                    TimeOfDay = new TimeSpan(18, 0, 0),
                    EngagementScore = 9.5
                },
                new PostTimeRecommendation
                {
                    DayOfWeek = DayOfWeek.Sunday,
                    TimeOfDay = new TimeSpan(12, 0, 0),
                    EngagementScore = 8.7
                },
                new PostTimeRecommendation
                {
                    DayOfWeek = DayOfWeek.Thursday,
                    TimeOfDay = new TimeSpan(20, 0, 0),
                    EngagementScore = 7.9
                }
            };
            
            return Ok(recommendations.OrderByDescending(r => r.EngagementScore).ToList());
        }

        [HttpGet("analytics/{creatorId}/engagement")]
        public async Task<ActionResult<decimal>> GetAverageEngagementRate(
            string creatorId,
            [FromQuery] SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
        {
            _logger.LogInformation("TestDashboardController.GetAverageEngagementRate: Calculando taxa de engajamento para criador {CreatorId}", creatorId);
            
            // Retornar taxa de engajamento de exemplo
            decimal engagementRate = 4.8m;
            return Ok(engagementRate);
        }

        [HttpGet("analytics/{creatorId}/revenue-growth")]
        public async Task<ActionResult<decimal>> GetRevenueGrowth(
            string creatorId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            _logger.LogInformation("TestDashboardController.GetRevenueGrowth: Calculando crescimento de receita para criador {CreatorId}", creatorId);
            
            // Retornar crescimento de receita de exemplo
            decimal revenueGrowth = 15.2m;
            return Ok(revenueGrowth);
        }

        [HttpGet("analytics/{creatorId}/follower-growth")]
        public async Task<ActionResult<int>> GetFollowerGrowth(
            string creatorId,
            [FromQuery] SocialMediaPlatform platform = SocialMediaPlatform.Instagram,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            _logger.LogInformation("TestDashboardController.GetFollowerGrowth: Calculando crescimento de seguidores para criador {CreatorId}", creatorId);
            
            // Retornar crescimento de seguidores de exemplo
            int followerGrowth = 180;
            return Ok(followerGrowth);
        }

        [HttpPost("metrics")]
        public async Task<ActionResult<PerformanceMetrics>> AddMetrics([FromBody] PerformanceMetrics metrics)
        {
            _logger.LogInformation("TestDashboardController.AddMetrics: Adicionando métricas para criador {CreatorId}", metrics.CreatorId);
            
            if (string.IsNullOrEmpty(metrics.Id) || metrics.Id == Guid.Empty.ToString())
            {
                metrics.Id = Guid.NewGuid().ToString();
            }
            
            _metrics.Add(metrics);
            
            return Created($"/api/Dashboard/metrics/{metrics.CreatorId}/daily", metrics);
        }

        [HttpPost("content-performance")]
        public async Task<ActionResult<ContentPerformance>> AddContentPerformance([FromBody] ContentPerformance performance)
        {
            _logger.LogInformation("TestDashboardController.AddContentPerformance: Adicionando performance para conteúdo {PostId}", performance.PostId);
            
            if (performance.Id == Guid.Empty)
            {
                performance.Id = Guid.NewGuid();
            }
            
            _contentPerformances.Add(performance);
            
            return Created($"/api/Dashboard/content/{performance.CreatorId.ToString()}/top", performance);
        }
    }
} 