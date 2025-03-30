using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;
using MicroSaaS.Shared.Enums;

namespace MicroSaaS.IntegrationTests.Utils
{
    [ApiController]
    [Route("api/Revenue")]
    public class TestRevenueController : ControllerBase
    {
        private readonly ILogger<TestRevenueController> _logger;
        private static readonly List<RevenueSummary> _revenueSummaries = new List<RevenueSummary>();
        private static readonly List<PlatformRevenue> _platformRevenues = new List<PlatformRevenue>();
        private static readonly List<DailyRevenue> _dailyRevenues = new List<DailyRevenue>();
        private static readonly List<MonetizationMetricsDto> _monetizationMetrics = new List<MonetizationMetricsDto>();
        private static readonly List<Guid> _connectedCreators = new List<Guid>();

        public TestRevenueController(ILogger<TestRevenueController> logger)
        {
            _logger = logger;
            
            // Inicializar dados de exemplo se as listas estiverem vazias
            if (!_revenueSummaries.Any() || !_platformRevenues.Any() || !_dailyRevenues.Any() || !_monetizationMetrics.Any())
            {
                InitializeTestData();
            }
        }

        [HttpPost("connect-adsense/{creatorId}")]
        public async Task<IActionResult> ConnectAdSense(
            Guid creatorId,
            [FromBody] ConnectAdSenseRequest request)
        {
            _logger.LogInformation("TestRevenueController.ConnectAdSense: Conectando AdSense para criador {CreatorId}", creatorId);
            
            // Verificação do token de autenticação
            var authHeader = Request.Headers["Authorization"].ToString();
            if (authHeader.Contains("invalid-token"))
            {
                return StatusCode(403);
            }
            
            if (creatorId == Guid.Empty || string.IsNullOrEmpty(request.Email))
            {
                return BadRequest(new { message = "Parâmetros inválidos" });
            }
            
            // Simular URL de autorização
            var authUrl = $"https://accounts.google.com/o/oauth2/auth?client_id=test&redirect_uri=test&scope=adsense&response_type=code&state={creatorId}";
            
            return Ok(new { authorizationUrl = authUrl });
        }

        [HttpGet("adsense-callback")]
        public async Task<IActionResult> AdSenseCallback(
            [FromQuery] Guid creatorId,
            [FromQuery] string code)
        {
            _logger.LogInformation("TestRevenueController.AdSenseCallback: Callback do AdSense para criador {CreatorId}", creatorId);
            
            if (creatorId == Guid.Empty || string.IsNullOrEmpty(code))
            {
                return BadRequest(new { message = "Parâmetros inválidos" });
            }
            
            // Adicionar criador à lista de conectados
            if (!_connectedCreators.Contains(creatorId))
            {
                _connectedCreators.Add(creatorId);
            }
            
            // Simular redirecionamento
            return Redirect($"https://seuapp.com/revenue/adsense-connected");
        }

        [HttpGet("revenue/{creatorId}")]
        public async Task<ActionResult<RevenueSummary>> GetRevenueSummary(
            Guid creatorId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            _logger.LogInformation("TestRevenueController.GetRevenueSummary: Buscando resumo de receita para criador {CreatorId}", creatorId);
            
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
            
            // Verificar se o criador está conectado
            if (creatorId == Guid.Parse("00000000-0000-0000-0000-000000000001")) // ID especial para teste
            {
                return NotFound(new { message = "Conta AdSense não conectada" });
            }
            
            // Buscar ou criar resumo de receita
            var summary = _revenueSummaries.FirstOrDefault(r => 
                r.CreatorId == creatorId && 
                r.StartDate == startDate.Date && 
                r.EndDate == endDate.Date);
                
            if (summary == null)
            {
                summary = CreateRevenueSummary(creatorId, startDate, endDate);
                _revenueSummaries.Add(summary);
            }
            
            return Ok(summary);
        }

