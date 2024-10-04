using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using TFT_API.Interfaces;
using TFT_API.Models.Trait;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TraitController(ITraitDataAccess traitRepo, IMapper mapper, IMemoryCache memoryCache) : ControllerBase
    {
        private readonly ITraitDataAccess _traitRepo = traitRepo;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly IMapper _mapper = mapper;

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<FullTraitDto>>> GetTraits()
        {
            var cacheKey = "traits";
            if(!_memoryCache.TryGetValue(cacheKey, out List<FullTraitDto>? cachedTraits))
            {
                cachedTraits = await _traitRepo.GetTraitsAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                };
                _memoryCache.Set(cacheKey, cachedTraits, cacheEntryOptions);  
            }
            if (cachedTraits == null || cachedTraits.Count == 0)
            {
                return NotFound("No traits found.");
            }
            return Ok(cachedTraits);
        }

        [AllowAnonymous]
        [HttpGet("{key}", Name = "GetTrait")]
        public async Task<ActionResult<TraitDto>> GetTraitByKey(
            [FromRoute, Required, MinLength(1, ErrorMessage = "Key cannot be empty")] string key)
        {
            var trait = await _traitRepo.GetTraitByKeyAsync(key);
            if (trait == null) return NotFound($"Trait with key '{key}' not found.");
            var traitDto = _mapper.Map<TraitDto>(trait);
            return Ok(traitDto);
        }
    }
}
