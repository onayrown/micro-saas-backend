using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MicroSaaS.IntegrationTests.Controllers
{
    [ApiController]
    [Route("api/health")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<string> Get()
        {
            _logger.LogInformation("Health check executado com sucesso");
            return Ok("Serviço funcionando corretamente");
        }
    }
} 