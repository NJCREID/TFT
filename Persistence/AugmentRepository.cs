using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Augments;

namespace TFT_API.Persistence
{
    public class AugmentRepository(TFTContext context) : IAugmentDataAccess
    {
        private readonly TFTContext _context = context;

        public async Task<AugmentDto?> GetAugmentByKeyAsync(string key)
        {
            return await ProjectToAugmentDto(_context.Augments
                .Where(a => a.Key == key))
                .FirstOrDefaultAsync();
        }

        public async Task<List<AugmentDto>> GetAugmentsAsync()
        {
            return await ProjectToAugmentDto(_context.Augments
                .Where(a => a.IsHidden != true)
                .OrderBy(a => a.Tier))
                .ToListAsync();
        }

        public async Task<List<AugmentDto>> GetAugmentsByTierAsync(int tier)
        {
            return await ProjectToAugmentDto(_context.Augments
                .Where(a => a.IsHidden != true && a.Tier == tier))
                .ToListAsync();
        }

        public static IQueryable<AugmentDto> ProjectToAugmentDto(IQueryable<PersistedAugment> query)
        {
            return query.Select(a => new AugmentDto
            {
                InGameKey = a.InGameKey,
                Name = a.Name,
                Desc = a.Desc,
                Tier = a.Tier
            });
        }
    }
}
