using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Trait;

namespace TFT_API.Persistence
{
    public class TraitRepository(TFTContext context) : ITraitDataAccess
    {
        private readonly TFTContext _context = context;

        public async Task<List<FullTraitDto>> GetTraitsAsync()
        {
            return await ProjectToTraitDto(_context.Traits
                .Where(a => a.IsHidden != true))
                .ToListAsync();
        }

        public async Task<PersistedTrait?> GetTraitByKeyAsync(string key)
        {
            return await _context.Traits
                .Where(t => t.Key == key)
                .FirstOrDefaultAsync();
        }

        private static IQueryable<FullTraitDto> ProjectToTraitDto(IQueryable<PersistedTrait> query)
        {
            return query.Select(t => new FullTraitDto
            {
                Name = t.Name,
                InGameKey = t.InGameKey,
                TierString = t.TierString,
                Desc = t.Desc,
                Stats = t.Stats,
                Tiers = t.Tiers.Select(tt => new TraitTierDto
                {
                    Level = tt.Level,
                    Rarity = tt.Rarity,
                    Desc = tt.Desc,
                }).ToList()
            });
        }

    }
}