        [HttpGet("revenue/{creatorId}/by-platform")]
        public async Task<ActionResult<List<PlatformRevenue>>> GetRevenueByPlatform(
            Guid creatorId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            _logger.LogInformation("TestRevenueController.GetRevenueByPlatform: Buscando receita por plataforma para criador {CreatorId}", creatorId);
            
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
            
            // Verificar se o criador está conectado
            if (creatorId == Guid.Parse("00000000-0000-0000-0000-000000000001")) // ID especial para teste
            {
                return NotFound(new { message = "Conta AdSense não conectada" });
            }
            
            // Filtrar ou criar receitas por plataforma
            var revenues = _platformRevenues.Where(r => 
                r.CreatorId == creatorId && 
                r.Date >= startDate.Date && 
                r.Date <= endDate.Date).ToList();
                
            if (!revenues.Any())
            {
                revenues = CreatePlatformRevenues(creatorId, startDate, endDate);
                foreach (var revenue in revenues)
                {
                    _platformRevenues.Add(revenue);
                }
            }
            
            return Ok(revenues);
        }

        [HttpGet("revenue/{creatorId}/by-day")]
        public async Task<ActionResult<List<DailyRevenue>>> GetRevenueByDay(
            Guid creatorId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            _logger.LogInformation("TestRevenueController.GetRevenueByDay: Buscando receita por dia para criador {CreatorId}", creatorId);
            
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
            
            // Verificar se o criador está conectado
            if (creatorId == Guid.Parse("00000000-0000-0000-0000-000000000001")) // ID especial para teste
            {
                return NotFound(new { message = "Conta AdSense não conectada" });
            }
            
            // Filtrar ou criar receitas diárias
            var revenues = _dailyRevenues.Where(r => 
                r.CreatorId == creatorId && 
                r.Date >= startDate.Date && 
                r.Date <= endDate.Date).ToList();
                
            if (!revenues.Any())
            {
                revenues = CreateDailyRevenues(creatorId, startDate, endDate);
                foreach (var revenue in revenues)
                {
                    _dailyRevenues.Add(revenue);
                }
            }
            
            return Ok(revenues);
        }

        [HttpGet("monetization/{creatorId}")]
        public async Task<ActionResult<MonetizationMetricsDto>> GetMonetizationMetrics(
            Guid creatorId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            _logger.LogInformation("TestRevenueController.GetMonetizationMetrics: Buscando métricas de monetização para criador {CreatorId}", creatorId);
            
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
            
            // Verificar se o criador está conectado
            if (creatorId == Guid.Parse("00000000-0000-0000-0000-000000000001")) // ID especial para teste
            {
                return NotFound(new { message = "Conta AdSense não conectada" });
            }
            
            // Buscar ou criar métricas de monetização
            var metrics = _monetizationMetrics.FirstOrDefault(m => 
                m.CreatorId == creatorId && 
                m.PeriodStart == startDate.Date && 
                m.PeriodEnd == endDate.Date);
                
            if (metrics == null)
            {
                metrics = CreateMonetizationMetrics(creatorId, startDate, endDate);
                _monetizationMetrics.Add(metrics);
            }
            
            return Ok(metrics);
        }

        [HttpPost("adsense/refresh/{creatorId}")]
        public async Task<IActionResult> RefreshAdSenseData(Guid creatorId)
        {
            _logger.LogInformation("TestRevenueController.RefreshAdSenseData: Atualizando dados do AdSense para criador {CreatorId}", creatorId);
            
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
            
            // Verificar se o criador está conectado
            if (creatorId == Guid.Parse("00000000-0000-0000-0000-000000000001")) // ID especial para teste
            {
                return NotFound(new { message = "Conta AdSense não conectada" });
            }
            
            return Ok(new { message = "Dados do AdSense atualizados com sucesso" });
        }

