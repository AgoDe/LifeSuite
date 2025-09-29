using Microsoft.AspNetCore.Mvc;

namespace BudgetManager.Api.Controllers
{
    [Route("api/health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        /// <summary>
        /// Health check endpoint
        /// </summary>
        /// <returns>Service health status</returns>
        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "healthy",
                service = "Budget Manager API",
                version = "1.0.0",
                timestamp = DateTime.UtcNow.ToString("O")
            });
        }
    }
}