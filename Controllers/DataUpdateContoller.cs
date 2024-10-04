using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TFT_API.Services;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataUpdateContoller(DataCheckService dataCheckService) : ControllerBase
    {
        private readonly DataCheckService _dataCheckService = dataCheckService;

        [Authorize]
        [HttpPost("trigger")]
        public async Task<IActionResult> TriggerDataCheck(CancellationToken cancellationToken)
        {
            await _dataCheckService.RunCheckAsync(cancellationToken);
            return Ok("Data check triggered successfully.");
        }
    }
}
