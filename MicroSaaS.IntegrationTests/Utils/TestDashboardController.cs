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
        private static readonly Random _random = new Random();

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
                            PotentialImpact = "Alta",
                            RequiredEffort = "Média"
                        },
                        new GrowthOpportunity
                        {
                            Title = "Criação de curso online",
                            Description = "Um mini-curso sobre edição tem potencial baseado nas suas métricas de engajamento",
                            PotentialImpact = "Média",
                            RequiredEffort = "Alta"
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
                        PotentialImpact = "Alta",
                        RequiredEffort = "Média"
                    },
                    new GrowthOpportunity
                    {
                        Title = "Membros premium com conteúdo exclusivo",
                        Description = "12% dos seguidores engajam profundamente e têm potencial para assinatura",
                        PotentialImpact = "Média",
                        RequiredEffort = "Alta"
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
        public ActionResult<List<PerformanceMetrics>> GetDashboardMetricsAsync(Guid creatorId)
        {
            _logger.LogInformation($"Recebendo solicitação para métricas do creator {creatorId}");
            
            if (creatorId == Guid.Empty)
            {
                return BadRequest("CreatorId inválido");
            }
            
            // Obter parâmetros de consulta para filtrar
            var startDateParam = Request.Query["startDate"].FirstOrDefault();
            var endDateParam = Request.Query["endDate"].FirstOrDefault();
            var platformParam = Request.Query["platform"].FirstOrDefault();
            
            DateTime? startDate = null;
            DateTime? endDate = null;
            SocialMediaPlatform? platform = null;
            
            // Parsear data de início
            if (!string.IsNullOrEmpty(startDateParam) && DateTime.TryParse(startDateParam, out var parsedStartDate))
            {
                startDate = parsedStartDate;
            }
            
            // Parsear data de fim
            if (!string.IsNullOrEmpty(endDateParam) && DateTime.TryParse(endDateParam, out var parsedEndDate))
            {
                endDate = parsedEndDate;
            }
            
            // Parsear plataforma
            if (!string.IsNullOrEmpty(platformParam) && Enum.TryParse<SocialMediaPlatform>(platformParam, true, out var parsedPlatform))
            {
                platform = parsedPlatform;
            }
            
            var metrics = new List<PerformanceMetrics>();
            
            // Gerar métricas para os últimos 30 dias
            var random = new Random();
            for (int i = 0; i < 30; i++)
            {
                var date = DateTime.UtcNow.AddDays(-i);
                
                // Pular se estiver fora do intervalo de datas
                if (startDate.HasValue && date < startDate.Value)
                    continue;
                
                if (endDate.HasValue && date > endDate.Value)
                    continue;
                
                // Alternar entre plataformas para ter dados variados
                var metricPlatform = i % 2 == 0 ? SocialMediaPlatform.Instagram : SocialMediaPlatform.YouTube;
                
                // Pular se não corresponder à plataforma selecionada
                if (platform.HasValue && metricPlatform != platform.Value)
                    continue;
                
                metrics.Add(new PerformanceMetrics
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Platform = metricPlatform,
                    Date = date,
                    Followers = 10000 + random.Next(-500, 1000),
                    FollowersGrowth = random.Next(-100, 500),
                    EngagementRate = (decimal)(random.NextDouble() * 10),
                    Views = random.Next(5000, 20000),
                    Likes = random.Next(500, 3000),
                    Comments = random.Next(50, 500),
                    Shares = random.Next(10, 100),
                    EstimatedRevenue = (decimal)(random.NextDouble() * 1000)
                });
            }
            
            return Ok(metrics);
        }

        [HttpGet("metrics/{creatorId}/daily")]
        public async Task<ActionResult<DailyMetricsResponse>> GetDailyMetrics(Guid creatorId)
        {
            _logger.LogInformation("TestDashboardController.GetDailyMetrics: Buscando métricas diárias para criador {CreatorId}", creatorId);
            
            if (creatorId == Guid.Empty)
            {
                return BadRequest("ID do criador inválido");
            }

            var random = new Random();
            var today = DateTime.UtcNow.Date;
            
            // Gerar métricas diárias para os últimos 30 dias
            var dailyMetrics = new List<DailyMetrics>();
            
            for (int i = 0; i < 30; i++)
            {
                var date = today.AddDays(-i);
                dailyMetrics.Add(new DailyMetrics
                {
                    CreatorId = creatorId,
                    Date = date,
                    Followers = random.Next(900, 1100) + (i * 10),
                    Views = random.Next(5000, 8000) - (i * 50),
                    Likes = random.Next(800, 1200) - (i * 15),
                    Comments = random.Next(100, 300) - (i * 5),
                    Shares = random.Next(50, 150) - (i * 2),
                    Revenue = Math.Round(random.Next(50, 200) * 1.0m - (i * 0.5m), 2)
                });
            }
            
            var response = new DailyMetricsResponse
            {
                CreatorId = creatorId,
                Metrics = dailyMetrics,
                GeneratedAt = DateTime.UtcNow
            };
            
            return Ok(response);
        }

        [HttpGet("metrics/{creatorId}/daily/{date}")]
        public ActionResult<PerformanceMetrics> GetDailyMetrics(Guid creatorId, string date)
        {
            _logger.LogInformation("TestDashboardController.GetDailyMetrics: Buscando métricas diárias para o criador {CreatorId} na data {Date}", creatorId, date);
            
            if (creatorId == Guid.Empty)
            {
                return BadRequest("ID do criador inválido");
            }
            
            if (!DateTime.TryParse(date, out DateTime specificDate))
            {
                return BadRequest("Formato de data inválido");
            }

            var random = new Random();
            
            // Criar uma métrica única para a data específica
            var metric = new PerformanceMetrics
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                Date = specificDate,
                Followers = random.Next(1000, 10000),
                FollowersGrowth = random.Next(50, 200),
                TotalViews = random.Next(5000, 50000),
                TotalLikes = random.Next(500, 5000),
                TotalComments = random.Next(100, 1000),
                TotalShares = random.Next(50, 500),
                Reach = random.Next(2000, 20000),
                Impressions = random.Next(5000, 50000),
                EngagementRate = Math.Round((decimal)random.Next(1, 8) + (decimal)random.NextDouble(), 2),
                EstimatedRevenue = Math.Round(random.Next(10, 100) * 1.0m, 2),
                Platform = (SocialMediaPlatform)random.Next(0, 3),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
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
            
            if (metrics.Id == Guid.Empty)
            {
                metrics.Id = Guid.NewGuid();
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

        [HttpGet("insights/weekly/{creatorId}")]
        public async Task<ActionResult<WeeklyInsights>> GetWeeklyInsights(Guid creatorId)
        {
            _logger.LogInformation("TestDashboardController.GetWeeklyInsights: Buscando insights semanais para criador {CreatorId}", creatorId);
            
            if (creatorId == Guid.Empty)
            {
                return BadRequest("ID do criador inválido");
            }

            string creatorIdString = creatorId.ToString();
            var random = new Random();
            
            // Gerar insights semanais
            var insights = new WeeklyInsights
            {
                CreatorId = creatorId,
                Period = $"{DateTime.UtcNow.AddDays(-7).ToShortDateString()} - {DateTime.UtcNow.ToShortDateString()}",
                FollowerGain = random.Next(20, 100),
                FollowerGrowthPercentage = Math.Round((decimal)random.Next(2, 5) + (decimal)random.NextDouble(), 2),
                AverageEngagementRate = Math.Round((decimal)random.Next(3, 6) + (decimal)random.NextDouble(), 2),
                TopPerformingContent = new List<TopContent>
                {
                    new TopContent { 
                        ContentId = Guid.NewGuid().ToString(), 
                        Title = "7 dicas para aumentar engajamento", 
                        Platform = SocialMediaPlatform.Instagram,
                        PostDate = DateTime.UtcNow.AddDays(-3),
                        EngagementRate = Math.Round((decimal)random.Next(6, 10) + (decimal)random.NextDouble(), 2),
                        Likes = random.Next(500, 1500),
                        Views = random.Next(5000, 15000)
                    },
                    new TopContent { 
                        ContentId = Guid.NewGuid().ToString(), 
                        Title = "Como criar conteúdo viral", 
                        Platform = SocialMediaPlatform.TikTok,
                        PostDate = DateTime.UtcNow.AddDays(-5),
                        EngagementRate = Math.Round((decimal)random.Next(5, 8) + (decimal)random.NextDouble(), 2),
                        Likes = random.Next(400, 1200),
                        Views = random.Next(4000, 12000)
                    }
                },
                RecommendedActions = new List<RecommendedAction>
                {
                    new RecommendedAction { 
                        Description = "Poste conteúdos semelhantes ao seu top performer deste período", 
                        Importance = ActionImportance.High 
                    },
                    new RecommendedAction { 
                        Description = "Aumente a frequência de postagens no Instagram", 
                        Importance = ActionImportance.Medium 
                    }
                },
                GeneratedAt = DateTime.UtcNow,
                CreatorIdString = creatorIdString
            };
            
            return Ok(insights);
        }

        [HttpGet("insights/monthly/{creatorId}")]
        public ActionResult<MonthlyInsights> GetMonthlyInsights(Guid creatorId)
        {
            _logger.LogInformation("TestDashboardController.GetMonthlyInsights: Buscando insights mensais para criador {CreatorId}", creatorId);
            
            if (creatorId == Guid.Empty)
            {
                return BadRequest("ID do criador inválido");
            }

            var random = new Random();
            
            var insights = new MonthlyInsights
            {
                CreatorId = creatorId,
                Month = DateTime.UtcNow.ToString("MMMM yyyy"),
                FollowerGrowth = random.Next(100, 1000),
                FollowerGrowthPercentage = Math.Round(Convert.ToDecimal(random.Next(2, 15) + random.NextDouble()), 2),
                AverageEngagementRate = Math.Round(Convert.ToDecimal(random.Next(3, 8) + random.NextDouble()), 2),
                TopContent = new List<TopContentItem>
                {
                    new TopContentItem
                    {
                        ContentId = Guid.NewGuid().ToString(),
                        Title = "Como aumentar sua produtividade em 200%",
                        Platform = SocialMediaPlatform.Instagram,
                        Likes = random.Next(500, 5000),
                        Comments = random.Next(50, 500),
                        Shares = random.Next(10, 200),
                        EngagementRate = Math.Round((decimal)random.Next(4, 12) + (decimal)random.NextDouble(), 2)
                    },
                    new TopContentItem
                    {
                        ContentId = Guid.NewGuid().ToString(),
                        Title = "5 dicas para iniciantes em programação",
                        Platform = SocialMediaPlatform.YouTube,
                        Likes = random.Next(200, 2000),
                        Comments = random.Next(30, 300),
                        Shares = random.Next(5, 100),
                        EngagementRate = Math.Round((decimal)random.Next(3, 10) + (decimal)random.NextDouble(), 2)
                    },
                    new TopContentItem
                    {
                        ContentId = Guid.NewGuid().ToString(),
                        Title = "Truques de edição de vídeo que ninguém te contou",
                        Platform = SocialMediaPlatform.TikTok,
                        Likes = random.Next(1000, 10000),
                        Comments = random.Next(100, 1000),
                        Shares = random.Next(50, 500),
                        EngagementRate = Math.Round((decimal)random.Next(5, 15) + (decimal)random.NextDouble(), 2)
                    }
                },
                RecommendedActions = new List<string>
                {
                    "Publique mais conteúdo sobre produtividade para capitalizar seu tema de maior sucesso",
                    "Aumente sua presença no TikTok, onde seu engajamento está crescendo mais rapidamente",
                    "Considere criar uma série de conteúdo sobre edição de vídeo baseada em seu post de maior sucesso"
                },
                GeneratedAt = DateTime.UtcNow
            };
            
            return Ok(insights);
        }

        [HttpGet("insights/latest/{creatorId}")]
        public ActionResult<DashboardInsights> GetLatestInsightsAsync(Guid creatorId)
        {
            _logger.LogInformation("TestDashboardController.GetLatestInsightsAsync: Obtendo insights para o criador {CreatorId}", creatorId);

            if (creatorId == Guid.Empty)
            {
                return BadRequest("Id do criador é obrigatório");
            }

            var rng = new Random();

            var insights = new DashboardInsights
            {
                CreatorId = creatorId,
                GeneratedAt = DateTime.UtcNow,
                PlatformsPerformance = new List<PlatformInsight>
                {
                    new PlatformInsight
                    {
                        Platform = SocialMediaPlatform.Instagram,
                        FollowerCount = rng.Next(1000, 50000),
                        FollowerGrowth = rng.Next(10, 100),
                        EngagementRate = Convert.ToDecimal(rng.Next(2, 8) + rng.NextDouble()),
                        ReachGrowth = Convert.ToDecimal(rng.Next(-2, 15) + rng.NextDouble()),
                        TopPerformingContent = "Vídeo sobre produtividade"
                    },
                    new PlatformInsight
                    {
                        Platform = SocialMediaPlatform.YouTube,
                        FollowerCount = rng.Next(500, 20000),
                        FollowerGrowth = rng.Next(5, 80),
                        EngagementRate = Convert.ToDecimal(rng.Next(1, 5) + rng.NextDouble()),
                        ReachGrowth = Convert.ToDecimal(rng.Next(-2, 12) + rng.NextDouble()),
                        TopPerformingContent = "Tutorial de edição de vídeo"
                    },
                    new PlatformInsight
                    {
                        Platform = SocialMediaPlatform.TikTok,
                        FollowerCount = rng.Next(2000, 100000),
                        FollowerGrowth = rng.Next(20, 150),
                        EngagementRate = Convert.ToDecimal(rng.Next(3, 12) + rng.NextDouble()),
                        ReachGrowth = Convert.ToDecimal(rng.Next(0, 25) + rng.NextDouble()),
                        TopPerformingContent = "Dance challenge"
                    }
                },
                TotalRevenue = Convert.ToDecimal(rng.Next(1000, 5000) + rng.NextDouble()),
                RevenueGrowth = Convert.ToDecimal(rng.Next(5, 20) + rng.NextDouble()),
                PostingConsistency = rng.Next(70, 95),
                DemographicData = new DemographicData
                {
                    AgeGroups = new Dictionary<string, int>
                    {
                        { "18-24", Convert.ToInt32(rng.Next(20, 35) + rng.NextDouble()) },
                        { "25-34", Convert.ToInt32(rng.Next(30, 45) + rng.NextDouble()) },
                        { "35-44", Convert.ToInt32(rng.Next(15, 25) + rng.NextDouble()) },
                        { "45+", Convert.ToInt32(rng.Next(5, 15) + rng.NextDouble()) }
                    },
                    Gender = new Dictionary<string, decimal>
                    {
                        { "Masculino", Convert.ToDecimal(rng.Next(40, 60) + rng.NextDouble()) },
                        { "Feminino", Convert.ToDecimal(rng.Next(40, 60) + rng.NextDouble()) },
                        { "Outro", Convert.ToDecimal(rng.Next(1, 5) + rng.NextDouble()) }
                    },
                    Locations = new Dictionary<string, decimal>
                    {
                        { "Brasil", Convert.ToDecimal(rng.Next(50, 70) + rng.NextDouble()) },
                        { "Portugal", Convert.ToDecimal(rng.Next(10, 20) + rng.NextDouble()) },
                        { "Estados Unidos", Convert.ToDecimal(rng.Next(5, 15) + rng.NextDouble()) },
                        { "Outros", Convert.ToDecimal(rng.Next(5, 15) + rng.NextDouble()) }
                    },
                    TopLocations = new Dictionary<string, decimal>
                    {
                        { "Brasil", Convert.ToDecimal(rng.Next(50, 70) + rng.NextDouble()) },
                        { "Portugal", Convert.ToDecimal(rng.Next(10, 20) + rng.NextDouble()) },
                        { "Estados Unidos", Convert.ToDecimal(rng.Next(5, 15) + rng.NextDouble()) },
                        { "Outros", Convert.ToDecimal(rng.Next(5, 15) + rng.NextDouble()) }
                    }
                },
                KeyInsights = new List<string>
                {
                    "Seu conteúdo sobre dicas de produtividade recebe 45% mais engajamento que outros tópicos",
                    "Posts publicados às quartas-feiras têm melhor desempenho no Instagram",
                    "Vídeos curtos (menos de 60 segundos) estão gerando 3x mais visualizações no TikTok"
                },
                ActionRecommendations = new List<string>
                {
                    "Aumente a frequência de posts no Instagram para 5 vezes por semana",
                    "Considere criar mais conteúdo orientado para o público de 25-34 anos",
                    "Experimente colaborações com outros criadores para expandir seu alcance"
                },
                GrowthOpportunities = new List<GrowthOpportunity>
                {
                    new GrowthOpportunity
                    {
                        Title = "Expansão para LinkedIn",
                        Description = "Seu conteúdo sobre desenvolvimento profissional pode ser adaptado para o LinkedIn",
                        PotentialImpact = "Alta",
                        RequiredEffort = "Média"
                    },
                    new GrowthOpportunity
                    {
                        Title = "Série de tutoriais em vídeo",
                        Description = "Transforme suas dicas populares em uma série estruturada de tutoriais",
                        PotentialImpact = "Média",
                        RequiredEffort = "Alta"
                    },
                    new GrowthOpportunity
                    {
                        Title = "Parcerias com marcas no setor de tecnologia",
                        Description = "Seu público demonstra forte interesse em produtos de tecnologia",
                        PotentialImpact = "Alta",
                        RequiredEffort = "Média"
                    }
                },
                CompetitorBenchmarks = new List<CompetitorBenchmark>
                {
                    new CompetitorBenchmark
                    {
                        MetricName = "Taxa de Engajamento",
                        YourValue = Convert.ToDecimal(rng.Next(4, 8) + rng.NextDouble()),
                        IndustryAverage = Convert.ToDecimal(rng.Next(2, 5) + rng.NextDouble()),
                        TopPerformerValue = Convert.ToDecimal(rng.Next(8, 15) + rng.NextDouble())
                    },
                    new CompetitorBenchmark
                    {
                        MetricName = "Frequência de Postagem",
                        YourValue = rng.Next(3, 7),
                        IndustryAverage = rng.Next(2, 5),
                        TopPerformerValue = rng.Next(7, 14)
                    },
                    new CompetitorBenchmark
                    {
                        MetricName = "Crescimento de Seguidores Mensal",
                        YourValue = Convert.ToDecimal(rng.Next(5, 12) + rng.NextDouble()),
                        IndustryAverage = Convert.ToDecimal(rng.Next(3, 7) + rng.NextDouble()),
                        TopPerformerValue = Convert.ToDecimal(rng.Next(12, 25) + rng.NextDouble())
                    }
                }
            };

            insights.PlatformsPerformance2 = new List<PlatformPerformance>
            {
                new PlatformPerformance
                {
                    Platform = SocialMediaPlatform.Instagram,
                    Followers = rng.Next(1000, 50000),
                    FollowersGrowth = Convert.ToDecimal(rng.Next(1, 10) + rng.NextDouble()),
                    AvgEngagementRate = Convert.ToDecimal(rng.Next(2, 8) + rng.NextDouble()),
                    ReachGrowth = Convert.ToDecimal(rng.Next(-2, 15) + rng.NextDouble()),
                    ImpressionGrowth = Convert.ToDecimal(rng.Next(-3, 20) + rng.NextDouble())
                },
                new PlatformPerformance
                {
                    Platform = SocialMediaPlatform.YouTube,
                    Followers = rng.Next(500, 20000),
                    FollowersGrowth = Convert.ToDecimal(rng.Next(1, 8) + rng.NextDouble()),
                    AvgEngagementRate = Convert.ToDecimal(rng.Next(1, 5) + rng.NextDouble()),
                    ReachGrowth = Convert.ToDecimal(rng.Next(-2, 12) + rng.NextDouble()),
                    ImpressionGrowth = Convert.ToDecimal(rng.Next(-3, 15) + rng.NextDouble())
                },
                new PlatformPerformance
                {
                    Platform = SocialMediaPlatform.TikTok,
                    Followers = rng.Next(2000, 100000),
                    FollowersGrowth = Convert.ToDecimal(rng.Next(2, 15) + rng.NextDouble()),
                    AvgEngagementRate = Convert.ToDecimal(rng.Next(3, 12) + rng.NextDouble()),
                    ReachGrowth = Convert.ToDecimal(rng.Next(0, 25) + rng.NextDouble()),
                    ImpressionGrowth = Convert.ToDecimal(rng.Next(0, 30) + rng.NextDouble())
                }
            };

            return Ok(insights);
        }

        [HttpGet("revenue/overview/{creatorId}")]
        public ActionResult<RevenueOverview> GetRevenueOverviewAsync(Guid creatorId)
        {
            _logger.LogInformation("TestDashboardController.GetRevenueOverviewAsync: Buscando visão geral de receita para o criador {CreatorId}", creatorId);
            
            if (creatorId == Guid.Empty)
            {
                return BadRequest("ID do criador inválido");
            }
            
            var random = new Random();
            
            var overview = new RevenueOverview
            {
                CreatorId = creatorId.ToString(),
                TotalRevenue = Convert.ToDecimal(random.Next(5000, 50000) + random.NextDouble()),
                RevenueGrowth = Convert.ToDecimal(random.Next(5, 25) + random.NextDouble()),
                MonthlyBreakdown = new List<MonthlyRevenue>
                {
                    new MonthlyRevenue
                    {
                        Month = DateTime.UtcNow.AddMonths(-5).ToString("MMM yyyy"),
                        Amount = (decimal)Math.Round(random.Next(3000, 8000) + random.NextDouble(), 2)
                    },
                    new MonthlyRevenue
                    {
                        Month = DateTime.UtcNow.AddMonths(-4).ToString("MMM yyyy"),
                        Amount = (decimal)Math.Round(random.Next(3500, 9000) + random.NextDouble(), 2)
                    },
                    new MonthlyRevenue
                    {
                        Month = DateTime.UtcNow.AddMonths(-3).ToString("MMM yyyy"),
                        Amount = (decimal)Math.Round(random.Next(4000, 10000) + random.NextDouble(), 2)
                    },
                    new MonthlyRevenue
                    {
                        Month = DateTime.UtcNow.AddMonths(-2).ToString("MMM yyyy"),
                        Amount = (decimal)Math.Round(random.Next(4500, 11000) + random.NextDouble(), 2)
                    },
                    new MonthlyRevenue
                    {
                        Month = DateTime.UtcNow.AddMonths(-1).ToString("MMM yyyy"),
                        Amount = (decimal)Math.Round(random.Next(5000, 12000) + random.NextDouble(), 2)
                    },
                    new MonthlyRevenue
                    {
                        Month = DateTime.UtcNow.ToString("MMM yyyy"),
                        Amount = (decimal)Math.Round(random.Next(5500, 13000) + random.NextDouble(), 2)
                    }
                },
                RevenueSources = new List<RevenueSource>
                {
                    new RevenueSource
                    {
                        Source = "Produtos Digitais",
                        Amount = (decimal)Math.Round(random.Next(2000, 20000) + random.NextDouble(), 2),
                        Percentage = (decimal)Math.Round(random.Next(25, 45) + random.NextDouble(), 2)
                    },
                    new RevenueSource
                    {
                        Source = "Patrocínios",
                        Amount = (decimal)Math.Round(random.Next(1500, 15000) + random.NextDouble(), 2),
                        Percentage = (decimal)Math.Round(random.Next(20, 35) + random.NextDouble(), 2)
                    },
                    new RevenueSource
                    {
                        Source = "Assinaturas",
                        Amount = (decimal)Math.Round(random.Next(1000, 10000) + random.NextDouble(), 2),
                        Percentage = (decimal)Math.Round(random.Next(15, 30) + random.NextDouble(), 2)
                    },
                    new RevenueSource
                    {
                        Source = "Outros",
                        Amount = (decimal)Math.Round(random.Next(500, 5000) + random.NextDouble(), 2),
                        Percentage = (decimal)Math.Round(random.Next(5, 15) + random.NextDouble(), 2)
                    }
                }
            };
            
            return Ok(overview);
        }

        [HttpGet("content-performance/{creatorId}")]
        public ActionResult<List<TestContentPerformance>> GetContentPerformanceAsync(Guid creatorId)
        {
            _logger.LogInformation("TestDashboardController.GetContentPerformanceAsync: Obtendo desempenho de conteúdo para o criador {CreatorId}", creatorId);
            
            if (creatorId == Guid.Empty)
            {
                return BadRequest("Id do criador é obrigatório");
            }
            
            var random = new Random();
            var performances = new List<TestContentPerformance>();
            
            for (int i = 0; i < 5; i++)
            {
                var performance = new TestContentPerformance
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    ContentId = Guid.NewGuid(),
                    Title = $"Conteúdo teste {i + 1}",
                    Platform = (SocialMediaPlatform)(i % 3),
                    PostUrl = $"https://example.com/post/{i}",
                    PostedAt = DateTime.UtcNow.AddDays(-i),
                    Likes = random.Next(100, 5000),
                    Comments = random.Next(10, 500),
                    Shares = random.Next(5, 200),
                    Views = random.Next(1000, 50000),
                    EngagementRate = Convert.ToDecimal(random.Next(1, 10) / 10.0f),
                    Demographics = new AudienceDemographics
                    {
                        AgeGroups = new Dictionary<string, decimal>
                        {
                            {"18-24", random.Next(10, 40) / 100.0m},
                            {"25-34", random.Next(20, 50) / 100.0m},
                            {"35-44", random.Next(10, 30) / 100.0m},
                            {"45+", random.Next(5, 20) / 100.0m}
                        },
                        Gender = new Dictionary<string, decimal>
                        {
                            {"Masculino", random.Next(30, 70) / 100.0m},
                            {"Feminino", random.Next(30, 70) / 100.0m},
                            {"Outro", random.Next(1, 10) / 100.0m}
                        },
                        TopCountries = new Dictionary<string, decimal>
                        {
                            {"Brasil", random.Next(40, 80) / 100.0m},
                            {"Portugal", random.Next(5, 20) / 100.0m},
                            {"Estados Unidos", random.Next(5, 15) / 100.0m}
                        }
                    }
                };
                
                performances.Add(performance);
            }
            
            return Ok(performances.OrderByDescending(p => p.EngagementRate).ToList());
        }

        [HttpGet("analytics/content/{creatorId}")]
        public ActionResult<List<TestContentPerformance>> GetContentAnalyticsAsync(Guid creatorId)
        {
            _logger.LogInformation("TestDashboardController.GetContentAnalyticsAsync: Obtendo análise de conteúdo para o criador {CreatorId}", creatorId);
            
            if (creatorId == Guid.Empty)
            {
                return BadRequest("Id do criador é obrigatório");
            }
            
            var rng = new Random();
            var platforms = new[] { SocialMediaPlatform.Instagram, SocialMediaPlatform.TikTok, SocialMediaPlatform.YouTube };
            
            var contentAnalytics = new List<TestContentPerformance>();
            
            for (int i = 0; i < 10; i++)
            {
                var platform = platforms[rng.Next(platforms.Length)];
                var performance = new TestContentPerformance
                {
                    Id = Guid.NewGuid(),
                    ContentId = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Platform = platform,
                    Title = $"Conteúdo de teste {i+1}",
                    PostedAt = DateTime.UtcNow.AddDays(-rng.Next(1, 30)),
                    Likes = rng.Next(100, 5000),
                    Comments = rng.Next(10, 500),
                    Shares = rng.Next(5, 200),
                    Views = rng.Next(1000, 50000),
                    Impressions = rng.Next(2000, 100000),
                    Saves = rng.Next(10, 300),
                    DemographicData = new ContentDemographicData
                    {
                        AgeGroups = new Dictionary<string, decimal>
                        {
                            { "18-24", Math.Round((decimal)rng.Next(20, 35) + (decimal)rng.NextDouble(), 2) },
                            { "25-34", Math.Round((decimal)rng.Next(30, 45) + (decimal)rng.NextDouble(), 2) },
                            { "35-44", Math.Round((decimal)rng.Next(15, 25) + (decimal)rng.NextDouble(), 2) },
                            { "45+", Math.Round((decimal)rng.Next(5, 15) + (decimal)rng.NextDouble(), 2) }
                        },
                        GenderDistribution = new Dictionary<string, decimal>
                        {
                            { "Masculino", Math.Round((decimal)rng.Next(40, 60) + (decimal)rng.NextDouble(), 2) },
                            { "Feminino", Math.Round((decimal)rng.Next(40, 60) + (decimal)rng.NextDouble(), 2) },
                            { "Outro", Math.Round((decimal)rng.Next(1, 5) + (decimal)rng.NextDouble(), 2) }
                        }
                    }
                };
                
                contentAnalytics.Add(performance);
            }
            
            return Ok(contentAnalytics);
        }

        [HttpGet("content/{creatorId}")]
        public async Task<ActionResult<List<ContentMetrics>>> GetContentMetrics(Guid creatorId)
        {
            _logger.LogInformation("TestDashboardController.GetContentMetrics: Buscando métricas de conteúdo para criador {CreatorId}", creatorId);
            
            if (creatorId == Guid.Empty)
            {
                return BadRequest("ID do criador inválido");
            }

            var random = new Random();
            var today = DateTime.UtcNow.Date;
            
            // Gerar métricas para 10 conteúdos
            var contentMetrics = new List<ContentMetrics>();
            
            for (int i = 0; i < 10; i++)
            {
                var date = today.AddDays(-random.Next(1, 30));
                contentMetrics.Add(new ContentMetrics
                {
                    ContentId = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = $"Conteúdo #{i+1}",
                    Platform = (SocialMediaPlatform)random.Next(0, 5),
                    PublishedDate = date,
                    Views = random.Next(1000, 10000),
                    Likes = random.Next(100, 2000),
                    Comments = random.Next(10, 500),
                    Shares = random.Next(5, 200),
                    EngagementRate = Math.Round((decimal)random.Next(1, 10) + (decimal)random.NextDouble(), 2),
                    Revenue = Math.Round(random.Next(10, 100) * 1.0m, 2)
                });
            }
            
            return Ok(contentMetrics);
        }

        [HttpGet("recommendations/{creatorId}")]
        public async Task<IActionResult> GetRecommendations(Guid creatorId)
        {
            _logger.LogInformation($"Getting recommendations for creator {creatorId}");

            // Validar creatorId
            if (creatorId == Guid.Empty)
            {
                return new BadRequestObjectResult("CreatorId não pode ser vazio");
            }

            // Array de tipos de recomendação como strings
            string[] types = new string[]
            {
                "ContentTopic",
                "PostingTime",
                "MonetizationStrategy",
                "EngagementTactic"
            };

            var recommendations = new List<RecommendationDto>();

            for (int i = 0; i < 5; i++)
            {
                var recommendation = new RecommendationDto
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Title = $"Recomendação {i + 1}",
                    Description = $"Descrição da recomendação {i + 1}",
                    // Usar diretamente o tipo como string
                    Type = types[_random.Next(types.Length)],
                    Priority = _random.Next(1, 6),
                    PotentialImpact = $"Impacto potencial {_random.Next(1, 101)}%",
                    RecommendedAction = $"Ação recomendada {i + 1}",
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 30))
                };

                recommendations.Add(recommendation);
            }

            return new OkObjectResult(recommendations);
        }

        public class MetricDataPoint
        {
            public string Date { get; set; }
            public object Value { get; set; }
        }
        
        private List<MetricDataPoint> GenerateMetricData(int days, int minValue, int maxValue, bool isRevenue = false)
        {
            var random = new Random();
            var result = new List<MetricDataPoint>();
            
            for (int i = 0; i < days; i++)
            {
                var date = DateTime.Now.AddDays(-i).ToString("yyyy-MM-dd");
                object value;
                
                if (isRevenue)
                {
                    value = Math.Round(random.NextDouble() * (maxValue - minValue) + minValue, 2);
                }
                else
                {
                    value = random.Next(minValue, maxValue + 1);
                }
                
                result.Add(new MetricDataPoint
                {
                    Date = date,
                    Value = value
                });
            }
            
            return result;
        }

        // Classes adicionais para uso neste controller
        public class DailyMetrics
        {
            public Guid CreatorId { get; set; }
            public DateTime Date { get; set; }
            public int Followers { get; set; }
            public int Views { get; set; }
            public int Likes { get; set; }
            public int Comments { get; set; }
            public int Shares { get; set; }
            public decimal Revenue { get; set; }
        }

        public class DailyMetricsResponse
        {
            public Guid CreatorId { get; set; }
            public List<DailyMetrics> Metrics { get; set; } = new List<DailyMetrics>();
            public DateTime GeneratedAt { get; set; }
        }

        public class ContentMetrics
        {
            public Guid ContentId { get; set; }
            public Guid CreatorId { get; set; }
            public string Title { get; set; } = string.Empty;
            public SocialMediaPlatform Platform { get; set; }
            public DateTime PublishedDate { get; set; }
            public int Views { get; set; }
            public int Likes { get; set; }
            public int Comments { get; set; }
            public int Shares { get; set; }
            public decimal EngagementRate { get; set; }
            public decimal Revenue { get; set; }
        }

        public class WeeklyInsights
        {
            public Guid CreatorId { get; set; }
            public string Period { get; set; } = string.Empty;
            public int FollowerGain { get; set; }
            public decimal FollowerGrowthPercentage { get; set; }
            public decimal AverageEngagementRate { get; set; }
            public List<TopContent> TopPerformingContent { get; set; } = new List<TopContent>();
            public List<RecommendedAction> RecommendedActions { get; set; } = new List<RecommendedAction>();
            public DateTime GeneratedAt { get; set; }
            public string CreatorIdString { get; set; } = string.Empty;
        }

        public class MonthlyInsights
        {
            public Guid Id { get; set; }
            public Guid CreatorId { get; set; }
            public DateTime MonthStartDate { get; set; }
            public DateTime MonthEndDate { get; set; }
            public int TotalFollowers { get; set; }
            public int FollowersGrowth { get; set; }
            public int FollowerGrowth { get; set; }
            public int TotalPosts { get; set; }
            public long TotalViews { get; set; }
            public decimal AverageEngagementRate { get; set; }
            public decimal TotalRevenue { get; set; }
            public decimal RevenueGrowth { get; set; }
            public List<TopContentItem> TopPerformingContent { get; set; } = new List<TopContentItem>();
            public DateTime GeneratedAt { get; set; }
            public string Month { get; set; }
            public decimal FollowerGrowthPercentage { get; set; }
            public List<TopContentItem> TopContent { get; set; } = new List<TopContentItem>();
            public List<string> RecommendedActions { get; set; } = new List<string>();
        }

        public class TopContent
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public SocialMediaPlatform Platform { get; set; }
            public decimal EngagementRate { get; set; }
            public long Views { get; set; }
            public DateTime PublishedAt { get; set; }
            public string ContentId { get; set; }
            public DateTime PostDate { get; set; }
            public int Likes { get; set; }
        }

        public class RecommendedAction
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public ActionImportance Importance { get; set; }
            public int PotentialImpressionIncrease { get; set; }
        }

        public enum ActionImportance
        {
            Low,
            Medium,
            High,
            Critical
        }

        public class TopContentItem
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public SocialMediaPlatform Platform { get; set; }
            public decimal EngagementRate { get; set; }
            public long Views { get; set; }
            public string ContentId { get; set; }
            public int Likes { get; set; }
            public int Comments { get; set; }
            public int Shares { get; set; }
        }

        public class PlatformPerformance
        {
            public SocialMediaPlatform Platform { get; set; }
            public int Followers { get; set; }
            public decimal FollowersGrowth { get; set; }
            public decimal AvgEngagementRate { get; set; }
            public decimal ReachGrowth { get; set; }
            public decimal ImpressionGrowth { get; set; }
        }

        public class DemographicData
        {
            public Dictionary<string, int> AgeGroups { get; set; } = new Dictionary<string, int>();
            public Dictionary<string, decimal> Gender { get; set; } = new Dictionary<string, decimal>();
            public Dictionary<string, decimal> Locations { get; set; } = new Dictionary<string, decimal>();
            public Dictionary<string, decimal> InterestCategories { get; set; } = new Dictionary<string, decimal>();
            public Dictionary<string, decimal> GenderDistribution { get; set; } = new Dictionary<string, decimal>();
            public Dictionary<string, decimal> TopLocations { get; set; } = new Dictionary<string, decimal>();
        }

        public class GrowthOpportunity
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string PotentialImpact { get; set; }
            public string RequiredEffort { get; set; }
        }

        public class CompetitorBenchmark
        {
            public string Metric { get; set; }
            public decimal YourValue { get; set; }
            public decimal AverageInNiche { get; set; }
            public decimal Difference { get; set; }
            public bool IsPositive { get; set; }
            public string MetricName { get; set; }
            public decimal TopPerformerValue { get; set; }
            public decimal IndustryAverage { get; set; }
        }

        public class RevenueOverview
        {
            public decimal TotalRevenue { get; set; }
            public decimal GrowthPercentage { get; set; }
            public decimal ProjectedRevenue { get; set; }
            public Dictionary<string, decimal> PlatformBreakdown { get; set; } = new Dictionary<string, decimal>();
            public List<MonthlyRevenue> MonthlyTrend { get; set; } = new List<MonthlyRevenue>();
            public List<RevenueSource> RevenueSources { get; set; } = new List<RevenueSource>();
            public string CreatorId { get; set; }
            public decimal RevenueGrowth { get; set; }
            public List<MonthlyRevenue> MonthlyBreakdown { get; set; } = new List<MonthlyRevenue>();
        }

        public class MonthlyRevenue
        {
            public string Month { get; set; }
            public decimal Amount { get; set; }
            public decimal GrowthPercentage { get; set; }
        }

        public class RevenueSource
        {
            public string Name { get; set; }
            public decimal Amount { get; set; }
            public decimal Percentage { get; set; }
            public decimal GrowthPercentage { get; set; }
            public string Source { get; set; }
        }

        // Classe estendida de ContentPerformance para testes
        public class TestContentPerformance : ContentPerformance
        {
            public Guid ContentId { get; set; }
            public string Title { get; set; }
            public string PostUrl { get; set; }
            public DateTime PostedAt { get; set; }
            public long Impressions { get; set; }
            public int Saves { get; set; }
            public ContentDemographicData DemographicData { get; set; }
            public AudienceDemographics Demographics { get; set; }
        }

        public class ContentDemographicData
        {
            public Dictionary<string, decimal> AgeGroups { get; set; } = new Dictionary<string, decimal>();
            public Dictionary<string, decimal> GenderDistribution { get; set; } = new Dictionary<string, decimal>();
            public Dictionary<string, decimal> TopLocations { get; set; } = new Dictionary<string, decimal>();
        }

        public class DashboardInsights
        {
            public Guid Id { get; set; }
            public Guid CreatorId { get; set; }
            public DateTime Date { get; set; }
            public DateTime PeriodStart { get; set; }
            public DateTime PeriodEnd { get; set; }
            public DateTime GeneratedDate { get; set; }
            public DateTime GeneratedAt { get; set; }
            public List<SocialMediaPlatform> Platforms { get; set; } = new();
            public List<PlatformInsight> PlatformsPerformance { get; set; } = new();
            public int TotalFollowers { get; set; }
            public int TotalPosts { get; set; }
            public int TotalViews { get; set; }
            public int TotalLikes { get; set; }
            public int TotalComments { get; set; }
            public int TotalShares { get; set; }
            public decimal AverageEngagementRate { get; set; }
            public decimal TotalRevenue { get; set; }
            public decimal TotalRevenueInPeriod { get; set; }
            public decimal GrowthRate { get; set; }
            public decimal RevenueGrowth { get; set; }
            public int PostingConsistency { get; set; }
            public AudienceDemographics AudionceDemographics { get; set; } = new();
            public Dictionary<string, decimal> ContentTypePerformance { get; set; } = new();
            public List<string> KeyInsights { get; set; } = new();
            public List<string> ActionRecommendations { get; set; } = new();
            public List<GrowthOpportunity> GrowthOpportunities { get; set; } = new();
            public List<CompetitorComparison> CompetitorBenchmark { get; set; } = new();
            public DateRange AnalysisPeriod { get; set; } = new();
            public InsightType Type { get; set; }
            public Dictionary<string, decimal> ComparisonWithPreviousPeriod { get; set; } = new();
            public List<ContentInsight> TopContentInsights { get; set; } = new();
            public List<PostTimeRecommendation> BestTimeToPost { get; set; } = new();
            public List<ContentRecommendation> Recommendations { get; set; } = new();
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public List<ContentInsight> Insights { get; set; } = new();
            
            // Campos adicionais para compatibilidade
            public List<PlatformPerformance> PlatformsPerformance2 { get; set; } = new List<PlatformPerformance>();
            public DemographicData DemographicData { get; set; }
            public List<CompetitorBenchmark> CompetitorBenchmarks { get; set; } = new List<CompetitorBenchmark>();
        }

        // Classes adicionais necessárias
        public class ContentInsight
        {
            public Guid PostId { get; set; }
            public string Title { get; set; }
            public SocialMediaPlatform Platform { get; set; }
            public DateTime PublishedAt { get; set; }
            public long Views { get; set; }
            public decimal EngagementRate { get; set; }
            public decimal Revenue { get; set; }
            public InsightType InsightType { get; set; }
            public string Insight { get; set; }
        }

        public enum InsightType
        {
            Normal,
            HighEngagement,
            HighReach,
            HighRevenue
        }

        public class ContentRecommendation
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string RecommendationType { get; set; }
            public decimal ExpectedImpactPercentage { get; set; }
            public List<string> Tags { get; set; } = new();
        }

        public class PostTimeRecommendation
        {
            public DayOfWeek DayOfWeek { get; set; }
            public TimeSpan TimeOfDay { get; set; }
            public double EngagementScore { get; set; }
        }

        public class PlatformInsight
        {
            public SocialMediaPlatform Platform { get; set; }
            public int FollowerCount { get; set; }
            public int FollowerGrowth { get; set; }
            public decimal EngagementRate { get; set; }
            public decimal ReachGrowth { get; set; }
            public string TopPerformingContent { get; set; } = string.Empty;
        }

        public class DateRange
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public class RecommendationDto
        {
            public Guid Id { get; set; }
            public Guid CreatorId { get; set; }
            public string Type { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public int Priority { get; set; }
            public string PotentialImpact { get; set; } = string.Empty;
            public string RecommendedAction { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
        }
    }
} 