        private void InitializeTestData()
        {
            var creatorId = Guid.NewGuid();
            var startDate = DateTime.Now.AddDays(-30);
            var endDate = DateTime.Now;
            
            // Adicionar criador à lista de conectados
            _connectedCreators.Add(creatorId);
            
            // Criar dados de exemplo
            _revenueSummaries.Add(CreateRevenueSummary(creatorId, startDate, endDate));
            
            var platformRevenues = CreatePlatformRevenues(creatorId, startDate, endDate);
            foreach (var revenue in platformRevenues)
            {
                _platformRevenues.Add(revenue);
            }
            
            var dailyRevenues = CreateDailyRevenues(creatorId, startDate, endDate);
            foreach (var revenue in dailyRevenues)
            {
                _dailyRevenues.Add(revenue);
            }
            
            _monetizationMetrics.Add(CreateMonetizationMetrics(creatorId, startDate, endDate));
        }

        private RevenueSummary CreateRevenueSummary(Guid creatorId, DateTime startDate, DateTime endDate)
        {
            var random = new Random();
            
            return new RevenueSummary
            {
                CreatorId = creatorId,
                StartDate = startDate.Date,
                EndDate = endDate.Date,
                TotalRevenue = (decimal)Math.Round(random.NextDouble() * 1000, 2),
                AdSenseRevenue = (decimal)Math.Round(random.NextDouble() * 800, 2),
                AffiliateRevenue = (decimal)Math.Round(random.NextDouble() * 200, 2),
                SponsoredContent = (decimal)Math.Round(random.NextDouble() * 300, 2),
                OtherRevenue = (decimal)Math.Round(random.NextDouble() * 100, 2),
                ComparisonWithLastPeriod = (decimal)Math.Round(random.NextDouble() * 20 - 5, 2),
                ProjectedRevenue = (decimal)Math.Round(random.NextDouble() * 1500, 2)
            };
        }

        private List<PlatformRevenue> CreatePlatformRevenues(Guid creatorId, DateTime startDate, DateTime endDate)
        {
            var random = new Random();
            var result = new List<PlatformRevenue>();
            
            foreach (SocialMediaPlatform platform in Enum.GetValues(typeof(SocialMediaPlatform)))
            {
                result.Add(new PlatformRevenue
                {
                    CreatorId = creatorId,
                    Platform = platform,
                    Date = DateTime.Now.Date,
                    TotalRevenue = (decimal)Math.Round(random.NextDouble() * 500, 2),
                    ViewsCount = random.Next(1000, 100000),
                    EngagementRate = (decimal)Math.Round(random.NextDouble() * 10, 2),
                    RevenuePerView = (decimal)Math.Round(random.NextDouble() * 0.01, 4),
                    AdImpressions = random.Next(500, 50000),
                    ClickThroughRate = (decimal)Math.Round(random.NextDouble() * 5, 2)
                });
            }
            
            return result;
        }

        private List<DailyRevenue> CreateDailyRevenues(Guid creatorId, DateTime startDate, DateTime endDate)
        {
            var random = new Random();
            var result = new List<DailyRevenue>();
            
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                result.Add(new DailyRevenue
                {
                    CreatorId = creatorId,
                    Date = date,
                    TotalRevenue = (decimal)Math.Round(random.NextDouble() * 100, 2),
                    AdRevenue = (decimal)Math.Round(random.NextDouble() * 70, 2),
                    AffiliateRevenue = (decimal)Math.Round(random.NextDouble() * 20, 2),
                    SponsoredRevenue = (decimal)Math.Round(random.NextDouble() * 30, 2),
                    ContentCount = random.Next(1, 5),
                    ViewsCount = random.Next(100, 10000)
                });
            }
            
            return result;
        }

