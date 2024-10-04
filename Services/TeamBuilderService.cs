﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;
using TFT_API.Data;
using TFT_API.Models.Match;
using TFT_API.Models.Stats.TraitStats;
using TFT_API.Models.Stats.UnitStats;
using TFT_API.Models.Trait;
using TFT_API.Models.Unit;
using TFT_API.Models.UserGuides;
using Match = TFT_API.Models.Match.Match;

namespace TFT_API.Services
{
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
        private Dictionary<string, Dictionary<string, int>> _unitItemRates = [];
        private Dictionary<string, Dictionary<string, int>> _augmentPlayRates = [];
        private Dictionary<string, Dictionary<string, int>> _emblems = [];
        private Dictionary<string, int> _unitThreeItemCounts = [];
        private Dictionary<string, (int count, bool? isContributing, List<PersistedUnit> units)> _traitContributions = [];
        private List<PersistedUnit> _team = [];
        private int _matchCount;

        private readonly string[] _excludedUnits = ["TFT11_Kayle"];

        public async Task BuildTeams()
        {
            await LoadData();

            var guides = await GenerateGuides();

            await SaveGuides(guides);

            ClearAutoGuideEntries();
        }

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

        private async Task<List<UserGuide>> GenerateGuides()
        {
            var guides = new List<UserGuide>();

            foreach (var unit in _units)
            {
                if (_excludedUnits.Contains(unit.InGameKey)) continue;
                ResetPrevUnitData();
                var unitMatches = await GetUnitMatches(unit.InGameKey, unit.Key);
                CalculateStatsForMatches(unitMatches);
                _matchCount = unitMatches.Count;

                var guide = BuildTeam(unit);
                guides.Add(guide);
            }

            return guides;
        }

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

        private async Task<List<Match>> GetUnitMatches(string unitInGameKey, string unitKey)
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

        private void ClearAutoGuideEntries()
        {
            _memoryCache.Remove("autogenerated");
        }

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

        private void UpdateItemRates(MatchUnit unit)
        {
            var unitCharacterId = unit.CharacterId;
            if (!_unitItemRates.TryGetValue(unitCharacterId, out var itemCounts))
            {
                itemCounts = [];
                _unitItemRates[unitCharacterId] = itemCounts;
            }

            foreach (var item in unit.ItemNames)
            {
                if (item.Contains("Emblem"))
                {
                    UpdateEmblemCounts(unitCharacterId, item);
                }

                if (!itemCounts.TryGetValue(item, out var itemCount))
                {
                    itemCount = 0;
                    itemCounts[item] = itemCount;
                }

                itemCounts[item] = ++itemCount;
            }
        }

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

