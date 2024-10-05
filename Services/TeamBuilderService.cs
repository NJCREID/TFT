using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;
using TFT_API.Data;
using TFT_API.Models.Item;
using TFT_API.Models.Match;
using TFT_API.Models.Stats.TraitStats;
using TFT_API.Models.Stats.UnitStats;
using TFT_API.Models.Trait;
using TFT_API.Models.Unit;
using TFT_API.Models.UserGuides;
using Match = TFT_API.Models.Match.Match;

namespace TFT_API.Services
{
    /// <summary>
    /// Build teams around each unit by using match data and generating statistics.
    /// </summary>
    public class TeamBuilderService(TFTContext context, IConfiguration configuration, IMemoryCache memoryCache)
    {
        private readonly TFTContext _context = context;
        private readonly IConfiguration _configuration = configuration;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private List<Match> _matchList = [];
        private List<PersistedUnit> _units = [];
        private List<TraitStat> _traitStats = [];
        private List<UnitStat> _unitStats = [];
        private List<PersistedTrait> _traits = [];
        private Dictionary<string, string> _emblemToTrait = [];
        private Dictionary<string, Dictionary<string, int>> _unitCooccurrence = [];
        private Dictionary<string, Dictionary<string, (int occurrenceRate, double avgAmountPerUnit)>> _unitItemRates = [];
        private Dictionary<string, Dictionary<string, int>> _augmentPlayRates = [];
        private Dictionary<string, Dictionary<string, int>> _emblems = [];
        private Dictionary<string, int> _unitThreeItemCounts = [];
        private Dictionary<string, (int count, bool? isContributing, List<PersistedUnit> units)> _traitContributions = [];
        private List<PersistedUnit> _team = [];
        private int _matchCount;

        //Units who should be exluded from building teams from. (Units that are trait specific or summoned)
        private readonly string[] _excludedUnits = ["TFT11_Kayle"];

        /// <summary>
        /// Builds the teams by loading data, generating guides, saving them, and clearing auto guide entries.
        /// </summary>
        public async Task BuildTeams()
        {
            await LoadData();

            var guides = await GenerateGuides();

            await SaveGuides(guides);

            ClearAutoGuideEntries();
        }

        /// <summary>
        /// Loads necessary data from the database, including units, trait stats, unit stats, traits, matches, and emblem-trait mappings.
        /// </summary>
        private async Task LoadData()
        {
            _units = await _context.Units.Include(u => u.Traits).Where(u => u.IsHidden != true).ToListAsync();
            _traitStats = await _context.TraitStats.AsNoTracking().Include(ts => ts.Stat).ToListAsync();
            _unitStats = await _context.UnitStats.AsNoTracking().Include(ts => ts.Stat).ToListAsync();
            _traits = await _context.Traits.ToListAsync();
            _matchList = await _context.Matches.AsNoTracking().Include(m => m.Units).ToListAsync();
            var emblems = await _context.Items
                .Where(i => i.AffectedTraitKey != null)
                .AsNoTracking()
                .ToListAsync();
            _emblemToTrait = emblems
                .ToDictionary(
                    i => i.InGameKey,
                    i => _traits.Where(t => t.Key == i.AffectedTraitKey)
                    .Select(t => t.InGameKey)
                    .FirstOrDefault() ?? ""
                );
        }

        /// <summary>
        /// Generates guides for each unit by calculating stats and building teams, excluding specified units.
        /// </summary>
        /// <returns>A list of generated guides.</returns>
        private async Task<List<UserGuide>> GenerateGuides()
        {
            var guides = new List<UserGuide>();

            foreach (var unit in _units)
            {
                if (_excludedUnits.Contains(unit.InGameKey)) continue;
                ResetPrevUnitData();
                var unitMatches = await GetMatchesThatIncludeUnit(unit.InGameKey, unit.Key);
                CalculateStatsForMatches(unitMatches);
                _matchCount = unitMatches.Count;

                var guide = BuildTeam(unit);
                guides.Add(guide);
            }

            return guides;
        }

        /// <summary>
        /// Resets data related to the previous unit.
        /// </summary>
        private void ResetPrevUnitData()
        {
            _unitCooccurrence = [];
            _augmentPlayRates = [];
            _unitItemRates = [];
            _emblems = [];
            _unitThreeItemCounts = [];
            _team = [];
            _traitContributions = [];
        }

