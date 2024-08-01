using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Augments;
using TFT_API.Models.Item;

namespace TFT_API.Persistence
{
    public class AugmentRepository : IAugmentDataAccess
    {
        private readonly TFTContext _context;

        public AugmentRepository(TFTContext context)
        {
            _context = context;
        }
        public PersistedAugment AddAugment(PersistedAugment augment)
        {
            _context.Augments.Add(augment);
            return Save(augment);
        }

        public PersistedAugment? GetAugmentByKey(string key)
        {
            return _context.Augments.AsNoTracking().FirstOrDefault(a => a.Key == key);
        }

        public List<PersistedAugment> GetAugments()
        {
            var augments = _context.Augments
                .AsNoTracking()
                .Where(a => a.IsHidden != true)
                .OrderBy(a => a.Tier)
                .ToList();
            return augments;
        }

        public List<PersistedAugment> GetAugmentsByTier(int tier)
        {
            var components = _context.Augments
                .AsNoTracking()
                .Where(a => a.IsHidden != true)
                .Where(item => item.Tier == tier)
                .ToList();

            return components;
        }
        public PersistedAugment Save(PersistedAugment item)
        {
            _context.SaveChanges();
            return item;
        }
    }
}
