using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TFT_API.Interfaces;
using TFT_API.Models.Trait;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TraitController : Controller
    {
 
            private readonly ITraitDataAccess _traitRepo;
            private readonly IMapper _mapper;

            public TraitController(ITraitDataAccess traitRepo, IMapper mapper)
            {
                _traitRepo = traitRepo;
                _mapper = mapper;
            }

            [HttpGet]
            public ActionResult<List<TraitDto>> GetTraits()
            {
                var traits = _traitRepo.GetTraits();
                return Ok(_mapper.Map<List<TraitDto>>(traits));
            }

            [HttpGet("{key}", Name = "GetTrait")]
            public ActionResult<TraitDto> GetUnitByKey(string key)
            {
                var trait = _traitRepo.GetTraitByKey(key);
                if (trait == null) return NotFound();
                return Ok(_mapper.Map<TraitDto>(trait));
            }

            [HttpPost]
            public ActionResult<PersistedTrait> AddTrait(PersistedTrait request)
            {
                var currentTraits = _traitRepo.GetTraits();
                if (request == null) return BadRequest();
                if (currentTraits.Any(c => c.Key == request.Key))
                    return Conflict("A trait with that key already exists.");
                var result = _traitRepo.AddTrait(request);
                return CreatedAtRoute("GetTrait", new { key = result.Key }, result);
            }

        }
    
}
