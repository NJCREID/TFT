using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Models.Match;
using TFT_API.Models.Stats;
using TFT_API.Models.Stats.CoOccurrence;
using Match = TFT_API.Models.Match.Match;

namespace TFT_API.Services
{
    /// <summary>
    /// Service for processing and managing co-occurrences of units, items, traits, and augments in TFT matches.
    /// </summary>
    public class CooccurrenceService(TFTContext context)
    {
        private readonly TFTContext _context = context;
        private readonly Dictionary<(string, string), CoOccurrence> _coOccurrence = [];
        private Dictionary<string, int[]> _tiersData = [];
        private Dictionary<string, string> _nameLookup = [];


        /// <summary>
        /// Processes matches to calculate and update co-occurrence statistics.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ProcessMatches()
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var statsToRemove = _context.BaseCoOccurrences.SelectMany(bc => bc.CoOccurrences.Select(c => c.Stat));
                _context.Stats.RemoveRange(statsToRemove);
                _context.BaseCoOccurrences.RemoveRange(_context.BaseCoOccurrences);

                var matches = await _context.Matches
                    .AsSplitQuery()
                    .Include(m => m.Traits)
                    .Include(m => m.Units).ToListAsync();
                var totalMatches = 0;
                var totalPlacement = 0;

                _context.SaveChanges();

                await BuildNameLookupAsync();
                await BuildTierDataAsync();

                // Update co-occurrence statistics for each match.
                foreach (var match in matches)
                {
                    UpdateAllCoOccurrences(match);
                    totalMatches++;
                    totalPlacement += match.Placement;
                }

