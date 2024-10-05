using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using TFT_API.Interfaces;
using TFT_API.Models.Augments;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AugmentController(IAugmentDataAccess augmentRepo, IMemoryCache memoryCache) : ControllerBase
    {
        private readonly IAugmentDataAccess _augmentRepo = augmentRepo;
        private readonly IMemoryCache _memoryCache = memoryCache;


        /// <summary>
        /// Retrieves a list of all augments.
        /// </summary>
        /// <returns>A list of augments</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/augments
        ///
        /// </remarks>
        /// <response code="200">Returns a list of augments</response>
        /// <response code="404">If no augments are found</response>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<AugmentDto>>> GetAugments()
        {
            var cacheKey = "augments";
            if(!_memoryCache.TryGetValue(cacheKey, out List<AugmentDto>? cachedAugments))
            {
                cachedAugments = await _augmentRepo.GetAugmentsAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                };
                _memoryCache.Set(cacheKey, cachedAugments, cacheEntryOptions);
            }
            if (cachedAugments == null || cachedAugments.Count == 0)
            {
                return NotFound("No augmnets found.");
            }
            return Ok(cachedAugments);
        }

        /// <summary>
        /// Retrieves a list of augments by their tier.
        /// </summary>
        /// <param name="tier">The tier of the augments (1 to 3)</param>
        /// <returns>A list of augments for the specified tier</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/augments/tier/1
        ///
        /// </remarks>
        /// <response code="200">Returns a list of augments for the specified tier</response>
        /// <response code="404">If no augments are found for the specified tier</response>
        [AllowAnonymous]
        [HttpGet("tier/{tier}")]
        public async Task<ActionResult<List<AugmentDto>>> GetAugmentByTier([FromRoute, Range(1, 3, ErrorMessage = "Tier must be between 1 and 3")] int tier)
        {
            var augments = await _augmentRepo.GetAugmentsByTierAsync(tier);
            if (augments == null || augments.Count == 0)
            {
                return NotFound($"No augments found for tier {tier}.");
            }
            return Ok(augments);
        }

        /// <summary>
        /// Retrieves an augment by its key.
        /// </summary>
        /// <param name="key">The key of the augment</param>
        /// <returns>The augment with the specified key</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/augments/{key}
        ///
        /// </remarks>
        /// <response code="200">Returns the augment with the specified key</response>
        /// <response code="404">If the specified augment key is not found</response>
        [AllowAnonymous]
        [HttpGet("{key}", Name = "GetAugment")]
        public async Task<ActionResult<AugmentDto>> GetAugmentByKey([FromRoute, Required(ErrorMessage = "Key is required")] string key)
        {
            var augments = await _augmentRepo.GetAugmentByKeyAsync(key);
            if (augments == null) return NotFound($"Augment with key '{key}' not found.");
            return Ok(augments);
        }
    }
}