        private UserGuide BuildTeam(PersistedUnit initialUnit)
        {
            var availableUnits = _units.Where(u => u.InGameKey != initialUnit.InGameKey).ToList();
            HexItem? selectedEmblem = null;

            _team.Add(initialUnit);
            UpdateTraitContributions(initialUnit);

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

        private void UpdateEmblem(ref HexItem? selectedEmblem)
        {
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

        private PersistedUnit? GetLeastContributingUnit(PersistedUnit? nextBestUnit)
        {
            var traitsToRemoveUnitFrom = new PriorityQueue<(string trait, List<PersistedUnit> units), int>(Comparer<int>.Create((a, b) => b - a));
            var traitsToNotRemoveFrom = new HashSet<string>();
            var priorityTraitsToRemove = new HashSet<string>();
            PersistedUnit? unitToRemove = null;
            var worstScore = double.MaxValue;

            foreach (var kvp in _traitContributions)
            {
                var (trait, (traitCount, isContributing, units)) = kvp;
                if (isContributing ?? true) continue;
                var traitEqualsATier = _traitStats.Any(ts => ts.InGameKey == trait && ts.NumUnits == traitCount);
                var adding1MakesATier = _traitStats.Any(ts => ts.InGameKey == trait && ts.NumUnits == traitCount + 1);
                if (traitEqualsATier)
                {
                    traitsToNotRemoveFrom.Add(trait);
                } else if (adding1MakesATier && nextBestUnit != null && nextBestUnit.Traits.Any(ut => ut.Trait.InGameKey == trait))
                {
                    UpdatePriorityTraitsToRemove(nextBestUnit, priorityTraitsToRemove);
                    traitsToRemoveUnitFrom.Enqueue((trait, units), int.MinValue + traitCount);
                }
                else
                {
                    traitsToRemoveUnitFrom.Enqueue((trait, units), traitCount);
                }
            }
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

        private double CalculateUnitRemovalScore(PersistedUnit unit, HashSet<string> traitsToNotRemoveFrom, HashSet<string> priorityTraitsToRemove)
        {
            double score = _team.Sum(teamUnit => {
                    if (!_unitCooccurrence.TryGetValue(teamUnit.InGameKey, out var coOccurrence)) return 0;
                    return coOccurrence.GetValueOrDefault(unit.InGameKey, 0);
            });
            score *= 1 + (unit.Tier / 10);
            if (unit.Traits.Any(ut => traitsToNotRemoveFrom.Contains(ut.Trait.InGameKey)))
            {
                return double.MaxValue; 
            }
            if (unit.Traits.Any(ut => priorityTraitsToRemove.Contains(ut.Trait.InGameKey)))
            {
                score /= 2;
            }
            return score;
        }

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

        private double CalculateBestUnitScore(PersistedUnit initialUnit, PersistedUnit candidateUnit)
        {         
            var score = 0.0;
            foreach (var member in _team)
            {
                if (!_unitCooccurrence.ContainsKey(member.InGameKey)) continue;

                if (_unitCooccurrence[member.InGameKey].TryGetValue(candidateUnit.InGameKey, out var cooccurrence))
                {
                    if (member == initialUnit)
                    {
                        cooccurrence *= 2;
                    }
                    score += cooccurrence;
                }
            }
            foreach (var unitTrait in candidateUnit.Traits)
            {
                var traitCount = _traitContributions.GetValueOrDefault(unitTrait.Trait.InGameKey, (0, false, [])).count;
                var traitStats = _traitStats.Where(t => t.Name == unitTrait.Trait.Name && t.NumUnits > traitCount).ToList();
                if (traitStats == null || traitStats.Count == 0)
                    return double.NegativeInfinity;

                foreach (var traitStat in traitStats)
                {
                    if (traitCount + 1 == traitStat.NumUnits)
                    {
                        score *= (1 + traitStat.Stat.Top4 / traitStat.Stat.Games);
                    }
                }
            }
            var candidateUnitStat = _unitStats.FirstOrDefault(us => us.InGameKey == candidateUnit.InGameKey);
            if (candidateUnitStat != null)
            {
                score *= (1 + (candidateUnitStat.Stat.Top4 / candidateUnitStat.Stat.Games));
            }
            return score;
        }

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
            guide.Hexes = new PositionSelector().CalculateUnitPositions(guide.Hexes);
            return guide;
        }

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
            };
        }

        private HexItem? SelectEmblem()
        {
            var mostUsedEmblem = GetMostUsedEmblem();
            if (mostUsedEmblem == null || !_emblemToTrait.TryGetValue(mostUsedEmblem, out var traitKey)) return null;

            var emblemUsagePercentage = CalculateEmblemUsagePercentage();
            if (emblemUsagePercentage < 60) return null;

            var emblemItem = _context.Items.FirstOrDefault(x => x.InGameKey == mostUsedEmblem);
            if (emblemItem == null) return null;

            return new HexItem { Item = emblemItem };
        }

        private void AddEmblemToGuide(UserGuide guide, HexItem emblem)
        {
            var unitWithHighestEmblemRate = GetUnitWithHighestEmblemRate(emblem);
            if (unitWithHighestEmblemRate == null) return;

            var hex = guide.Hexes.FirstOrDefault(h => h.Unit == unitWithHighestEmblemRate);
            if (hex == null) return;

            hex.CurrentItems.Add(emblem);
        }

