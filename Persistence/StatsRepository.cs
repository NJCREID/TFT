using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Stats;
using TFT_API.Models.Stats.AugmentStats;
using TFT_API.Models.Stats.CompStats;
using TFT_API.Models.Stats.CoOccurrence;
using TFT_API.Models.Stats.ItemStats;
using TFT_API.Models.Stats.TraitStats;
using TFT_API.Models.Stats.UnitStats;

namespace TFT_API.Persistence
{
    public class StatsRepository(TFTContext context) : IStatDataAccess
    {
        private readonly TFTContext _context = context;

        public async Task<BaseUnitStatDto?> GetUnitStatsAsync(string league)
        {
            return await _context.BaseUnitStat
                .AsSplitQuery()
                .Where(ts => ts.League == league)
                .Select(bu => new BaseUnitStatDto
                {
                    Games = bu.Games,
                    UnitStats = bu.UnitStats.Select(us => new UnitStatDto
                    {
                        Name = us.Name,
                        InGameKey = us.InGameKey,
                        Stat = new StatDto
                        {
                            Games = us.Stat.Games,
                            Place = us.Stat.Place,
                            Top4 = us.Stat.Top4,
                            Win = us.Stat.Win,
                            Delta = us.Stat.Delta
                        }
                    }).ToList(),
                    StarredUnitStats = bu.StarredUnitStats.Select(sus => new StarredUnitStatDto
                    {
                        Name = sus.Name,
                        InGameKey = sus.InGameKey,
                        Stat = new StatDto
                        {
                            Games = sus.Stat.Games,
                            Place = sus.Stat.Place,
                            Top4 = sus.Stat.Top4,
                            Win = sus.Stat.Win,
                            Delta = sus.Stat.Delta
                        }
                    }).ToList()
                })
                .SingleOrDefaultAsync();
        }

        public async Task<BaseTraitStatDto?> GetTraitStatsAsync(string league)
        {
            return await _context.BaseTraitStat
                .AsSplitQuery()
                .Where(ts => ts.League == league)
                .Select(ts => new BaseTraitStatDto
                {
                    Games = ts.Games,
                    TraitStats = ts.TraitStats.Select(ts => new TraitStatDto
                    {
                        Name = ts.Name,
                        InGameKey = ts.InGameKey,
                        NumUnits = ts.NumUnits,
                        Stat = new StatDto
                        {
                            Games = ts.Stat.Games,
                            Place = ts.Stat.Place,
                            Top4 = ts.Stat.Top4,
                            Win = ts.Stat.Win,
                            Delta = ts.Stat.Delta
                        }
                    }).ToList()
                })
                .SingleOrDefaultAsync();
        }

        public async Task<BaseAugmentStatDto?> GetAugmentStatsAsync(string league)
        {
            return await _context.BaseAugmentStat
                .AsSplitQuery()
                .Where(bu => bu.League == league)
                .Select(bu => new BaseAugmentStatDto
                {
                    Games = bu.Games,
                    AugmentStats = bu.AugmentStats.Select(aus => new AugmentStatDto
                    {
                        Name = aus.Name,
                        InGameKey = aus.InGameKey,
                        Stats = aus.Stats.Select(s => new StatDto
                        {
                            Games = s.Games,
                            Place = s.Place,
                            Top4 = s.Top4,
                            Win = s.Win,
                            Delta = s.Delta
                        }).ToList()
                    }).ToList()
                })
                .SingleOrDefaultAsync();
        }

        public async Task<BaseItemStatDto?> GetItemStatsAsync(string league)
        {
            return await _context.BaseItemStat
                .AsSplitQuery()
                .Where(bu => bu.League == league)
                .Select(bu => new BaseItemStatDto
                {
                    Games = bu.Games,
                    ItemStats = bu.ItemStats.Select(its => new ItemStatDto
                    {
                        Name = its.Name,
                        InGameKey = its.InGameKey,
                        Stat = new StatDto
                        {
                            Games = its.Stat.Games,
                            Place = its.Stat.Place,
                            Top4 = its.Stat.Top4,
                            Win = its.Stat.Win,
                            Delta = its.Stat.Delta
                        }
                    }).ToList()
                })
                .SingleOrDefaultAsync();
        }

        public async Task<BaseCompStatDto?> GetCompStatsAsync(string league)
        {
            return await _context.BaseCompStat
                .AsSplitQuery()
                .Where(cs => cs.League == league)
                .Select(cs => new BaseCompStatDto
                {
                    Games = cs.Games,
                    CompStats = cs.CompStats.Select(cs => new CompStatDto
                    {
                        Name = cs.Name,
                        InGameKey = cs.InGameKey,
                        Units = cs.Units.Select(csu => new CompStatUnitDto
                        {
                            InGameKey = csu.Unit.InGameKey,
                            Name = csu.Unit.Name,
                            Tier = csu.Unit.Tier
                        }).ToList(),
                        Stat = new StatDto
                        {
                            Games = cs.Stat.Games,
                            Place = cs.Stat.Place,
                            Top4 = cs.Stat.Top4,
                            Win = cs.Stat.Win,
                            Delta = cs.Stat.Delta
                        }
                    }).ToList()
                })
                .SingleOrDefaultAsync();
        }

        public async Task<BaseCoOccurrenceDto?> GetCoOccurrenceStatsAsync(string type, string key)
        {
            var reversedType = string.Join(":", type.Split(':').Reverse());

            return await _context.BaseCoOccurrences
                .AsSplitQuery()
                .Where(b => b.CoOccurrences.Any(c =>
                    (c.Type == type || c.Type == reversedType) &&
                    (c.Key1 == key || c.Key2 == key)))
                .Select(b => new BaseCoOccurrenceDto
                {
                    Games = b.Games,
                    CoOccurrences = b.CoOccurrences
                        .Where(c =>
                            (c.Type == type || c.Type == reversedType) &&
                            (c.Key1 == key || c.Key2 == key))
                        .Select(c => new CoOccurrenceDto
                        {
                            InGameKey = c.Key1 == key ? c.Key2 : c.Key1,
                            Name = c.Key1 == key ? c.Name2 : c.Name1,
                            Stat = new StatDto
                            {
                                Games = c.Stat.Games,
                                Place = c.Stat.Place,
                                Top4 = c.Stat.Top4,
                                Win = c.Stat.Win,
                                Delta = c.Stat.Delta
                            }
                        }).ToList()
                })
                .SingleOrDefaultAsync();
        }
    }
}
