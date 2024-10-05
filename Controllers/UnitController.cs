using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using TFT_API.Interfaces;
using TFT_API.Models.Unit;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitController(IUnitDataAccess unitRepo, IMemoryCache memoryCache) : ControllerBase
    {
        private readonly IUnitDataAccess _unitRepo = unitRepo;
        private readonly IMemoryCache _memoryCache = memoryCache;

        /// <summary>
        /// Retrieves a list of all units with full details.
        /// </summary>
        /// <returns>A list of unit DTOs.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/unit/full
        ///
        /// </remarks>
        /// <response code="200">Returns a list of all units</response>
        /// <response code="404">If no units are found</response>
        [AllowAnonymous]
        [HttpGet("full")]
        public async Task<ActionResult<List<UnitDto>>> GetUnits()
        {
            var cacheKey = $"units_full";
            if(!_memoryCache.TryGetValue(cacheKey, out List<UnitDto>? cachedUnits))
            {
                cachedUnits = await _unitRepo.GetFullUnitsAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                };
                _memoryCache.Set(cacheKey, cachedUnits, cacheEntryOptions);
            }
            if (cachedUnits == null || cachedUnits.Count == 0)
            {
                return NotFound("No Units Found.");
            }
            return Ok(cachedUnits);
        }

        /// <summary>
        /// Retrieves a list of all units with partial details.
        /// </summary>
        /// <returns>A list of partial unit DTOs.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/unit/partial
        ///
        /// </remarks>
        /// <response code="200">Returns a list of all units</response>
        /// <response code="404">If no units are found</response>
        [AllowAnonymous]
        [HttpGet("partial")]
        public async Task<ActionResult<List<PartialUnitDto>>> GetPartialUnits()
        {
            var cacheKey = $"units_partial";
            if (!_memoryCache.TryGetValue(cacheKey, out List<PartialUnitDto>? cachedUnits))
            {
                cachedUnits = await _unitRepo.GetPartialUnitsAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                };
                _memoryCache.Set(cacheKey, cachedUnits, cacheEntryOptions);
            }
            if (cachedUnits == null || cachedUnits.Count == 0)
            {
                return NotFound("No Units Found.");
            }
            return Ok(cachedUnits);

        }

        /// <summary>
        /// Retrieves a unit by its key.
        /// </summary>
        /// <param name="key">The key of the unit to retrieve.</param>
        /// <returns>The unit DTO for the specified key.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/unit/unitKey
        ///
        /// </remarks>
        /// <response code="200">Returns the unit with the specified key</response>
        /// <response code="404">If the specified unit key is not found</response>
        [AllowAnonymous]
        [HttpGet("{key}", Name = "GetUnit")]
        public async Task<ActionResult<UnitDto>> GetUnitByKey(
             [FromRoute, Required, MinLength(1, ErrorMessage = "Key cannot be empty")] string key)
        {
            var unit = await _unitRepo.GetUnitByKeyAsync(key);
            if (unit == null) return NotFound($"Unit with key '{key}' not found.");
            return Ok(unit);
        }
    }
}
