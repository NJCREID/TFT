using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TFT_API.Interfaces;
using TFT_API.Models.Item;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : Controller
    {
        private readonly IItemDataAccess _itemRepo;
        private readonly IMapper _mapper;
        public ItemController(IItemDataAccess itemRepo, IMapper mapper)
        {
            _itemRepo = itemRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<List<ItemDto>> GetItems()
        {
            var items = _itemRepo.GetItems();
            return Ok(_mapper.Map<List<ItemDto>>(items));
        }

        [HttpGet("components")]
        public ActionResult<List<ItemDto>>  GetComponents()
        {
            var items = _itemRepo.GetComponents();
            return Ok(_mapper.Map<List<ItemDto>>(items));
        }

        [HttpGet("{key}", Name = "GetItem")]
        public ActionResult<ItemDto> GetItemByKey(string key)
        {
            var item = _itemRepo.GetItemByKey(key);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<ItemDto>(item));
        }

        [HttpPost]
        public ActionResult<ItemDto> AddItem(PersistedItem request)
        {
            var currentItems = _itemRepo.GetItems();
            if (request == null) return BadRequest();
            if (currentItems.Any(c => c.Key == request.Key))
                return Conflict("A item with that key already exists.");
            var result = _itemRepo.AddItem(request);
            return CreatedAtRoute("GetItem", new { key = result.Key }, result);
        }
    }
}
