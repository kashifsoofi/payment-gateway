namespace Payments.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Payments.Contracts.Responses;

    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            var health = new Health(true);
            return Ok(health);
        }
    }
}
