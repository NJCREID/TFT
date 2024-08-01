using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TFT_API.Interfaces;
using TFT_API.Models.Unit;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UnitController : ControllerBase
    {
        private readonly IUnitDataAccess _unitRepo;
        private readonly IMapper _mapper;

        public UnitController(IUnitDataAccess unitRepo, IMapper mapper)
        {
            _unitRepo = unitRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<List<PartialUnitDto>> GetUnits()
        {
            var units = _unitRepo.GetUnits();
            return Ok(_mapper.Map<List<PartialUnitDto>>(units));
        }

        [HttpGet("{key}", Name = "GetUnit")]
        public ActionResult<UnitDto> GetUnitByKey(string key)
        {
            var unit = _unitRepo.GetUnitByKey(key);
            if (unit == null) return NotFound();
            return Ok(_mapper.Map<UnitDto>(unit));
        }

        [HttpPost]
        public ActionResult<UnitDto> AddUnit(UnitRequest request)
        {
            var currentUnits = _unitRepo.GetUnits();
            if (request == null) return BadRequest();
            if (currentUnits.Any(c => c.Key == request.Key))
                return Conflict("A unit with that key already exists.");
            var newUnit = _mapper.Map<PersistedUnit>(request);
            var result = _unitRepo.AddUnit(newUnit);
            return CreatedAtRoute("GetUnit", new { key = result.Key }, result);
        }
    }
}