        /// <summary>
        /// Saves the generated guides to the database within a transaction, rolling back in case of errors.
        /// </summary>
        /// <param name="guides">The list of guides to save.</param>
        private async Task SaveGuides(List<UserGuide> guides)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.UserGuides.RemoveRange(_context.UserGuides.Where(ug => ug.IsAutoGenerated == true));
                await _context.UserGuides.AddRangeAsync(guides);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
            }
        }

        /// <summary>
        /// Retrieves matches for a given unit based on specific criteria, including carry augments and placement.
        /// </summary>
        /// <param name="unitInGameKey">The in-game key of the unit.</param>
        /// <param name="unitKey">The unique key of the unit.</param>
        /// <returns>A list of matches associated with the unit.</returns>
        private async Task<List<Match>> GetMatchesThatIncludeUnit(string unitInGameKey, string unitKey)
        {
            var carryAugment = await _context.Augments.FirstOrDefaultAsync(a => a.InGameKey.Contains(unitKey) && a.IsHidden != true);
            var query = _matchList.Where(m =>
                m.Units.Any(u => u.CharacterId == unitInGameKey && (u.ItemNames.Count == 3 || u.Tier == 3) && m.Placement > 4)
                );
            if (carryAugment != null)
            {
                query = query.Where(m => m.Augments.Contains(carryAugment.InGameKey));
            }
            return query.ToList();
        }

        /// <summary>
        /// Clears autogenerated guide entries from the memory cache.
        /// </summary>
        private void ClearAutoGuideEntries()
        {
            _memoryCache.Remove("autogenerated");
        }

        /// <summary>
        /// Calculates statistics for the provided matches, updating various metrics related to unit co-occurrences, item rates, and augment stats.
        /// </summary>
        /// <param name="matches">The list of matches to calculate stats for.</param>
        private void CalculateStatsForMatches(List<Match> matches)
        {
            foreach (var match in matches)
            {
                
                var unitsInMatch = match.Units
                    .GroupBy(u => u.CharacterId)
                    .Select(g => g.First())
                    .ToList();
                
                foreach (var unit in unitsInMatch)
                {
                    UpdateUnitCooccurrence(unit, unitsInMatch);
                    UpdateItemRates(unit);
                    UpdateAugmentStats(match, unit);
                    UpdateThreeItemCounts(unit);
                }
            }
        }

        /// <summary>
        /// Updates the co-occurrence counts for a given unit based on other units in the same match.
        /// </summary>
        /// <param name="unit">The unit for which co-occurrences are being updated.</param>
        /// <param name="unitsInMatch">The list of units present in the match.</param>
        private void UpdateUnitCooccurrence(MatchUnit unit, List<MatchUnit> unitsInMatch)
        {
            var unitCharacterId = unit.CharacterId;
            if (!_unitCooccurrence.TryGetValue(unitCharacterId, out var unitCoOccurrences))
            {
                unitCoOccurrences = [];
                _unitCooccurrence[unitCharacterId] = unitCoOccurrences;
            }

            foreach (var coUnit in unitsInMatch)
            {
                var coUnitCharacterId = coUnit.CharacterId;
                if (unitCharacterId == coUnitCharacterId) continue;

                if (!unitCoOccurrences.TryGetValue(coUnitCharacterId, out var unitCoOccurrenceCount))
                {
                    unitCoOccurrenceCount = 0;
                    unitCoOccurrences[coUnitCharacterId] = unitCoOccurrenceCount;
                }

                unitCoOccurrences[coUnitCharacterId] = ++unitCoOccurrenceCount;
            }
        }

        /// <summary>
        /// Updates item rates for a specified unit based on the items used in matches.
        /// </summary>
        /// <param name="unit">The unit for which item rates are being updated.</param>
        private void UpdateItemRates(MatchUnit unit)
        {
            var unitCharacterId = unit.CharacterId;
            if (!_unitItemRates.TryGetValue(unitCharacterId, out var itemCounts))
            {
                itemCounts = [];
                _unitItemRates[unitCharacterId] = itemCounts;
            }

            var currentUnitItemCounts = new Dictionary<string, int>();

            foreach (var item in unit.ItemNames)
            {
                if (item.Contains("Emblem"))
                {
                    UpdateEmblemCounts(unitCharacterId, item);
                }
                if (!currentUnitItemCounts.TryGetValue(item, out var count))
                {
                    count = 0;
                }
                currentUnitItemCounts[item] = count + 1;
            }

            foreach (var item in currentUnitItemCounts.Keys)
            {
                if (!itemCounts.TryGetValue(item, out var itemData))
                {
                    itemData = (0, 0.0);
                }

                int newOccurrenceRate = itemData.occurrenceRate + currentUnitItemCounts[item];
                // Average quantity of the same item appearing on the same unit. 
                double newAvgAmountPerUnit = (itemData.occurrenceRate == 0)
                    ? currentUnitItemCounts[item]
                    : newOccurrenceRate / ((itemData.occurrenceRate / itemData.avgAmountPerUnit) + 1);

                itemCounts[item] = (newOccurrenceRate, newAvgAmountPerUnit);
            }
        }

        /// <summary>
        /// Updates counts for emblems.
        /// </summary>
        /// <param name="unitCharacterId">The character ID of the unit.</param>
        /// <param name="item">The item being counted.</param>
        private void UpdateEmblemCounts(string unitCharacterId, string item)
        {
            if (!_emblems.TryGetValue(unitCharacterId, out var emblemCounts))
            {
                emblemCounts = [];
                _emblems[unitCharacterId] = emblemCounts;
            }

            if (!emblemCounts.TryGetValue(item, out var emblemCount))
            {
                emblemCount = 0;
                emblemCounts[item] = emblemCount;
            }

            emblemCounts[item] = ++emblemCount;
        }

        /// <summary>
        /// Updates statistics for augments used in the matches.
        /// </summary>
        /// <param name="match">The match from which to derive augment statistics.</param>
        /// <param name="unit">The unit for which augment stats are being updated.</param>
        private void UpdateAugmentStats(Match match, MatchUnit unit)
        {
            if (!_augmentPlayRates.TryGetValue(unit.CharacterId, out var augments))
            {
                augments = [];
                _augmentPlayRates[unit.CharacterId] = augments;
            }

            foreach (var augment in match.Augments)
            {
                if (!augments.TryGetValue(augment, out var augmentCount))
                {
                    augmentCount = 0;
                    augments[augment] = augmentCount;
                }

                augments[augment] = ++augmentCount;
            }
        }

        /// <summary>
        /// Updates the count of three-item occurrence for a given unit if applicable.
        /// </summary>
        /// <param name="unit">The unit being checked for three-item configurations.</param>
        private void UpdateThreeItemCounts(MatchUnit unit)
        {
            if (unit.ItemNames.Count == 3)
            {
                if (!_unitThreeItemCounts.TryGetValue(unit.CharacterId, out var threeItemCount))
                {
                    threeItemCount = 0;
                    _unitThreeItemCounts[unit.CharacterId] = threeItemCount;
                }

                _unitThreeItemCounts[unit.CharacterId] = ++threeItemCount;
            }
        }

        /// <summary>
        /// Builds a team for a specified initial unit by selecting additional units based on the genreated statistics.
        /// </summary>
        /// <param name="initialUnit">The initial unit to base the team on.</param>
        /// <returns>The generated guide for the team.</returns>
        private UserGuide BuildTeam(PersistedUnit initialUnit)
        {
            var availableUnits = _units.Where(u => u.InGameKey != initialUnit.InGameKey).ToList();
            HexItem? selectedEmblem = null;

            _team.Add(initialUnit);
            UpdateTraitContributions(initialUnit);

            // While team count is less than 9 continue adding units.
            while (_team.Count < 9)
            {
                var bestUnit = FindBestUnit(availableUnits, initialUnit);
                if (bestUnit == null) break;
                _team.Add(bestUnit);
                UpdateTraitContributions(bestUnit);

                availableUnits = availableUnits.Where(u => u.InGameKey != bestUnit.InGameKey).ToList();

                while (_team.Count == 9)
                {
                    UpdateEmblem(ref selectedEmblem);
                    // Unit considered for determining potential trait contribution improvement
                    var nextBestUnit = FindBestUnit(availableUnits, initialUnit);
                    var unitToRemove = GetLeastContributingUnit(nextBestUnit);
                    if (unitToRemove == null) break;
                    _team.Remove(unitToRemove);
                    UpdateTraitContributions(unitToRemove, false);
                }
            }
            var guide = CreateGuide(initialUnit, selectedEmblem);
            return guide;
        }

        /// <summary>
        /// Updates the selected emblem based on current team contributions and trait statistics.
        /// </summary>
        /// <param name="selectedEmblem">The currently selected emblem, passed by reference.</param>
        private void UpdateEmblem(ref HexItem? selectedEmblem)
        {
            // Unit has been removed and a new one was added in the previous iteration. Emblem and trait contributions needs to be updated.
            if (selectedEmblem != null)
            {
                var trait = _emblemToTrait[selectedEmblem.Item.InGameKey];
                if (_traitContributions.TryGetValue(trait, out var traitContribution))
                {
                    if (!_traitStats.Any(ts => ts.InGameKey == trait && ts.NumUnits == traitContribution.count - 1))
                    {
                        traitContribution.isContributing = false;
                    }
                    traitContribution.count--;
                    _traitContributions[trait] = traitContribution;
                }
            }

            selectedEmblem = SelectEmblem();
            if (selectedEmblem != null)
            {
                var trait = _emblemToTrait[selectedEmblem.Item.InGameKey];
                if (!_traitContributions.TryGetValue(trait, out var traitContribution))
                {
                    bool? traitContributes = _traitStats.Any(ts => ts.InGameKey == trait && ts.NumUnits == 1) ? true : null;
                    traitContribution = (0, traitContributes, []);
                }
                traitContribution.count++;
                _traitContributions[trait] = traitContribution;
            }
        }

        /// <summary>
        /// Updates trait contributions based on whether a unit is being added or removed from the team.
        /// </summary>
        /// <param name="unit">The unit being added or removed.</param>
        /// <param name="isAdding">Indicates whether the unit is being added (true) or removed (false).</param>
        private void UpdateTraitContributions(PersistedUnit unit, bool isAdding = true)
        {
            foreach (var unitTrait in unit.Traits)
            {
                var traitKey = unitTrait.Trait.InGameKey;
                if (!_traitContributions.TryGetValue(traitKey, out var traitContributions))
                {
                    traitContributions = (0, false, []);
                    _traitContributions[traitKey] = traitContributions;
                }

                var traitStats = _traitStats
                    .Where(t => t.Name == unitTrait.Trait.Name)
                    .ToList();
                if (traitStats == null || traitStats.Count == 0) continue;

                int minTraitStat = traitStats.Min(ts => ts.NumUnits);
                int increment = isAdding ? 1 : -1;
                bool traitContributes = traitStats.Any(ts => ts.NumUnits == traitContributions.count + increment);

                // If the unit is being added, counts are increased; otherwise, they are decreased. Trait contributions are re-evaluated.
                if (isAdding)
                {
                    traitContributions.units.Add(unit);
                    traitContributions.count++;
                    if (traitContributes)
                    {
                        traitContributions.isContributing = true;
                    }
                    else
                    {
                        if (traitContributions.count >= minTraitStat)
                        {
                            traitContributions.isContributing = false;
                        }
                        else
                        {
                            traitContributions.isContributing = null;
                        } 
                    }
                }
                else
                {
                    traitContributions.units.Remove(unit);
                    traitContributions.count--;
                    if (traitContributions.count < minTraitStat)
                    {
                        traitContributions.isContributing = null;
                    }
                    else
                    {
                        traitContributions.isContributing = traitContributes;
                    }
                }

                _traitContributions[traitKey] = traitContributions;
            }
        }


        /// <summary>
        /// Finds the least contributing unit from the current team based on various traits and scores.
        /// </summary>
        /// <param name="nextBestUnit">Unit considered for determining potential trait contribution improvement.</param>
        private PersistedUnit? GetLeastContributingUnit(PersistedUnit? nextBestUnit)
        {
            // Priority queue to store traits and associated units by priority of removal
            var traitsToRemoveUnitFrom = new PriorityQueue<(string trait, List<PersistedUnit> units), int>(Comparer<int>.Create((a, b) => b - a));
            // Set of traits that should not be removed to preserve important synergies
            var traitsToNotRemoveFrom = new HashSet<string>();
            // Set of traits that should be prioritised for removal
            var priorityTraitsToRemove = new HashSet<string>();
            PersistedUnit? unitToRemove = null;
            var worstScore = double.MaxValue;

            foreach (var kvp in _traitContributions)
            {
                var (trait, (traitCount, isContributing, units)) = kvp;

                // Skip if the trait is currently contributing to a tier
                if (isContributing ?? true) continue;

                // Check if not removing from a trait or adding a unit will contribute to a tier of the trait.
                var traitEqualsATier = _traitStats.Any(ts => ts.InGameKey == trait && ts.NumUnits == traitCount);
                var adding1MakesATier = _traitStats.Any(ts => ts.InGameKey == trait && ts.NumUnits == traitCount + 1);
                if (traitEqualsATier)
                {
                    traitsToNotRemoveFrom.Add(trait);
                } else if (adding1MakesATier && nextBestUnit != null && nextBestUnit.Traits.Any(ut => ut.Trait.InGameKey == trait))
                {
                    // If adding one more unit to this trait would complete a tier, update the priority traits to remove for the units other traits.
                    UpdatePriorityTraitsToRemove(nextBestUnit, priorityTraitsToRemove);
                    // If no other conditions are met for removing a unit. One should be removed from this trait.
                    traitsToRemoveUnitFrom.Enqueue((trait, units), int.MinValue + traitCount);
                }
                else
                {
                    // If a trait is non contributing, it can be added to the queue for potential removal. Evaluated by lowest traitCount (trait tier)
                    traitsToRemoveUnitFrom.Enqueue((trait, units), traitCount);
                }
            }
            // Calulate the best unit to remove from the least contributing trait.
            if (traitsToRemoveUnitFrom.TryDequeue(out var traitToRemoveUnitFrom, out var priority))
            {
                foreach(var unit in traitToRemoveUnitFrom.units)
                {
                    double score = CalculateUnitRemovalScore(unit, traitsToNotRemoveFrom, priorityTraitsToRemove);
                    if (score < worstScore)
                    {
                        worstScore = score;
                        unitToRemove = unit;
                    }
                }
            }
            return unitToRemove;  
        }

        /// <summary>
        /// Updates the priority traits to remove when evaluating unit removal.
        /// </summary>
        /// <param name="nextBestUnit">Unit considered for determining potential trait contribution improvement.</param>
        /// <param name="priorityTraitsToRemove">A set of traits that should be prioritised for removal.</param>
        private void UpdatePriorityTraitsToRemove(PersistedUnit nextBestUnit, HashSet<string> priorityTraitsToRemove)
        {
            foreach (var unitTrait in nextBestUnit.Traits)
            {
                var traitKey = unitTrait.Trait.InGameKey;
                if (!_traitContributions.TryGetValue(traitKey, out var traitContribution)) continue;

                if (!_traitStats.Any(ts => ts.InGameKey == traitKey && ts.NumUnits != traitContribution.count + 1))
                {
                    priorityTraitsToRemove.Add(traitKey);
                }
            }
        }

        /// <summary>
        /// Calculates the score for removing a specific unit based on its contribution to traits and the team's overall performance.
        /// </summary>
        /// <param name="unit">The unit being considered for removal.</param>
        /// <param name="traitsToNotRemoveFrom">A set of traits that should not be affected by unit removal.</param>
        /// <param name="priorityTraitsToRemove">A set of traits that are prioritised for removal.</param>
        /// <returns>The calculated removal score for the unit.</returns>
        private double CalculateUnitRemovalScore(PersistedUnit unit, HashSet<string> traitsToNotRemoveFrom, HashSet<string> priorityTraitsToRemove)
        {
            // Sum of co-occurrence rates between the current unit and the remaining team units
            double score = _team.Sum(teamUnit => {
                    if (!_unitCooccurrence.TryGetValue(teamUnit.InGameKey, out var coOccurrence)) return 0;
                    return coOccurrence.GetValueOrDefault(unit.InGameKey, 0);
            });
            // Adjust score based on the unit's tier, adding a slight penalty for higher tier units.
            score *= 1 + (unit.Tier / 10);
            // If the unit belongs to a trait that should not be removed, return maximum score to avoid removing it
            if (unit.Traits.Any(ut => traitsToNotRemoveFrom.Contains(ut.Trait.InGameKey)))
            {
                return double.MaxValue; 
            }
            // If the unit belongs to a trait that is prioritised for removal, halve the score to make it more likely to be selected
            if (unit.Traits.Any(ut => priorityTraitsToRemove.Contains(ut.Trait.InGameKey)))
            {
                score /= 2;
            }
            return score;
        }

        /// <summary>
        /// Finds the best unit from a list of available units to be added to the team.
        /// </summary>
        /// <param name="availableUnits">List of available units.</param>
        /// <param name="initialUnit">The initial unit to base the team on.</param>
        /// <returns>The best unit to replace the initial unit.</returns>
        private PersistedUnit? FindBestUnit(List<PersistedUnit> availableUnits, PersistedUnit initialUnit)
        {
            PersistedUnit? bestUnit = null;
            var bestScore = double.MinValue;
            foreach (var candidateUnit in availableUnits)
            {
                if (_excludedUnits.Contains(candidateUnit.InGameKey)) continue;
                var score = CalculateBestUnitScore(initialUnit, candidateUnit);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestUnit = candidateUnit;
                }
            }
            return bestUnit;
        }

        /// <summary>
        /// Calculates the score of a candidate unit to determine its score if it were to be added.
        /// </summary>
        /// <param name="initialUnit">The initial unit to base the team on.</param>
        /// <param name="candidateUnit">The candidate unit being considered for addition.</param>
        /// <returns>The score for the candidate unit.</returns>
        private double CalculateBestUnitScore(PersistedUnit initialUnit, PersistedUnit candidateUnit)
        {         
            var score = 0.0;
            foreach (var member in _team)
            {
                // Check if the current team member has co-occurrence data
                if (!_unitCooccurrence.ContainsKey(member.InGameKey)) continue;

                // Retrieve co-occurrence value for the candidate unit
                if (_unitCooccurrence[member.InGameKey].TryGetValue(candidateUnit.InGameKey, out var cooccurrence))
                {
                    // Double the co-occurrence score if the member is the initial unit
                    if (member == initialUnit)
                    {
                        cooccurrence *= 2;
                    }
                    score += cooccurrence;
                }
            }

            // Evaluate traits of the candidate unit and adjust the score accordingly
            foreach (var unitTrait in candidateUnit.Traits)
            {
                var traitCount = _traitContributions.GetValueOrDefault(unitTrait.Trait.InGameKey, (0, false, [])).count;
                var traitStats = _traitStats.Where(t => t.Name == unitTrait.Trait.Name && t.NumUnits > traitCount).ToList();

                // If no relevant trait stats are found, return a negative infinity score (At max tier already)
                if (traitStats == null || traitStats.Count == 0)
                    return double.NegativeInfinity;

                // Adjust score based on the traits that complete a new tier
                foreach (var traitStat in traitStats)
                {
                    if (traitCount + 1 == traitStat.NumUnits)
                    {
                        score *= (1 + traitStat.Stat.Top4 / traitStat.Stat.Games);
                    }
                }
            }

            // Retrieve the statistics for the candidate unit and adjust the score based on its performance
            var candidateUnitStat = _unitStats.FirstOrDefault(us => us.InGameKey == candidateUnit.InGameKey);
            if (candidateUnitStat != null)
            {
                score *= (1 + (candidateUnitStat.Stat.Top4 / candidateUnitStat.Stat.Games));
            }
            return score;
        }

        /// <summary>
        /// Creates a guide for a specific unit, including emblem, items, traits, and play style information.
        /// </summary>
        /// <param name="initialUnit">The initial unit to build the guide around.</param>
        /// <param name="emblem">Optional emblem to be added to the guide.</param>
        /// <returns>A guide object containing information for gameplay.</returns>
        private UserGuide CreateGuide(PersistedUnit initialUnit, HexItem? emblem)
        {
            var guide = InitializeGuide(initialUnit);
            if(emblem != null)
            {
                AddEmblemToGuide(guide, emblem);
            }
            AddItemsToHexes(guide,initialUnit);
            UpdateTraitsInGuide(guide);
            UpdateAugmentsInGuide(guide);
            (guide.PlayStyle, guide.DifficultyLevel) = CalculatePlayStyleAndDifficulty(guide);
            // Position units on the board
            guide.Hexes = new PositionSelector().CalculateUnitPositions(guide.Hexes);
            return guide;
        }

        /// <summary>
        /// Initializes a new guide for a specific unit.
        /// </summary>
        /// <param name="initialUnit">The initial unit for which the guide is being created.</param>
        /// <returns>A new guide object.</returns>
        private UserGuide InitializeGuide(PersistedUnit initialUnit)
        {
            var unit = _context.Units.FirstOrDefault(u => u.InGameKey == initialUnit.InGameKey);
            var guideName = unit?.Name ?? "";
            var patch = _configuration["TFT:Patch"] ?? string.Empty;

            return new UserGuide
            {
                InitialUnit = unit,
                Name = guideName,
                Patch = patch,
                Hexes = _team.Select((unit, index) => new Hex
                {
                    Unit = _units.First(u => u.InGameKey == unit.InGameKey),
                    Coordinates = index,
                    IsStarred = false,
                    CurrentItems = []
                }).ToList(),
                IsAutoGenerated = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,   
            };
        }

        /// <summary>
        /// Selects the emblem that is most frequently used across matches.
        /// </summary>
        /// <returns>The emblem item, or null if no emblem should be selected.</returns>
        private HexItem? SelectEmblem()
        {
            var mostUsedEmblem = GetMostUsedEmblem();
            if (mostUsedEmblem == null || !_emblemToTrait.TryGetValue(mostUsedEmblem, out var traitKey)) return null;

            var emblemUsagePercentage = CalculateEmblemUsagePercentage();
            // Return an emblem only if emblems are used at a high rate in the matches.
            if (emblemUsagePercentage < 60) return null;

            var emblemItem = _context.Items.FirstOrDefault(x => x.InGameKey == mostUsedEmblem);
            if (emblemItem == null) return null;

            return new HexItem { Item = emblemItem };
        }

        /// <summary>
        /// Adds an emblem item to the unit with the highest emblem rate in the guide.
        /// </summary>
        /// <param name="guide">The guide to which the emblem is added.</param>
        /// <param name="emblem">The emblem item to be added.</param>
        private void AddEmblemToGuide(UserGuide guide, HexItem emblem)
        {
            var unitWithHighestEmblemRate = GetUnitWithHighestEmblemRate(emblem);
            if (unitWithHighestEmblemRate == null) return;

            var hex = guide.Hexes.FirstOrDefault(h => h.Unit == unitWithHighestEmblemRate);
            if (hex == null) return;

            hex.CurrentItems.Add(emblem);
        }

        /// <summary>
        /// Adds items to the units included in the guide based on unit item rates.
        /// </summary>
        /// <param name="guide">The guide for which items are being added.</param>
        /// <param name="initialUnit">The initial unit for which the guide is being created.</param>
        private void AddItemsToHexes(UserGuide guide, PersistedUnit initialUnit)
        {
            // Maximum number of item slots available 
            var itemSlots = 10;
            // Sort team units based on their three item rates, placing the initial unit at the top
            var sortedUnits = _team.OrderByDescending(unit => _unitThreeItemCounts.TryGetValue(unit.InGameKey, out int threeItemCount) ? threeItemCount : 0).ToList();
            sortedUnits.Remove(initialUnit);
            sortedUnits.Insert(0, initialUnit);

            foreach (var unit in sortedUnits)
            {
                var hex = guide.Hexes.FirstOrDefault(h => h.Unit == unit);
                if (hex == null || !_unitItemRates.TryGetValue(unit.InGameKey, out var unitItemRate)) continue;

                // Order items for the unit by occurrence rate
                var unitItems = unitItemRate
                    .OrderByDescending(kv => kv.Value.occurrenceRate)
                    .ToList();

                // Calculate how many items can be added to the hex
                var itemsToAdd = Math.Min(itemSlots, 3 - hex.CurrentItems.Count);
                foreach (var item in unitItems)
                {
                    if (item.Key.Contains("Emblem")) continue;

                    if (itemsToAdd <= 0) break;

                    var existingItem = _context.Items.FirstOrDefault(x => x.InGameKey == item.Key);
                    if (existingItem == null) continue;

                    // Handle cases where the average amount per unit is greater than 1.5 (Meaning its optimal to have multiple of the same item)
                    if (item.Value.avgAmountPerUnit > 1.5)
                    {
                        RemoveExcessItems(hex, unitItemRate, ref itemSlots, ref itemsToAdd);
                        AddItemToHex(hex, existingItem, ref itemSlots, ref itemsToAdd, 2);
                        continue;
                    }

                    // Skip adding the current item if certain conditions are met
                    if (existingItem.Name.Contains("Gloves") && (hex.CurrentItems.Count > 0 || unit == initialUnit) ||
                        hex.CurrentItems.Any(i => i.Item.InGameKey.Contains("Gloves")) ||
                        hex.CurrentItems.Any(i => i.Item.InGameKey == item.Key))
                    {
                        continue;
                    }
                    AddItemToHex(hex, existingItem, ref itemSlots, ref itemsToAdd);
                }
                
            }
        }

        /// <summary>
        /// Removes excess items from a hex to maintain optimal item distribution.
        /// </summary>
        /// <param name="hex">The hex from which items are removed.</param>
        /// <param name="unitItemRate">The item rates for the unit.</param>
        /// <param name="itemSlots">The available item slots.</param>
        /// <param name="itemsToAdd">The number of items to be added.</param>
        private static void RemoveExcessItems(Hex hex, Dictionary<string, (int occurrenceRate, double avg)> unitItemRate, ref int itemSlots, ref int itemsToAdd)
        {
            while (hex.CurrentItems.Count > 1)
            {
                var itemToRemove = hex.CurrentItems.OrderBy(i => unitItemRate[i.Item.InGameKey].occurrenceRate).FirstOrDefault();
                if (itemToRemove == null) break;

                hex.CurrentItems.Remove(itemToRemove);
                itemSlots++;
                itemsToAdd++;
            }
        }


        /// <summary>
        /// Adds an item to a specific hex in the guide.
        /// </summary>
        /// <param name="hex">The hex to which the item is added.</param>
        /// <param name="item">The item to be added to the hex.</param>
        /// <param name="itemSlots">The available item slots.</param>
        /// <param name="itemsToAdd">The number of items to be added.</param>
        /// <param name="count">The count of the item to add, defaults to 1.</param>
        private static void AddItemToHex(Hex hex, PersistedItem item, ref int itemSlots, ref int itemsToAdd, int count = 1)
        {
            for (int i = 0; i < count && itemsToAdd > 0; i++)
            {
                hex.CurrentItems.Add(new HexItem { Item = item });
                itemSlots--;
                itemsToAdd--;
            }
        }

        /// <summary>
        /// Calculates the play style and difficulty level of a guide based on unit tiers and items.
        /// </summary>
        /// <param name="guide">The guide for which play style and difficulty are calculated.</param>
        /// <returns>A tuple containing the play style and difficulty level.</returns>
        private (string PlayStyle, string Difficulty) CalculatePlayStyleAndDifficulty( UserGuide guide)
        {
            var tierCounts = new Dictionary<int, int>();
            var totalTierCounts = new Dictionary<int, int>();
            foreach (var unit in _team)
            {
                var existingUnit = _context.Units.FirstOrDefault(u => u.InGameKey == unit.InGameKey);
                if (existingUnit == null) continue;

                var unitTier = existingUnit.Tier;
                var itemCount = guide.Hexes.FirstOrDefault(h => h.Unit == unit)?.CurrentItems.Count ?? 0;

                // Count units with 3 items
                if (itemCount == 3)
                {
                    if (!tierCounts.ContainsKey(unitTier))
                        tierCounts[unitTier] = 0;
                    tierCounts[unitTier]++;
                }

                // Count total units of tiers 4 and 5
                if (unitTier == 4 || unitTier == 5)
                {
                    if (!totalTierCounts.ContainsKey(unitTier))
                        totalTierCounts[unitTier] = 0;
                    totalTierCounts[unitTier]++;
                }
            }

            string playStyle;
            string difficulty;

            // Determine play style and difficulty based on tier counts
            if ((tierCounts.GetValueOrDefault(5, 0) >= 1) || (totalTierCounts.GetValueOrDefault(5, 0) > 2))
            {
                playStyle = "Fast 9";
                difficulty = "Hard";
            }
            else if ((tierCounts.GetValueOrDefault(4, 0) >= 2) || (tierCounts.GetValueOrDefault(4, 0) >= 1 && totalTierCounts.GetValueOrDefault(4, 0) >= 3))
            {
                playStyle = "Fast 8";
                difficulty = "Hard";
            }
            else if (tierCounts.GetValueOrDefault(3, 0) >= 2)
            {
                playStyle = "Level 7 Slow Roll";
                difficulty = "Medium";
            }
            else if (tierCounts.GetValueOrDefault(2, 0) >= 2)
            {
                playStyle = "Level 6 Slow Roll";
                difficulty = "Easy";
            }
            else if (tierCounts.GetValueOrDefault(1, 0) >= 2)
            {
                playStyle = "Level 5 Slow Roll";
                difficulty = "Easy";
            }
            else
            {
                playStyle = "Default";
                difficulty = "Default";
            }

            return (playStyle, difficulty);
        }

        /// <summary>
        /// Updates the traits of a guide based on the current team and contributions.
        /// </summary>
        /// <param name="guide">The guide whose traits are being updated.</param>
        private void UpdateTraitsInGuide(UserGuide guide)
        {
            guide.Traits = [.._traitContributions.Select(ct => new GuideTrait
            {
                Trait = _traits.First(t => t.InGameKey == ct.Key),
                Value = ct.Value.count,
                Tier = _traitStats
                    .Where(ts => ts.InGameKey == ct.Key && ts.NumUnits <= ct.Value.count)
                    .OrderByDescending(ts => ts.NumUnits)
                    .FirstOrDefault()?.NumUnits ?? 0
            })];
        }

        /// <summary>
        /// Updates the augments of a guide based on the top augments used across the current team.
        /// </summary>
        /// <param name="guide">The guide whose augments are being updated.</param>
        private void UpdateAugmentsInGuide(UserGuide guide)
        {
            var topAugments = new Dictionary<string, int>();
            var itemKeys = guide.Hexes
                                 .SelectMany(h => h.CurrentItems)
                                 .Select(ci => ci.Item.InGameKey)
                                 .ToList();

            foreach (var unit in _team)
            {
                if (!_augmentPlayRates.TryGetValue(unit.InGameKey, out var augmentStat)) continue;
                foreach (var augment in augmentStat)
                {
                    // If augment contains carry, and the unit that uses the augment isnt in the team; skip.
                    if (augment.Key.Contains("Carry"))
                    {
                        var remainingString = RemovePrefixAndSuffix(augment.Key, "_Augment_").Replace("Carry", "");
                        var hex = guide.Hexes.FirstOrDefault(h => h.Unit.Key == remainingString);
                        if (hex == null || hex.CurrentItems.Count != 3) continue;
                    }
                    // If augment contains Crest or Crown (Emblems). Skip it if that emblem isnt in use.
                    if (augment.Key.Contains("Crest") || augment.Key.Contains("Crown"))
                    {
                        var remainingString = RemovePrefixAndSuffix(augment.Key, "_Augment_").Replace("Crest", "").Replace("Crown", "");
                        if (!itemKeys.Any(e => e.Contains(remainingString))) continue;
                    }

                    if (!topAugments.ContainsKey(augment.Key))
                    {
                        topAugments[augment.Key] = 0;
                    }
                    topAugments[augment.Key] += augment.Value;
                }
            }
            // Get the top 3 augments based on their counts
            var top3Augments = topAugments.OrderByDescending(a => a.Value).Take(3).Select(a => a.Key).ToList();
            var augmentEntities = _context.Augments.Where(a => top3Augments.Contains(a.InGameKey)).ToList();

            guide.Augments = [..augmentEntities.Select(a => new GuideAugment
            {
                Augment = a
            })];
        }

        /// <summary>
        /// Calculates the usage percentage of emblems across all matches.
        /// </summary>
        /// <returns>The emblem usage percentage.</returns>
        private double CalculateEmblemUsagePercentage()
        {
            var totalEmblemCount = _emblems.Values.Sum(dic => dic.Values.Sum());
            return (double)totalEmblemCount / _matchCount * 100;
        }

        /// <summary>
        /// Gets the most used emblem across all matches.
        /// </summary>
        /// <returns>The key of the most used emblem, or null if no suitable emblem is found.</returns>
        private string? GetMostUsedEmblem()
        {
            // Group emblems by their keys and sum their usage counts
            var mostUsedEmblems = _emblems.Values
                .SelectMany(dic => dic)
                .GroupBy(kv => kv.Key)  
                .Select(g => new KeyValuePair<string, int>(g.Key, g.Sum(kv => kv.Value)))  
                .OrderByDescending(kv => kv.Value)
                .ToList();

            // Find the most used emblem that can be added and has an associated trait
            foreach (var emblem in mostUsedEmblems)
            {
                if (!CanAddEmblem(emblem.Key)) continue;
                if (!_emblemToTrait.TryGetValue(emblem.Key, out var traitKey)) continue;
                return emblem.Key;
            }
            return null;
        }

        /// <summary>
        /// Checks if an emblem item can be added based on the trait and current team configuration.
        /// </summary>
        /// <param name="itemKey">The key of the emblem item to be checked.</param>
        /// <returns>True if the emblem can be added, false otherwise.</returns>
        private bool CanAddEmblem(string itemKey)
        {
            // Check if the item key is an emblem
            if (!itemKey.Contains("Emblem"))
                return false;

            if(!_emblemToTrait.TryGetValue(itemKey, out var traitKey)) return false;

            // Check for a matching trait statistic thats tier is greater than 2, and the emblem contributes to a tier.
            var matchingTraitStat = _traitStats.FirstOrDefault(ts =>
                ts.InGameKey == traitKey &&
                _traitContributions.ContainsKey(ts.InGameKey) &&
                _traitContributions[ts.InGameKey].count + 1 > 2 &&
                _traitContributions[ts.InGameKey].count + 1 == ts.NumUnits);

            return matchingTraitStat != null;
        }

        /// <summary>
        /// Finds the unit with the highest emblem rate to add an emblem to it.
        /// </summary>
        /// <param name="emblem">The emblem item to be added.</param>
        /// <returns>The unit with the highest emblem rate, or null if no suitable unit is found.</returns>
        private PersistedUnit? GetUnitWithHighestEmblemRate(HexItem emblem)
        {
            var unitWithHighestEmblemRate = _team
                .OrderByDescending(unit => _unitItemRates.TryGetValue(unit.InGameKey, out Dictionary<string, (int occurrenceRate, double avgAmountPerUnit)>? unitCounts) 
                && unitCounts.TryGetValue(emblem.Item.InGameKey, out var itemRate) ? itemRate.occurrenceRate : 0)
                .FirstOrDefault();
            return unitWithHighestEmblemRate;
        }

        /// <summary>
        /// Removes the prefix and suffix from a given string based on a provided suffix.
        /// </summary>
        /// <param name="key">The key from which the prefix and suffix are removed.</param>
        /// <param name="suffix">The suffix to be removed from the key.</param>
        /// <returns>The string after removing the prefix and suffix.</returns>
        private static string RemovePrefixAndSuffix(string key, string suffix)
        {
            string escapedSuffix = Regex.Escape(suffix);
            string pattern = $"^\\D*\\d*{escapedSuffix}";
            return Regex.Replace(key, pattern, string.Empty);
        }
    }
}