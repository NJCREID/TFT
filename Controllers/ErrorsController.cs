using Microsoft.AspNetCore.Mvc;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ErrorsController : ControllerBase
    {
        [HttpGet] 
        public IActionResult Error()
        {
            return Problem();
        }
    }
}