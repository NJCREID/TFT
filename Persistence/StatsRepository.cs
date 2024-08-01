using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Stats.AugmentStats;
using TFT_API.Models.Stats.CompStats;
using TFT_API.Models.Stats.ItemStats;
using TFT_API.Models.Stats.TraitStats;
using TFT_API.Models.Stats.UnitStats;

namespace TFT_API.Persistence
{
    public class StatsRepository : IStatDataAccess
    {
        private readonly TFTContext _context;

        public StatsRepository(TFTContext context)
        {
            _context = context;
        }
        public BaseUnitStat? GetUnitStats()
        {
            return _context.BaseUnitStat
                .AsNoTracking()
                .AsSplitQuery()
                .Include(bu => bu.UnitStats)
                    .ThenInclude(us => us.Stat)
                .Include(bu => bu.StarredUnitStats)
                    .ThenInclude(sus => sus.Stat)
                .FirstOrDefault();
        }   

        public BaseTraitStat? GetTraitStats()
        {
            return _context.BaseTraitStat
                .AsNoTracking()
                .AsSplitQuery()
                .Include(bu => bu.TraitStats)
                    .ThenInclude(us => us.Stat)
                .FirstOrDefault();
       
        }

        public BaseAugmentStat? GetAugmentStats()
        {
            return _context.BaseAugmentStat
                .AsNoTracking()
                .AsSplitQuery()
                .Include(bu => bu.AugmentStats)
                    .ThenInclude(us => us.Stats)
                .FirstOrDefault();
        }

        public BaseItemStat? GetItemStats()
        {
            return _context.BaseItemStat
                .AsNoTracking()
                .AsSplitQuery()
                .Include(bu => bu.ItemStats)
                    .ThenInclude(us => us.Stat)
                .FirstOrDefault();
        }

        public BaseCompStat? GetCompStats()
        {
            return _context.BaseCompStat
                .AsNoTracking()
                .AsSplitQuery()
                .Include(cs => cs.CompStats)
                    .ThenInclude(css => css.Stat)
                .Include(cs => cs.CompStats)
                     .ThenInclude(csu => csu.Units)
                        .ThenInclude(csuu => csuu.Unit)
                .FirstOrDefault();
        }


    }
}
