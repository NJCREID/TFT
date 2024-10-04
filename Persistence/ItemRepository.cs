using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Item;

namespace TFT_API.Persistence
{
    public class ItemRepository(TFTContext context) : IItemDataAccess
    {
        private readonly TFTContext _context = context;

        public async Task<ItemDto?> GetItemByKeyAsync(string key)
        {
            return await ProjectToItemDto(_context.Items
                .Where(t => t.InGameKey == key))
                .FirstOrDefaultAsync();
        }

        public async Task<List<ItemDto>> GetFullItemsAsync()
        {
            return await ProjectToItemDto(_context.Items
                .Where(a => a.IsHidden != true))
                .ToListAsync();
        }

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

        public async Task<List<ItemDto>> GetComponentsAsync()
        {
            return await ProjectToItemDto(_context.Items
                .Where(item => item.IsComponent == true && item.IsHidden != true))
                .ToListAsync();
        }

        private static IQueryable<ItemDto> ProjectToItemDto(IQueryable<PersistedItem> query)
        {
            return query.Select(i => new ItemDto
            {
                InGameKey = i.InGameKey,
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