                await CheckVisibilityAsync();
                SetDelta(totalMatches, totalPlacement);
                await SaveBaseCoOccurrenceAsync(totalMatches);
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Builds a lookup table for unit, item, augment, and trait names.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task BuildNameLookupAsync()
        {
            var unitNames = await _context.Units.AsNoTracking().ToDictionaryAsync(u => u.InGameKey, u => u.Name);
            var itemNames = await _context.Items.AsNoTracking().ToDictionaryAsync(i => i.InGameKey, i => i.Name);
            var augmentNames = await _context.Augments.AsNoTracking().ToDictionaryAsync(a => a.InGameKey, a => a.Name);
            var traitNames = await _context.Traits.AsNoTracking().ToDictionaryAsync(t => t.InGameKey, t => t.Name);

            _nameLookup = unitNames
                .Concat(itemNames)
                .Concat(augmentNames)
                .Concat(traitNames)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Builds a lookup table for trait tiers data.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task BuildTierDataAsync()
        {
            _tiersData = await _context.Traits
                        .Select(t => new
                        {
                            t.InGameKey,
                            Levels = t.Tiers.Select(tier => tier.Level).ToArray()
                        })
                        .ToDictionaryAsync(
                            t => t.InGameKey,
                            t => t.Levels
                        );
        }

        /// <summary>
        /// Saves the calculated base co-occurrence statistics to the database.
        /// </summary>
        /// <param name="totalMatches">Total number of matches processed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task SaveBaseCoOccurrenceAsync(int totalMatches)
        {
            var baseCoOccurrence = new BaseCoOccurrence
            {
                Games = totalMatches,
                CoOccurrences = [.. _coOccurrence.Values]
            };

            _context.BaseCoOccurrences.Add(baseCoOccurrence);
            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// Updates co-occurrence statistics with a given match.
        /// </summary>
        /// <param name="match">The match to process.</param>
        private void UpdateAllCoOccurrences(Match match)
        {
            
            var placement = match.Placement;
            bool isTop4 = placement <= 4;
            bool isWin = placement == 1;

            
            for (int i = 0; i < match.Units.Count; i++)
            {
                var unit1 = match.Units[i];
                // Units with Units
                for (int j = i + 1; j < match.Units.Count; j++)
                {
                    var unit2 = match.Units[j];
                    UpdateCoOccurrenceStats("unit:unit", unit1.CharacterId, unit2.CharacterId, placement, isTop4, isWin);
                }
                // Units with Traits
                foreach (var trait in match.Traits)
                {
                    UpdateTrait(trait, out int updatedNumUnits);
                    if (updatedNumUnits == 0) continue;
                    UpdateCoOccurrenceStats("unit:trait", unit1.CharacterId, $"{trait.Name}-{updatedNumUnits}", placement, isTop4, isWin);
                }
                // Units with Augments
                foreach (var augment in match.Augments)
                {
                    UpdateCoOccurrenceStats("unit:augment", unit1.CharacterId, augment, placement, isTop4, isWin);
                }
                // Units with Items
                foreach (var item in unit1.ItemNames)
                {
                    UpdateCoOccurrenceStats("unit:item", unit1.CharacterId, item, placement, isTop4, isWin);
                }
            }

            
            var allItems = match.Units.SelectMany(u => u.ItemNames).ToList();
            for (int i = 0; i < allItems.Count; i++)
            {
                var item1 = allItems[i];
                // Items with Items
                for (int j = i + 1; j < allItems.Count; j++)
                {
                    var item2 = allItems[j];
                    UpdateCoOccurrenceStats("item:item", item1, item2, placement, isTop4, isWin);
                }
                // Items with Traits
                foreach (var trait in match.Traits)
                {
                    UpdateTrait(trait, out int updatedNumUnits);
                    if (updatedNumUnits == 0) continue;
                    UpdateCoOccurrenceStats("item:trait", item1, $"{trait.Name}-{updatedNumUnits}", placement, isTop4, isWin);
                }
                for (int k = 0; k < match.Augments.Count; k++)
                {
                    UpdateCoOccurrenceStats("item:augment", item1, match.Augments[k], placement, isTop4, isWin);
                }
            }

            
            for (int i = 0; i < match.Traits.Count; i++)
            {
                var trait1 = match.Traits[i];
                UpdateTrait(trait1, out int updatedNumUnits1);
                if (updatedNumUnits1 == 0) continue;
                // Traits with Traits
                for (int j = i + 1; j < match.Traits.Count; j++)
                {
                    var trait2 = match.Traits[j];
                    UpdateTrait(trait2, out int updatedNumUnits2);
                    if (updatedNumUnits2 == 0) continue;
                    UpdateCoOccurrenceStats("trait:trait", $"{trait1.Name}-{updatedNumUnits1}", $"{trait2.Name}-{updatedNumUnits2}", placement, isTop4, isWin);
                }
                // Traits with Augments
                foreach (var augment in match.Augments)
                {
                    UpdateCoOccurrenceStats("trait:augment", $"{trait1.Name}-{updatedNumUnits1}", augment, placement, isTop4, isWin);
                }
            }

            // Augments with Augments
            for (int i = 0; i < match.Augments.Count; i++)
            {
                for (int j = i + 1; j < match.Augments.Count; j++)
                {
                    UpdateCoOccurrenceStats("augment:augment", match.Augments[i], match.Augments[j], placement, isTop4, isWin);
                }
            }
        }

        /// <summary>
        /// Sets the delta value for each co-occurrence statistic based on average placement.
        /// </summary>
        /// <param name="totalMatches">Total number of matches processed.</param>
        /// <param name="totalPlacement">Total placement points across all matches.</param>
        private void SetDelta(int totalMatches, int totalPlacement)
        {
            double averagePlace = totalMatches != 0
                ? totalPlacement / (double)totalMatches
                : 0;

            foreach (var kvp in _coOccurrence)
            {
                var stat = kvp.Value.Stat;
                stat.Delta = stat.Games != 0 ? (stat.Place / (double)stat.Games) - averagePlace : averagePlace;
            }
        }

        /// <summary>
        /// Updates the number of units for a given trait based on its floored tier.
        /// </summary>
        /// <param name="trait">The trait for which to update the number of units.</param>
        /// <param name="updatedNumUnits">The updated number of units for the trait based on the closest tier level.</param>
        private void UpdateTrait(MatchTrait trait, out int updatedNumUnits)
        {
            if (_tiersData != null && _tiersData.TryGetValue(trait.Name, out var tiers))
            {
                var flooredNumUnits = tiers.OrderBy(t => t).LastOrDefault(t => t <= trait.NumUnits);
                updatedNumUnits = flooredNumUnits == 0 ? 0 : flooredNumUnits;
            }
            else
            {
                updatedNumUnits = trait.NumUnits;
            }
        }

        /// <summary>
        /// Updates co-occurrence statistics for a pair of items, traits, units, or augments.
        /// </summary>
        /// <param name="type">The type of co-occurrence (e.g., unit:trait, item:item).</param>
        /// <param name="key1">The first key in the co-occurrence pair.</param>
        /// <param name="key2">The second key in the co-occurrence pair.</param>
        /// <param name="placement">The placement of the match.</param>
        /// <param name="isTop4">Indicates whether the match placement was in the top 4.</param>
        /// <param name="isWin">Indicates whether the match placement was a win.</param>
        private void UpdateCoOccurrenceStats(string type, string key1, string key2, int placement, bool isTop4, bool isWin)
        {
            var pair = string.Compare(key1, key2) > 0 ? (key2, key1) : (key1, key2);

            var lookupKey1 = pair.Item1.Split('-')[0];
            var lookupKey2 = pair.Item2.Split('-')[0];
            if (!_coOccurrence.TryGetValue(pair, out var coOccurrence))
            {
                var name1 = _nameLookup.TryGetValue(lookupKey1, out var n1) ? n1 : pair.Item1;
                var name2 = _nameLookup.TryGetValue(lookupKey2, out var n2) ? n2 : pair.Item2;
                coOccurrence = new CoOccurrence
                {
                    Type = type,
                    Key1 = pair.Item1,
                    Key2 = pair.Item2,
                    Name1 = name1,
                    Name2 = name2,
                    Stat = new Stat()
                };
                _coOccurrence[pair] = coOccurrence;
            }

            var stat = coOccurrence.Stat;
            stat.Games++;
            stat.Place += placement;
            stat.Top4 += isTop4 ? 1 : 0;
            stat.Win += isWin ? 1 : 0;
        }

        /// <summary>
        /// Checks the visibility of units, items, traits, and augments, and filters out hidden co-occurrences.
        /// </summary>
        /// <returns>A task representing the asynchronous operation of checking and filtering visibility.</returns>
        private async Task CheckVisibilityAsync()
        {
            var units = await _context.Units.AsNoTracking().ToListAsync();
            var items = await _context.Items.AsNoTracking().ToListAsync();
            var augments = await _context.Augments.AsNoTracking().ToListAsync();
            var traits = await _context.Traits.AsNoTracking().ToListAsync();

            var unitDict = units.ToDictionary(u => u.InGameKey, u => u.IsHidden != true);
            var itemDict = items.ToDictionary(i => i.InGameKey, i => i.IsHidden != true);
            var augmentDict = augments.ToDictionary(a => a.InGameKey, a => a.IsHidden != true);
            var traitDict = traits.ToDictionary(t => t.InGameKey, t => t.IsHidden != true);

            var filteredCoOccurrences = new Dictionary<(string, string), CoOccurrence>();

            foreach (var kvp in _coOccurrence)
            {
                var (key1, key2) = kvp.Key;
                var coOccurrence = kvp.Value;

                bool IsVisible(string key)
                {
                    if (unitDict.TryGetValue(key, out var unitVisible)) return unitVisible;
                    if (itemDict.TryGetValue(key, out var itemVisible)) return itemVisible;
                    if (augmentDict.TryGetValue(key, out var augmentVisible)) return augmentVisible;
                    if (traitDict.TryGetValue(key.Split('-')[0], out var traitVisible)) return traitVisible;
                    return false;
                }

                if (IsVisible(key1) && IsVisible(key2))
                {
                    filteredCoOccurrences[kvp.Key] = coOccurrence;
                }
            }

            _coOccurrence.Clear();
            foreach (var kvp in filteredCoOccurrences)
            {
                _coOccurrence[kvp.Key] = kvp.Value;
            }
        }
    }
}
