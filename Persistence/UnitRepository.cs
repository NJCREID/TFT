using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Unit;

namespace TFT_API.Persistence
{
    public class UnitRepository : IUnitDataAccess
    {
        private readonly TFTContext _context;

        public UnitRepository(TFTContext context)
        {
            _context = context;
        }
        public PersistedUnit AddUnit(PersistedUnit unit)
        {
            _context.Units.Add(unit);
            return Save(unit);
        }

        public PersistedUnit? GetUnitByKey(string key)
        {
            var unit = _context.Units
                 .AsNoTracking()
                 .Include(u => u.Traits)
                    .ThenInclude(ut => ut.Trait)
                 .FirstOrDefault(u => u.Key == key);

            return unit;
        }

        public List<PersistedUnit> GetUnits()
        {
            var units = _context.Units
                .AsNoTracking()
                .Include(u => u.Traits)
                    .ThenInclude(ut => ut.Trait)
                .ToList();

            return units;
        }
        public PersistedUnit Save(PersistedUnit unit)
        {
            _context.SaveChanges();
            return unit;
        }
    }
}
