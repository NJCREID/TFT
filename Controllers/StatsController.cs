using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TFT_API.Interfaces;
using TFT_API.Models.Item;
using TFT_API.Models.Stats.AugmentStats;
using TFT_API.Models.Stats.CompStats;
using TFT_API.Models.Stats.ItemStats;
using TFT_API.Models.Stats.TraitStats;
using TFT_API.Models.Stats.UnitStats;
using TFT_API.Services;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatsController : Controller
    {
        private readonly IStatDataAccess _statDataAccess;
        private readonly IMapper _mapper;
        private readonly CoocurrenceStats _coocurrenceStats;

        public StatsController(IStatDataAccess statDataAccess, IMapper mapper, CoocurrenceStats coocurrenceStats) { 
            _statDataAccess = statDataAccess;
            _mapper = mapper;
            _coocurrenceStats = coocurrenceStats;
        }

        [HttpGet("calculate")]
        public async Task<IActionResult> GetStatistics([FromQuery] string[] constraints)
        {
            var result = await _coocurrenceStats.CalculateStatisticsAsync(constraints);
            return Ok(result);
        }

        [HttpGet("unit")]
        public ActionResult<BaseUnitStatDto> GetUnitStats()
        {
            var unitStats = _mapper.Map<BaseUnitStatDto>(_statDataAccess.GetUnitStats());
            return Ok(unitStats);
        }

        [HttpGet("item")]
        public ActionResult<BaseItemStatDto> GetItemStats()
        {
            var itemStats = _statDataAccess.GetItemStats();
            return Ok(_mapper.Map<BaseItemStatDto>(itemStats));
        }

        [HttpGet("trait")]
        public ActionResult<BaseTraitStatDto> GetTraitStats()
        {
            var traitStats = _statDataAccess.GetTraitStats();
            return Ok(_mapper.Map<BaseTraitStatDto>(traitStats));
        }

        [HttpGet("augment")]
        public ActionResult<BaseAugmentStatDto> GetAugmentStats()
        {
            var augmentStats = _statDataAccess.GetAugmentStats();
            return Ok(_mapper.Map<BaseAugmentStatDto>(augmentStats));
        }

        [HttpGet("comp")]
        public ActionResult<BaseCompStatDto> GetCompStats()
        {
            var augmentStats = _statDataAccess.GetCompStats();
            return Ok(_mapper.Map<BaseCompStatDto>(augmentStats));
        }
    }
}
