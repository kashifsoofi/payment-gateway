namespace AcquiringBank.Api.Controllers
{
    using AcquiringBank.Api.Responses;
    using Microsoft.AspNetCore.Mvc;

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
