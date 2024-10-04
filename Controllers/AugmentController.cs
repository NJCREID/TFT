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
