using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Trait;

namespace TFT_API.Persistence
{
    public class TraitRepository: ITraitDataAccess
    {
        private readonly TFTContext _context;

        public TraitRepository(TFTContext context)
        {
            _context = context;
        }

        public PersistedTrait AddTrait(PersistedTrait trait)
        {
            _context.Traits.Add(trait);
            _context.SaveChanges();
            return trait;
        }

        public List<PersistedTrait> GetTraits()
        {
            var traits = _context.Traits.AsNoTracking().Include(t => t.Tiers).ToList();
            return traits;
        }
        public PersistedTrait? GetTraitByKey(string key)
        {
            return _context.Traits.FirstOrDefault(t => t.Key == key);
        }
    }
}
