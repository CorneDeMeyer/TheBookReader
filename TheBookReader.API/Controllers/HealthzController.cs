using Microsoft.AspNetCore.Mvc;

namespace TheBookReader.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthzController : Controller
    {

        [HttpGet]
        [IgnoreAntiforgeryToken]
        public IActionResult Get() => Ok();
    }
}
