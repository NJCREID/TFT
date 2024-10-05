using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TFT_API.Interfaces;
using TFT_API.Models.Item;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController(IItemDataAccess itemRepo, IMemoryCache memoryCache) : ControllerBase
    {
        private readonly IItemDataAccess _itemRepo = itemRepo;
        private readonly IMemoryCache _memoryCache = memoryCache;

        /// <summary>
        /// Retrieves a complete list of items.
        /// </summary>
        /// <returns>A list of complete item DTOs.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/items/full
        ///
        /// </remarks>
        /// <response code="200">Returns a list of complete items</response>
        /// <response code="404">If no items are found</response>
        [AllowAnonymous]
        [HttpGet("full")]
        public async Task<ActionResult<List<ItemDto>>> GetFullItems()
        {
            var cacheKey = "items_full";
            if(!_memoryCache.TryGetValue(cacheKey, out List<ItemDto>? cachedItems))
            {
                cachedItems = await _itemRepo.GetFullItemsAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                };
                _memoryCache.Set(cacheKey, cachedItems, cacheEntryOptions);
            }
            if(cachedItems == null || cachedItems.Count == 0)
            {
                return NotFound("No items found.");
            }
            return Ok(cachedItems);
        }

        /// <summary>
        /// Retrieves a partial list of items.
        /// </summary>
        /// <returns>A list of partial item DTOs.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/items/partial
        ///
        /// </remarks>
        /// <response code="200">Returns a list of partial items</response>
        /// <response code="404">If no items are found</response>
        [AllowAnonymous]
        [HttpGet("partial")]
        public async Task<ActionResult<List<PartialItemDto>>> GetPartialItems()
        {
            var cacheKey = "items_partial";
            if (!_memoryCache.TryGetValue(cacheKey, out List<PartialItemDto>? cachedItems))
            {
                cachedItems = await _itemRepo.GetPartialItemsAsync();
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30)
                };
                _memoryCache.Set(cacheKey, cachedItems, cacheEntryOptions);
            }
            if (cachedItems == null || cachedItems.Count == 0)
            {
                return NotFound("No items found.");
            }
            return Ok(cachedItems);
        }

        /// <summary>
        /// Retrieves a list of item components.
        /// </summary>
        /// <returns>A list of item DTOs representing components.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/items/components
        ///
        /// </remarks>
        /// <response code="200">Returns a list of components</response>
        /// <response code="404">If no components are found</response>
        [AllowAnonymous]
        [HttpGet("components")]
        public async Task<ActionResult<List<ItemDto>>>  GetComponents()
        {
            var items = await _itemRepo.GetComponentsAsync();
            if (items == null || items.Count == 0) return NotFound("No components found.");
            return Ok(items);
        }

        /// <summary>
        /// Retrieves an item by its unique key.
        /// </summary>
        /// <param name="key">The unique key of the item.</param>
        /// <returns>The item DTO associated with the specified key.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/items/{key}
        ///
        /// </remarks>
        /// <response code="200">Returns the item with the specified key</response>
        /// <response code="404">If the item with the specified key is not found</response>
        [AllowAnonymous]
        [HttpGet("{key}", Name = "GetItem")]
        public async Task<ActionResult<ItemDto>> GetItemByKey(
            [FromRoute, Required, MinLength(1, ErrorMessage = "Key cannot be empty")] string key)
        {
            var item = await _itemRepo.GetItemByKeyAsync(key);
            if (item == null) NotFound($"Item with key '{key}' not found.");
            return Ok(item);
        }
    }
}
