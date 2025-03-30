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
        public async Task<ActionResult<DashboardInsights>> GetLatestInsights(Guid creatorId)
        {
            _logger.LogInformation("TestDashboardController.GetLatestInsights: Buscando insights para criador {CreatorId}", creatorId);
            
            // Verificação do token de autenticação
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.Contains("invalid-token"))
            {
                // Usa StatusCode diretamente para evitar dependência de configuração de autenticação
                return StatusCode(403);
            }
            
            var insight = _insights.FirstOrDefault(i => i.CreatorId == creatorId);
            if (insight == null)
            {
                // Criar insights de exemplo se não existirem
                var now = DateTime.UtcNow;
                var startDate = now.AddDays(-30);
                var endDate = now;
                
                insight = new DashboardInsights
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    GeneratedDate = now,
                    Platforms = new List<SocialMediaPlatform> { SocialMediaPlatform.Instagram, SocialMediaPlatform.YouTube },
                    PeriodStart = startDate,
                    PeriodEnd = endDate,
                    GrowthRate = 3.5m,
                    TotalRevenueInPeriod = 2500.00m,
                    ComparisonWithPreviousPeriod = 12.5m,
                    TopContentInsights = new List<ContentInsight>
                    {
                        new ContentInsight
                        {
                            Id = Guid.NewGuid(),
                            Title = "Vídeo de tutorial tem alto engajamento",
                            Type = InsightType.HighEngagement,
                            Description = "Seus tutoriais estão gerando 2x mais engajamento que outros conteúdos",
                            RecommendedAction = "Postar mais vídeos tutoriais"
                        },
                        new ContentInsight
                        {
                            Id = Guid.NewGuid(),
                            Title = "Posts de dicas rápidas muito compartilhados",
                            Type = InsightType.Performance,
                            Description = "Conteúdos breves e diretos têm taxa de compartilhamento 40% maior",
                            RecommendedAction = "Aumentar frequência de publicações de dicas"
                        },
                        new ContentInsight
                        {
                            Id = Guid.NewGuid(),
                            Title = "Análises de tendências geram discussão",
                            Type = InsightType.Engagement,
                            Description = "Seus posts sobre análise de tendências geram 50% mais comentários",
                            RecommendedAction = "Criar uma série de conteúdo sobre análise de tendências"
                        }
                    },
                    Recommendations = new List<ContentRecommendation>
                    {
                        new ContentRecommendation
                        {
                            Id = Guid.NewGuid(),
                            Title = "Aumentar frequência no Instagram",
                            Description = "Aumente a frequência de publicações no Instagram para melhorar o alcance",
                            Priority = RecommendationPriority.High,
                            Type = MicroSaaS.Shared.Enums.RecommendationType.PostingFrequency
                        },
                        new ContentRecommendation
                        {
                            Id = Guid.NewGuid(),
                            Title = "Criar série de tutoriais",
                            Description = "Desenvolva uma série de tutoriais conectados para aumentar retenção",
                            Priority = RecommendationPriority.Medium,
                            Type = MicroSaaS.Shared.Enums.RecommendationType.ContentFormat
                        },
                        new ContentRecommendation
                        {
                            Id = Guid.NewGuid(),
                            Title = "Experimente vídeos mais curtos no TikTok",
                            Description = "Vídeos entre 15-30 segundos têm melhor desempenho nesta plataforma",
                            Priority = RecommendationPriority.Medium,
                            Type = MicroSaaS.Shared.Enums.RecommendationType.Platform
                        }
                    },
                    BestTimeToPost = new List<PostTimeRecommendation>
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
                        }
                    },
                    CreatedAt = now,
                    UpdatedAt = now,
                    Date = now.Date,
                    TotalFollowers = 5000,
                    TotalPosts = 120,
                    TotalViews = 50000,
                    TotalLikes = 15000,
                    TotalComments = 3000,
                    TotalShares = 1800,
                    AverageEngagementRate = 4.5m,
                    TotalRevenue = 2800.00m,
                    Type = InsightType.Normal,
                    Insights = new List<ContentInsight>
                    {
                        new ContentInsight
                        {
                            Id = Guid.NewGuid(),
                            Title = "Seguidores crescendo mais rápido no Instagram",
                            Type = InsightType.Growth,
                            Description = "Crescimento de seguidores no Instagram é 3x maior que no YouTube",
                            RecommendedAction = "Focar esforços no Instagram para acelerar crescimento"
                        },
                        new ContentInsight
                        {
                            Id = Guid.NewGuid(),
                            Title = "TikTok mostra potencial inexplorado",
                            Type = InsightType.Trend,
                            Description = "Taxa de conversão no TikTok é promissora apesar do menor volume",
                            RecommendedAction = "Aumentar presença no TikTok com conteúdo nativo da plataforma"
                        }
                    }
                };
                _insights.Add(insight);
            }

            return Ok(insight);
        }

        [HttpGet("insights/{creatorId}/generate")]
        public async Task<ActionResult<DashboardInsights>> GenerateInsights(
            Guid creatorId, 
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            _logger.LogInformation("TestDashboardController.GenerateInsights: Gerando insights para criador {CreatorId}", creatorId);
            
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;
            var now = DateTime.UtcNow;
            
            var insight = new DashboardInsights
            {
                Id = Guid.NewGuid(),
                CreatorId = creatorId,
                GeneratedDate = now,
                Platforms = new List<SocialMediaPlatform> { SocialMediaPlatform.Instagram, SocialMediaPlatform.YouTube, SocialMediaPlatform.TikTok },
                PeriodStart = start,
                PeriodEnd = end,
                GrowthRate = 4.2m,
                TotalRevenueInPeriod = 2800.00m,
                ComparisonWithPreviousPeriod = 15.2m,
                TopContentInsights = new List<ContentInsight>
                {
                    new ContentInsight
                    {
                        Id = Guid.NewGuid(),
                        Title = "Vídeo de tutorial tem alto engajamento",
                        Type = InsightType.HighEngagement,
                        Description = "Seus tutoriais estão gerando 2x mais engajamento que outros conteúdos",
                        RecommendedAction = "Postar mais vídeos tutoriais"
                    },
                    new ContentInsight
                    {
                        Id = Guid.NewGuid(),
                        Title = "Posts de dicas rápidas muito compartilhados",
                        Type = InsightType.Performance,
                        Description = "Conteúdos breves e diretos têm taxa de compartilhamento 40% maior",
                        RecommendedAction = "Aumentar frequência de publicações de dicas"
                    },
                    new ContentInsight
                    {
                        Id = Guid.NewGuid(),
                        Title = "Análises de tendências geram discussão",
                        Type = InsightType.Engagement,
                        Description = "Seus posts sobre análise de tendências geram 50% mais comentários",
                        RecommendedAction = "Criar uma série de conteúdo sobre análise de tendências"
                    }
                },
                Recommendations = new List<ContentRecommendation>
                {
                    new ContentRecommendation
                    {
                        Id = Guid.NewGuid(),
                        Title = "Aumentar frequência no Instagram",
                        Description = "Aumente a frequência de publicações no Instagram para melhorar o alcance",
                        Priority = RecommendationPriority.High,
                        Type = MicroSaaS.Shared.Enums.RecommendationType.PostingFrequency
                    },
                    new ContentRecommendation
                    {
                        Id = Guid.NewGuid(),
                        Title = "Criar série de tutoriais",
                        Description = "Desenvolva uma série de tutoriais conectados para aumentar retenção",
                        Priority = RecommendationPriority.Medium,
                        Type = MicroSaaS.Shared.Enums.RecommendationType.ContentFormat
                    },
                    new ContentRecommendation
                    {
                        Id = Guid.NewGuid(),
                        Title = "Experimente vídeos mais curtos no TikTok",
                        Description = "Vídeos entre 15-30 segundos têm melhor desempenho nesta plataforma",
                        Priority = RecommendationPriority.Medium,
                        Type = MicroSaaS.Shared.Enums.RecommendationType.Platform
                    }
                },
                BestTimeToPost = new List<PostTimeRecommendation>
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
                        EngagementScore = 8.2
                    }
                },
                CreatedAt = now,
                UpdatedAt = now,
                Date = now.Date,
                TotalFollowers = 5500,
                TotalPosts = 140,
                TotalViews = 75000,
                TotalLikes = 22000,
                TotalComments = 4500,
                TotalShares = 2200,
                AverageEngagementRate = 4.8m,
                TotalRevenue = 3200.00m,
                Type = InsightType.Normal,
                Insights = new List<ContentInsight>
                {
                    new ContentInsight
                    {
                        Id = Guid.NewGuid(),
                        Title = "Seguidores crescendo mais rápido no Instagram",
                        Type = InsightType.Growth,
                        Description = "Crescimento de seguidores no Instagram é 3x maior que no YouTube",
                        RecommendedAction = "Focar esforços no Instagram para acelerar crescimento"
                    },
                    new ContentInsight
                    {
                        Id = Guid.NewGuid(),
                        Title = "TikTok mostra potencial inexplorado",
                        Type = InsightType.Trend,
                        Description = "Taxa de conversão no TikTok é promissora apesar do menor volume",
                        RecommendedAction = "Aumentar presença no TikTok com conteúdo nativo da plataforma"
                    }
                }
            };
            
            _insights.Add(insight);
            return Ok(insight);
        }

        [HttpGet("metrics/{creatorId}")]
        public async Task<ActionResult<IEnumerable<PerformanceMetrics>>> GetMetrics(
            Guid creatorId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] SocialMediaPlatform? platform = null)
        {
            _logger.LogInformation("TestDashboardController.GetMetrics: Buscando métricas para criador {CreatorId}", creatorId);
            
            if (!_metrics.Any(m => m.CreatorId == creatorId))
            {
                // Criar métricas de exemplo
                for (int i = 0; i < 10; i++)
                {
                    var date = DateTime.UtcNow.Date.AddDays(-i);
                    _metrics.Add(new PerformanceMetrics
                    {
                        Id = Guid.NewGuid(),
                        CreatorId = creatorId,
                        Date = date,
                        Platform = i % 2 == 0 ? SocialMediaPlatform.Instagram : SocialMediaPlatform.YouTube,
                        Followers = 5000 + (i * 50),
                        FollowersGrowth = i * 10,
                        TotalViews = 1500 + (i * 100),
                        TotalLikes = 750 + (i * 50),
                        TotalComments = 120 + (i * 10),
                        TotalShares = 60 + (i * 5),
                        EngagementRate = 4.8m - (i * 0.1m),
                        EstimatedRevenue = 350.00m + (i * 25.00m),
                        CreatedAt = date,
                        UpdatedAt = date
                    });
                }
            }
            
            // Filtrando
            var result = _metrics.Where(m => m.CreatorId == creatorId);
            
            if (startDate.HasValue)
                result = result.Where(m => m.Date >= startDate.Value);
                
            if (endDate.HasValue)
                result = result.Where(m => m.Date <= endDate.Value);
                
            if (platform.HasValue)
                result = result.Where(m => m.Platform == platform.Value);
            
            return Ok(result.ToList());
        }

        [HttpGet("metrics/{creatorId}/daily")]
        public async Task<ActionResult<PerformanceMetrics>> GetDailyMetrics(
            Guid creatorId,
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
                // Criar métrica de exemplo se não existir
                metric = new PerformanceMetrics
                {
                    Id = Guid.NewGuid(),
                    CreatorId = creatorId,
                    Date = targetDate,
                    Platform = platform,
                    TotalViews = 1500,
                    TotalLikes = 750,
                    TotalComments = 120,
                    TotalShares = 60,
                    Followers = 5500,
                    EngagementRate = 4.8m,
                    EstimatedRevenue = 350.00m,
                    TopPerformingContentIds = new List<string> { "post1", "post2" },
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _metrics.Add(metric);
            }

            return Ok(metric);
        }

        [HttpGet("content/{creatorId}/top")]
        public async Task<ActionResult<List<ContentPost>>> GetTopContent(
            Guid creatorId,
            [FromQuery] int limit = 5)
        {
            _logger.LogInformation("TestDashboardController.GetTopContent: Buscando top conteúdos para criador {CreatorId}", creatorId);
            
            if (!_contentPosts.Any(p => p.CreatorId == creatorId))
            {
                // Criar posts de exemplo
                for (int i = 0; i < 10; i++)
                {
                    _contentPosts.Add(new ContentPost
                    {
                        Id = Guid.NewGuid(),
                        CreatorId = creatorId,
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
                .Where(p => p.CreatorId == creatorId)
                .OrderByDescending(p => p.EngagementRate)
                .Take(limit)
                .ToList();
                
            return Ok(topPosts);
        }

        [HttpGet("recommendations/{creatorId}/posting-times")]
        public async Task<ActionResult<List<PostTimeRecommendation>>> GetBestTimeToPost(
            Guid creatorId,
            [FromQuery] SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
        {
            _logger.LogInformation("TestDashboardController.GetBestTimeToPost: Buscando melhores horários para criador {CreatorId}", creatorId);
            
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
            Guid creatorId,
            [FromQuery] SocialMediaPlatform platform = SocialMediaPlatform.Instagram)
        {
            _logger.LogInformation("TestDashboardController.GetAverageEngagementRate: Calculando taxa de engajamento para criador {CreatorId}", creatorId);
            
            // Retornar taxa de engajamento de exemplo
            decimal engagementRate = 4.8m;
            return Ok(engagementRate);
        }

        [HttpGet("analytics/{creatorId}/revenue-growth")]
        public async Task<ActionResult<decimal>> GetRevenueGrowth(
            Guid creatorId,
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
            Guid creatorId,
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
            
            return Created($"/api/Dashboard/content/{performance.CreatorId}/top", performance);
        }
    }
} 