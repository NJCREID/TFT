using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TFT_API.Interfaces;
using TFT_API.Models.Augments;
using TFT_API.Models.Item;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AugmentController : Controller
    {
        private readonly IAugmentDataAccess _augmentRepo;
        private readonly IMapper _mapper;
        public AugmentController(IAugmentDataAccess augmentRepo, IMapper mapper)
        {
            _augmentRepo = augmentRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<List<AugmentDto>> GetAugments()
        {
            var items = _augmentRepo.GetAugments();
            return Ok(_mapper.Map<List<AugmentDto>>(items));
        }

        [HttpGet("tier/{tier}")]
        public ActionResult<List<AugmentDto>> GetAugmentByTier(int tier)
        {
            var items = _augmentRepo.GetAugmentsByTier(tier);
            return Ok(_mapper.Map<List<AugmentDto>>(items));
        }

        [HttpGet("{key}", Name = "GetAugment")]
        public ActionResult<AugmentDto> GetAugmentByKey(string key)
        {
            var item = _augmentRepo.GetAugmentByKey(key);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<AugmentDto>(item));
        }

        [HttpPost]
        public ActionResult<AugmentDto> AddAugment(PersistedAugment request)
        {
            var currentItems = _augmentRepo.GetAugments();
            if (request == null) return BadRequest();
            if (currentItems.Any(c => c.Key == request.Key))
                return Conflict("A item with that key already exists.");
            var result = _augmentRepo.AddAugment(request);
            return CreatedAtRoute("GetAugment", new { key = result.Key }, result);
        }
    }
}
