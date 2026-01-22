using Microsoft.AspNetCore.Mvc;

namespace HK.ServerReact.Server.Controllers
{
    [ApiController]
    [Route("api")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { msg = "Hello" });
        }
    }
}