        private MonetizationMetricsDto CreateMonetizationMetrics(Guid creatorId, DateTime startDate, DateTime endDate)
        {
            var random = new Random();
            
            return new MonetizationMetricsDto
            {
                CreatorId = creatorId,
                PeriodStart = startDate.Date,
                PeriodEnd = endDate.Date,
                RevenuePerFollower = (decimal)Math.Round(random.NextDouble() * 0.05, 4),
                RevenuePerPost = (decimal)Math.Round(random.NextDouble() * 50, 2),
                RevenuePerView = (decimal)Math.Round(random.NextDouble() * 0.01, 4),
                EngagementToRevenueRatio = (decimal)Math.Round(random.NextDouble() * 2, 2),
                BestPerformingContent = new List<RevenueContentPerformance>
                {
                    new RevenueContentPerformance
                    {
                        ContentId = Guid.NewGuid().ToString(),
                        Title = "Melhor vídeo tutorial",
                        Revenue = (decimal)Math.Round(random.NextDouble() * 200, 2),
                        Views = random.Next(5000, 50000),
                        EngagementRate = (decimal)Math.Round(random.NextDouble() * 15, 2),
                        Platform = SocialMediaPlatform.YouTube
                    },
                    new RevenueContentPerformance
                    {
                        ContentId = Guid.NewGuid().ToString(),
                        Title = "Post sobre tendências",
                        Revenue = (decimal)Math.Round(random.NextDouble() * 150, 2),
                        Views = random.Next(3000, 30000),
                        EngagementRate = (decimal)Math.Round(random.NextDouble() * 12, 2),
                        Platform = SocialMediaPlatform.Instagram
                    }
                },
                RevenueByPlatform = new Dictionary<SocialMediaPlatform, decimal>
                {
                    { SocialMediaPlatform.Instagram, (decimal)Math.Round(random.NextDouble() * 300, 2) },
                    { SocialMediaPlatform.YouTube, (decimal)Math.Round(random.NextDouble() * 500, 2) },
                    { SocialMediaPlatform.TikTok, (decimal)Math.Round(random.NextDouble() * 200, 2) }
                },
                MonetizationSuggestions = new List<string>
                {
                    "Aumentar frequência de postagens no YouTube",
                    "Explorar oportunidades de patrocínio",
                    "Otimizar conteúdo para aumentar CPM"
                }
            };
        }
    }

    public class ConnectAdSenseRequest
    {
        public string Email { get; set; }
    }

    public class RevenueSummary
    {
        public Guid CreatorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AdSenseRevenue { get; set; }
        public decimal AffiliateRevenue { get; set; }
        public decimal SponsoredContent { get; set; }
        public decimal OtherRevenue { get; set; }
        public decimal ComparisonWithLastPeriod { get; set; }
        public decimal ProjectedRevenue { get; set; }
    }

    public class PlatformRevenue
    {
        public Guid CreatorId { get; set; }
        public SocialMediaPlatform Platform { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ViewsCount { get; set; }
        public decimal EngagementRate { get; set; }
        public decimal RevenuePerView { get; set; }
        public int AdImpressions { get; set; }
        public decimal ClickThroughRate { get; set; }
    }

    public class DailyRevenue
    {
        public Guid CreatorId { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AdRevenue { get; set; }
        public decimal AffiliateRevenue { get; set; }
        public decimal SponsoredRevenue { get; set; }
        public int ContentCount { get; set; }
        public int ViewsCount { get; set; }
    }

    public class MonetizationMetricsDto
    {
        public Guid CreatorId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal RevenuePerFollower { get; set; }
        public decimal RevenuePerPost { get; set; }
        public decimal RevenuePerView { get; set; }
        public decimal EngagementToRevenueRatio { get; set; }
        public List<RevenueContentPerformance> BestPerformingContent { get; set; }
        public Dictionary<SocialMediaPlatform, decimal> RevenueByPlatform { get; set; }
        public List<string> MonetizationSuggestions { get; set; }
    }

    public class RevenueContentPerformance
    {
        public string ContentId { get; set; }
        public string Title { get; set; }
        public decimal Revenue { get; set; }
        public int Views { get; set; }
        public decimal EngagementRate { get; set; }
        public SocialMediaPlatform Platform { get; set; }
    }
} 