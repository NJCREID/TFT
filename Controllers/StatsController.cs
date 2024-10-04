using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using TFT_API.Interfaces;
using TFT_API.Models.Stats.AugmentStats;
using TFT_API.Models.Stats.CompStats;
using TFT_API.Models.Stats.CoOccurrence;
using TFT_API.Models.Stats.ItemStats;
using TFT_API.Models.Stats.TraitStats;
using TFT_API.Models.Stats.UnitStats;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController(IStatDataAccess statDataAccess,  IMemoryCache memoryCache) : ControllerBase
    {
        private readonly IStatDataAccess _statDataAccess = statDataAccess;
        private readonly IMemoryCache _memoryCache = memoryCache;

        private static readonly HashSet<string> ValidLeagues = ["", "Challenger", "Master", "GrandMaster"];
        private static bool IsValidLeague(string league) => ValidLeagues.Contains(league);

        [AllowAnonymous]
        [HttpGet("unit/{league?}")]
        public async Task<ActionResult<BaseUnitStatDto>> GetUnitStats([FromRoute] string league = "")
        {
            if (!IsValidLeague(league))
            {
                return BadRequest("Invalid league value. Valid options are '', 'Challenger', 'Master', or 'GrandMaster'.");
            }
            var cacheKey = $"unitStat_{league}";
            if (!_memoryCache.TryGetValue(cacheKey, out var cachedUnitStats))
            {
                cachedUnitStats = await _statDataAccess.GetUnitStatsAsync(league);
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                };
                _memoryCache.Set(cacheKey, cachedUnitStats, cacheEntryOptions);
            }
            if (cachedUnitStats == null)
            {
                return NotFound($"No unit stats found for league: {league}.");
            }
            return Ok(cachedUnitStats);
        }

        [AllowAnonymous]
        [HttpGet("item/{league?}")]
        public async Task<ActionResult<BaseItemStatDto>> GetItemStats([FromRoute] string league = "")
        {
            if (!IsValidLeague(league))
            {
                return BadRequest("Invalid league value. Valid options are '', 'Challenger', 'Master', or 'GrandMaster'.");
            }
            var cacheKey = $"itemStat_{league}";
            if (!_memoryCache.TryGetValue(cacheKey, out var cachedItemStats))
            {
                cachedItemStats = await _statDataAccess.GetItemStatsAsync(league);
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                };
                _memoryCache.Set(cacheKey, cachedItemStats, cacheEntryOptions);
            }
            if (cachedItemStats == null)
            {
                return NotFound($"No item stats found for league: {league}.");
            }
            return Ok(cachedItemStats);
        }

        [AllowAnonymous]
        [HttpGet("trait/{league?}")]
        public async Task<ActionResult<BaseTraitStatDto>> GetTraitStats([FromRoute] string league = "")
        {
            if (!IsValidLeague(league))
            {
                return BadRequest("Invalid league value. Valid options are '', 'Challenger', 'Master', or 'GrandMaster'.");
            }
            var cacheKey = $"traitStat_{league}";
            if (!_memoryCache.TryGetValue(cacheKey, out var cachedTraitStats))
            {
                cachedTraitStats = await _statDataAccess.GetTraitStatsAsync(league);
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                };
                _memoryCache.Set(cacheKey, cachedTraitStats, cacheEntryOptions);
            }
            if (cachedTraitStats == null)
            {
                return NotFound($"No trait stats found for league: {league}.");
            }
            return Ok(cachedTraitStats);
        }

        [AllowAnonymous]
        [HttpGet("augment/{league?}")]
        public async Task<ActionResult<BaseAugmentStatDto>> GetAugmentStats([FromRoute] string league = "")
        {
            if (!IsValidLeague(league))
            {
                return BadRequest("Invalid league value. Valid options are '', 'Challenger', 'Master', or 'GrandMaster'.");
            }
            var cacheKey = $"augmentStat_{league}";
            if (!_memoryCache.TryGetValue(cacheKey, out var cachedAugmentStats))
            {
                cachedAugmentStats = await _statDataAccess.GetAugmentStatsAsync(league);
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                };
                _memoryCache.Set(cacheKey, cachedAugmentStats, cacheEntryOptions);
            }
            if (cachedAugmentStats == null)
            {
                return NotFound($"No augment stats found for league: {league}.");
            }
            return Ok(cachedAugmentStats);
        }

        [AllowAnonymous]
        [HttpGet("comp/{league?}")]
        public async Task<ActionResult<BaseCompStatDto>> GetCompStats([FromRoute] string league = "")
        {
            if (!IsValidLeague(league))
            {
                return BadRequest("Invalid league value. Valid options are '', 'Challenger', 'Master', or 'GrandMaster'.");
            }
            var cacheKey = $"compStat_{league}";
            if (!_memoryCache.TryGetValue(cacheKey, out var cachedCompStats))
            {
                cachedCompStats = await _statDataAccess.GetCompStatsAsync(league);
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                };
                _memoryCache.Set(cacheKey, cachedCompStats, cacheEntryOptions);
            }
            if (cachedCompStats == null)
            {
                return NotFound($"No comp stats found for league: {league}.");
            }
            return Ok(cachedCompStats);
        }

        [AllowAnonymous]
        [HttpGet("cooccurrence")]
        public async Task<ActionResult<BaseCoOccurrenceDto>> GetCoOccurrenceStats([FromQuery][Required] string type, [FromQuery][Required] string key)
        {
            if (string.IsNullOrWhiteSpace(type) || string.IsNullOrWhiteSpace(key))
            {
                return BadRequest("Type and key parameters are required.");
            }
            var baseCoOccurrence = await _statDataAccess.GetCoOccurrenceStatsAsync(type, key);
            if (baseCoOccurrence == null || baseCoOccurrence.CoOccurrences.Count <= 0)
            {
                return NotFound($"No co-occurrences found for the combination: {type}");
            }
            return Ok(baseCoOccurrence);
        }
    }
}
