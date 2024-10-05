using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Unit;
using TFT_API.Models.Trait;

namespace TFT_API.Persistence
{
    public class UnitRepository(TFTContext context) : IUnitDataAccess
    {
        private readonly TFTContext _context = context;

        // Gets a unit by its key, returning a UnitDto or null
        public async Task<UnitDto?> GetUnitByKeyAsync(string key)
        {
            return await ProjectToUnitDto(_context.Units
                .AsSplitQuery()
                .Where(u => u.Key == key))
                .FirstOrDefaultAsync();
        }

        // Gets a list of all units that are not hidden, returning a list of UnitDto
        public async Task<List<UnitDto>> GetFullUnitsAsync()
        {
            return await ProjectToUnitDto(_context.Units
                .AsSplitQuery()
                .Where(u => u.IsHidden != true))
                .ToListAsync();
        }

        // Gets a list of partial unit details, excluding hidden units
        public async Task<List<PartialUnitDto>> GetPartialUnitsAsync()
        {
            return await _context.Units
                .AsSplitQuery()
                .Where(u => u.IsHidden != true)
                .Select(u => new PartialUnitDto
                {
                    InGameKey = u.InGameKey,
                    Name = u.Name,
                    Tier = u.Tier,
                    Cost = u.Cost,
                    RecommendedItems = u.RecommendedItems,
                    Traits = u.Traits
                        .Select(t => new PartialTraitDto
                        {
                            InGameKey  = t.Trait.InGameKey,
                            Name = t.Trait.Name,
                            TierString = t.Trait.TierString,
                            Tiers = t.Trait.Tiers.Select(tt => new TraitTierDto
                            {
                                Level = tt.Level,
                                Rarity = tt.Rarity,
                            }).ToList() 
                        })
                        .ToList(),
                    IsItemIncompatible = u.IsItemIncompatible,
                    CompatabilityType = u.CompatabilityType,
                    IsTriggerUnit = u.IsTriggerUnit,
                    UniqueItemKey = u.UniqueItemKey
                })
                .ToListAsync();
        }

        // Projects a query of PersistedUnit into a UnitDto
        private static IQueryable<UnitDto> ProjectToUnitDto(IQueryable<PersistedUnit> query)
        {
            return query.Select(u => new UnitDto
            {
                InGameKey = u.InGameKey,
                Name = u.Name,
                Tier = u.Tier,
                Cost = u.Cost,
                RecommendedItems = u.RecommendedItems,
                Health = u.Health,
                AttackDamage = u.AttackDamage,
                DamagePerSecond = u.DamagePerSecond,
                AttackRange = u.AttackRange,
                AttackSpeed = u.AttackSpeed,
                Armor = u.Armor,
                MagicalResistance = u.MagicalResistance,
                Skill = u.Skill,
                Traits = u.Traits
                        .Select(t => new TraitDto
                        {
                            InGameKey = t.Trait.InGameKey,
                            Name = t.Trait.Name,
                            TierString = t.Trait.TierString,
                        }).ToList(),
            });
        }
    }
}
