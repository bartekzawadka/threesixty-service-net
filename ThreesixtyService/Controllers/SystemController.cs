using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ThreesixtyService.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SystemController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok();
        }

        public SystemController(IConfiguration configuration) : base(configuration)
        {
        }
    }
}