        private void AddItemsToHexes(UserGuide guide, PersistedUnit initialUnit)
        {
            var itemSlots = 10;
            var sortedUnits = _team.OrderByDescending(unit => _unitThreeItemCounts.TryGetValue(unit.InGameKey, out int threeItemCount) ? threeItemCount : 0).ToList();
            sortedUnits.Remove(initialUnit);
            sortedUnits.Insert(0, initialUnit);

            foreach (var unit in sortedUnits)
            {
                var hex = guide.Hexes.FirstOrDefault(h => h.Unit == unit);
                if (hex == null) continue;

                if (!_unitItemRates.TryGetValue(unit.InGameKey, out var unitItemRate)) continue;
                var unitItems = unitItemRate
                    .OrderByDescending(kv => kv.Value)
                    .Select(kv => kv.Key)
                    .ToList();

                var itemsToAdd = Math.Min(itemSlots, 3 - hex.CurrentItems.Count);
                foreach (var itemKey in unitItems)
                {
                    if (itemKey.Contains("Emblem")) continue;

                    if (itemsToAdd <= 0) break;

                    var item = _context.Items.FirstOrDefault(x => x.InGameKey == itemKey);
                    if (item == null) continue;
                    if (item.Name.Contains("Gloves") && (hex.CurrentItems.Count > 0 || unit == initialUnit) ||
                        hex.CurrentItems.Any(item => item.Item.InGameKey.Contains("Gloves")) ||
                        hex.CurrentItems.Any(item => item.Item.InGameKey == itemKey))
                    {
                        continue;
                    }

                    hex.CurrentItems.Add(new HexItem { Item = item });
                    itemSlots--;
                    itemsToAdd--;
                }
            }
        }

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

                if (itemCount == 3)
                {
                    if (!tierCounts.ContainsKey(unitTier))
                        tierCounts[unitTier] = 0;
                    tierCounts[unitTier]++;
                }

                if (unitTier == 4 || unitTier == 5)
                {
                    if (!totalTierCounts.ContainsKey(unitTier))
                        totalTierCounts[unitTier] = 0;
                    totalTierCounts[unitTier]++;
                }
            }

            string playStyle;
            string difficulty;

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
                    if (augment.Key.Contains("Carry"))
                    {
                        var remainingString = RemovePrefixAndSuffix(augment.Key, "_Augment_").Replace("Carry", "");
                        var hex = guide.Hexes.FirstOrDefault(h => h.Unit.Key == remainingString);
                        if (hex == null || hex.CurrentItems.Count != 3) continue;
                    }
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
            var top3Augments = topAugments.OrderByDescending(a => a.Value).Take(3).Select(a => a.Key).ToList();
            var augmentEntities = _context.Augments.Where(a => top3Augments.Contains(a.InGameKey)).ToList();

            guide.Augments = [..augmentEntities.Select(a => new GuideAugment
            {
                Augment = a
            })];
        }

        private double CalculateEmblemUsagePercentage()
        {
            var totalEmblemCount = _emblems.Values.Sum(dic => dic.Values.Sum());
            return (double)totalEmblemCount / _matchCount * 100;
        }

        private string? GetMostUsedEmblem()
        {
            var mostUsedEmblems = _emblems.Values
                .SelectMany(dic => dic)
                .GroupBy(kv => kv.Key)  
                .Select(g => new KeyValuePair<string, int>(g.Key, g.Sum(kv => kv.Value)))  
                .OrderByDescending(kv => kv.Value)
                .ToList();

            foreach (var emblem in mostUsedEmblems)
            {
                if (!CanAddEmblem(emblem.Key)) continue;
                if (!_emblemToTrait.TryGetValue(emblem.Key, out var traitKey)) continue;
                return emblem.Key;
            }
            return null;
        }

        private bool CanAddEmblem(string itemKey)
        {
            if (!itemKey.Contains("Emblem"))
                return false;

            if(!_emblemToTrait.TryGetValue(itemKey, out var traitKey)) return false;
            var matchingTraitStat = _traitStats.FirstOrDefault(ts =>
                ts.InGameKey == traitKey &&
                _traitContributions.ContainsKey(ts.InGameKey) &&
                _traitContributions[ts.InGameKey].count + 1 > 2 &&
                _traitContributions[ts.InGameKey].count + 1 == ts.NumUnits);

            return matchingTraitStat != null;
        }

        private PersistedUnit? GetUnitWithHighestEmblemRate(HexItem emblem)
        {
            var unitWithHighestEmblemRate = _team
                .OrderByDescending(unit => _unitItemRates.TryGetValue(unit.InGameKey, out Dictionary<string, int>? unitCounts) && unitCounts.TryGetValue(emblem.Item.InGameKey, out int unitCount) ? unitCount : 0)
                .FirstOrDefault();
            return unitWithHighestEmblemRate;
        }
        private static string RemovePrefixAndSuffix(string key, string suffix)
        {
            string escapedSuffix = Regex.Escape(suffix);
            string pattern = $"^\\D*\\d*{escapedSuffix}";
            return Regex.Replace(key, pattern, string.Empty);
        }
    }
}