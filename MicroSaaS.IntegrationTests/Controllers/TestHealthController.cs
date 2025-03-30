using Microsoft.AspNetCore.Mvc;

namespace MicroSaaS.IntegrationTests.Controllers
{
    [Route("api/test-health")]
    [ApiController]
    public class TestHealthController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Test Health Check OK");
        }
    }
} 