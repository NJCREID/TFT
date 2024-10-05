using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Item;

namespace TFT_API.Persistence
{
    public class ItemRepository(TFTContext context) : IItemDataAccess
    {
        private readonly TFTContext _context = context;

        // Gets an item by its in-game key, returning an ItemDto
        public async Task<ItemDto?> GetItemByKeyAsync(string key)
        {
            return await ProjectToItemDto(_context.Items
                .Where(t => t.InGameKey == key))
                .FirstOrDefaultAsync();
        }

        // Gets all non-hidden items, returning a list of ItemDto
        public async Task<List<ItemDto>> GetFullItemsAsync()
        {
            return await ProjectToItemDto(_context.Items
                .Where(a => a.IsHidden != true))
                .ToListAsync();
        }

        // Gets a partial representation of all non-hidden items
        public async Task<List<PartialItemDto>> GetPartialItemsAsync()
        {
            return await _context.Items
                .Where(i => i.IsHidden != true)
                .Select(i => new PartialItemDto
                {
                    InGameKey = i.InGameKey,
                    Name = i.Name,
                    Tags = i.Tags,
                    Recipe = i.Recipe,
                    Desc = i.Desc,  
                    ShortDesc = i.ShortDesc,
                    IsComponent = i.IsComponent,
                })
                .ToListAsync();
        }

        // Gets all components that are not hidden, returning a list of ItemDto
        public async Task<List<ItemDto>> GetComponentsAsync()
        {
            return await ProjectToItemDto(_context.Items
                .Where(item => item.IsComponent == true && item.IsHidden != true))
                .ToListAsync();
        }

        // Projects a query of PersistedItem entities to ItemDto objects
        private static IQueryable<ItemDto> ProjectToItemDto(IQueryable<PersistedItem> query)
        {
            return query.Select(i => new ItemDto
            {
                InGameKey = i.InGameKey,
                Key = i.Key,
                Name = i.Name,
                Tags = i.Tags,
                Recipe = i.Recipe,
                Desc = i.Desc,
                ShortDesc = i.ShortDesc,
                FromDesc = i.FromDesc,
                IsComponent = i.IsComponent,
                AffectedTraitKey = i.AffectedTraitKey,
                UnitCompatabilityKey = i.UnitCompatabilityKey,
                TraitCompatabilityKey = i.TraitCompatabilityKey
            });
        }
    }
}
