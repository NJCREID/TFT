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
