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

        /// <summary>
        /// Triggers a data check process for updating.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>An indication of whether the data check was triggered successfully</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// POST /api/dataupdate/trigger
        /// 
        /// </remarks>
        /// <response code="200">Returns a success message indicating that the data check was triggered</response>
        [Authorize]
        [HttpPost("trigger")]
        public async Task<IActionResult> TriggerDataCheck(CancellationToken cancellationToken)
        {
            await _dataCheckService.RunCheckAsync(cancellationToken);
            return Ok("Data check triggered successfully.");
        }
    }
}
