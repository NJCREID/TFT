using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using TFT_API.Data;
using TFT_API.Helpers;
using TFT_API.Models.Stats;
using TFT_API.Models.Match;
using TFT_API.Models.Stats.AugmentStats;
using TFT_API.Models.Stats.ItemStats;
using TFT_API.Models.Stats.TraitStats;
using TFT_API.Models.Stats.UnitStats;

namespace TFT_API.Services
{
    public class CoocurrenceStats
    {
        private readonly TFTContext _context;

        public CoocurrenceStats(TFTContext context)
        {
            _context = context;
        }

        public async Task<BaseStat> CalculateStatisticsAsync(string[] constraints = null)
        {
            var matchesQuery = _context.Matches
                .AsNoTracking()
                .AsSplitQuery()
                .Select(m => new
                {
                    m.Placement,
                    m.Augments,
                    Units = m.Units.Select(u => new { u.CharacterId, u.ItemNames, u.Tier }).ToList(),
                    Traits = m.Traits.Select(t => new MatchTraitDto { Name = t.Name, NumUnits = t.NumUnits }).ToList()
                })
                .AsQueryable();

            if (constraints != null && constraints.Length > 0)
            {
                foreach (var constraint in constraints)
                {
                    if (constraint.StartsWith("u-"))
                    {
                        var unitId = constraint.Substring(2);
                        matchesQuery = matchesQuery.Where(m => m.Units.Any(u => u.CharacterId == unitId));
                    }
                    else if (constraint.StartsWith("i-"))
                    {
                        var match = Regex.Match(constraint, @"i-(.*?)-(.*)$");
                        if (match.Success)
                        {
                            var itemId = match.Groups[1].Value;
                            var unitId = match.Groups[2].Value;
                            matchesQuery = matchesQuery.Where(m => m.Units.Any(u => u.CharacterId == unitId && u.ItemNames.Contains(itemId)));
                        }
                    }
                    else if (constraint.StartsWith("a-"))
                    {
                        var augmentId = constraint.Substring(2);
                        matchesQuery = matchesQuery.Where(m => m.Augments.Contains(augmentId));
                    }
                    else if (constraint.StartsWith("t-"))
                    {
                        var match = Regex.Match(constraint, @"t-(.*?)-(\d+)$");
                        if (match.Success)
                        {
                            var traitId = match.Groups[1].Value;
                            var numUnits = int.Parse(match.Groups[2].Value);
                            matchesQuery = matchesQuery.Where(m => m.Traits.Any(t => t.Name == traitId && t.NumUnits == numUnits));
                        }
                    }
                }
            }

            var matches = await matchesQuery.ToListAsync();

            var unitStats = new Dictionary<string, UnitStat>();
            var starUnitStats = new Dictionary<string, StarredUnitStat>();
            var traitStats = new Dictionary<string, Dictionary<int, TraitStat>>();
            var augmentStats = new Dictionary<string, AugmentStat>();
            var itemStats = new Dictionary<string, ItemStat>();

            var totalMatches = matches.Count;
            var top4 = 0;
            var win = 0;
            var place = 0;

            foreach (var match in matches)
            {
                var placement = match.Placement;
                if (placement <= 4) top4++;
                if (placement == 1) win++;
                place += placement;

                foreach (var unit in match.Units)
                {
                    if (!unitStats.TryGetValue(unit.CharacterId, out var unitStat))
                    {
                        unitStat = new UnitStat { Name = unit.CharacterId, Stat = new Stat() };
                        unitStats[unit.CharacterId] = unitStat;
                    }

                    var stat = unitStat.Stat;
                    stat.Games++;
                    stat.Place += placement;
                    stat.Top4 += placement <= 4 ? 1 : 0;
                    stat.Win += placement == 1 ? 1 : 0;

                    if (unit.Tier == 3)
                    {
                        if (!starUnitStats.TryGetValue(unit.CharacterId, out var starUnitStat))
                        {
                            starUnitStat = new StarredUnitStat { Name = unit.CharacterId, Stat = new Stat() };
                            starUnitStats[unit.CharacterId] = starUnitStat;
                        }

                        var starStat = starUnitStats[unit.CharacterId].Stat;
                        starStat.Games++;
                        starStat.Place += placement;
                        starStat.Top4 += placement <= 4 ? 1 : 0;
                        starStat.Win += placement == 1 ? 1 : 0;
                    }

                    foreach (var item in unit.ItemNames)
                    {
                        if (!itemStats.TryGetValue(item, out var itemStat))
                        {
                            itemStat = new ItemStat { Name = item, Stat = new Stat() };
                            itemStats[item] = itemStat;
                        }

                        var itemStatInstance = itemStats[item].Stat;
                        itemStatInstance.Games++;
                        itemStatInstance.Place += placement;
                        itemStatInstance.Top4 += placement <= 4 ? 1 : 0;
                        itemStatInstance.Win += placement == 1 ? 1 : 0;
                    }
                }

                foreach (var trait in match.Traits)
                {
                    if (TiersData.Tiers.TryGetValue(trait.Name, out int[] tiers))
                    {
                        var flooredNumUnits = tiers.OrderBy(t => t).LastOrDefault(t => t <= trait.NumUnits);
                        if (flooredNumUnits == 0) continue;
                        trait.NumUnits = flooredNumUnits;
                    }

                    if (!traitStats.TryGetValue(trait.Name, out var traitDict))
                    {
                        traitDict = new Dictionary<int, TraitStat>();
                        traitStats[trait.Name] = traitDict;
                    }

                    if (!traitDict.TryGetValue(trait.NumUnits, out var traitStat))
                    {
                        traitStat = new TraitStat { Name = trait.Name, NumUnits = trait.NumUnits, Stat = new Stat() };
                        traitDict[trait.NumUnits] = traitStat;
                    }

                    var traitStatInstance = traitDict[trait.NumUnits].Stat;
                    traitStatInstance.Games++;
                    traitStatInstance.Place += placement;
                    traitStatInstance.Top4 += placement <= 4 ? 1 : 0;
                    traitStatInstance.Win += placement == 1 ? 1 : 0;
                }

                for (int i = 0; i < match.Augments.Count; i++)
                {
                    var augment = match.Augments[i];
                    if (!augmentStats.TryGetValue(augment, out var augmentStat))
                    {
                        augmentStat = new AugmentStat { Name = augment, Stats = new List<Stat>() };
                        augmentStats[augment] = augmentStat;
                    }

                    while (augmentStat.Stats.Count <= i)
                    {
                        augmentStat.Stats.Add(new Stat());
                    }

                    var stat = augmentStat.Stats[i];
                    stat.Games++;
                    stat.Place += placement;
                    stat.Top4 += placement <= 4 ? 1 : 0;
                    stat.Win += placement == 1 ? 1 : 0;
                }
            }

            double averagePlace = totalMatches != 0 ? place / (double)totalMatches : 0;

            void UpdateDelta(Stat stat)
            {
                stat.Delta = stat.Games != 0 ? averagePlace - (stat.Place / (double)stat.Games) : averagePlace;
            }

            foreach (var unit in unitStats.Values) UpdateDelta(unit.Stat);
            foreach (var item in itemStats.Values) UpdateDelta(item.Stat);
            foreach (var traits in traitStats.Values) foreach (var trait in traits.Values) UpdateDelta(trait.Stat);
            foreach (var augment in augmentStats.Values) foreach (var stat in augment.Stats) UpdateDelta(stat);

            return new BaseStat
            {
                Games = totalMatches,
                Place = place,
                Top4 = top4,
                Win = win,
                UnitStats = unitStats.Values.ToList(),
                ItemStats = itemStats.Values.ToList(),
                AugmentStats = augmentStats.Values.ToList(),
                TraitStats = traitStats.Values.SelectMany(t => t.Values).ToList(),
            };
        }
    }
}
