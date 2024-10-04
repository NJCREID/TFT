using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Augments;
using TFT_API.Models.FetchedTFTData;
using TFT_API.Models.FetchResponse;
using TFT_API.Models.Item;
using TFT_API.Models.Trait;
using TFT_API.Models.Unit;
using Trait = TFT_API.Models.FetchedTFTData.Trait;

namespace TFT_API.Services
{
    public partial class TFTUpdateDataService(ITFTDataService TFTDataService, IMapper mapper, TFTContext context, IConfiguration configuration, IMemoryCache memoryCache)
    {
        private readonly TFTContext _context = context;
        private readonly ITFTDataService _TFTDataService = TFTDataService;
        private readonly IMapper _mapper = mapper;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly string _url = $"https://tft.dakgg.io/api/v1/data/{{type}}?hl=en&season=set{configuration["TFT:Set"]}";
        private readonly string _baseImageDirectory = $"wwwroot/images/set{configuration["TFT:Set"]}";

        [GeneratedRegex(@"\bCrit\b(?! Chance)")]
        private static partial Regex MyRegex();

        [GeneratedRegex("[^a-zA-Z0-9]")]
        private static partial Regex MyRegex1();

        public async Task UpdateSetDataAsync()
        {
            await FetchAndSaveAugmentsAsync();
            await FetchAndSaveChampionsAsync();
            await FetchAndSaveItemsAsync();
            await FetchAndSaveTraitsAsync();
            ClearCacheEntries();
        }

        private void ClearCacheEntries()
        {
            _memoryCache.Remove("augments");
            _memoryCache.Remove("units_full");
            _memoryCache.Remove("units_partial");
            _memoryCache.Remove("items_full");
            _memoryCache.Remove("items_partial");
            _memoryCache.Remove("traits");
        }

        private async Task FetchAndSaveChampionsAsync()
        {
            var url = _url.Replace("{type}", "champions");
            var champions = await _TFTDataService.FetchDataAsync<Champion>(url, "champions");

            var splashDirectory = _baseImageDirectory + "/champions/splash";
            var tileDirectory = _baseImageDirectory + "/champions/tiles";
            var skillDirectory = _baseImageDirectory + "/champions/skill";

            await _TFTDataService.SaveImagesAsync(champions,
                champion => $"https://cdn.metatft.com/file/metatft/championsplashes/{champion.IngameKey.ToLower()}.png", champion => champion.IngameKey, splashDirectory);

            await _TFTDataService.SaveImagesAsync(champions,
                champion => champion.ImageUrl, champion => champion.IngameKey, tileDirectory);

            await _TFTDataService.SaveImagesAsync(champions.Select(c => c.Skill).ToList(),
                         skill => skill.ImageUrl, skill => MyRegex1().Replace(skill.Name, ""), skillDirectory);

            foreach (var champion in champions)
            {
                champion.Skill.ImageUrl = $"/images/champions/skill/{MyRegex1().Replace(champion.Skill.Name, "")}.avif";
            }

            var persistedUnits = ProcessChampions(champions);

            foreach (var unit in persistedUnits)
            {
                var existingUnit = await _context.Units
                    .Include(u => u.Traits)
                    .Include(u => u.Skill)
                    .FirstOrDefaultAsync(u => u.InGameKey == unit.InGameKey);
                if (existingUnit != null)
                {
                    _mapper.Map(unit, existingUnit);
                }
                else
                {
                    await _context.Units.AddAsync(unit);
                }
            }
            await _context.SaveChangesAsync();
        }

        private async Task FetchAndSaveItemsAsync()
        {
            var url = _url.Replace("{type}", "items");
            var items = await _TFTDataService.FetchDataAsync<Item>(url, "items");

            var imageDirecectory = _baseImageDirectory + "/items";

            await _TFTDataService.SaveImagesAsync(items, item => item.ImageUrl, item => item.IngameKey, imageDirecectory);

            var persistedItems = ProcessItems(items);

            foreach (var item in persistedItems)
            {
                var existingItem = await _context.Items.FirstOrDefaultAsync(i => i.InGameKey == item.InGameKey);
                if (existingItem != null)
                {
                    _mapper.Map(item, existingItem);
                }
                else
                {
                    await _context.Items.AddAsync(item);
                }
            }
            await _context.SaveChangesAsync();
        }

        private async Task FetchAndSaveAugmentsAsync()
        {
            var url = _url.Replace("{type}", "augments");
            var augments = await _TFTDataService.FetchDataAsync<Augment>(url, "augments");

            var imageDirecectory = _baseImageDirectory + "/augments";

            await _TFTDataService.SaveImagesAsync(augments, augment => augment.ImageUrl, augment => augment.IngameKey, imageDirecectory);

            var persistedAugments = ProcessAugments(augments).Where(a => a.InGameKey != null);

            foreach (var augment in persistedAugments)
            {
                var existingAugment = await _context.Augments.FirstOrDefaultAsync(a => a.InGameKey == augment.InGameKey);
                if (existingAugment != null)
                {
                    _mapper.Map(augment, existingAugment);
                }
                else
                {
                    await _context.Augments.AddAsync(augment);
                }
            }
            await _context.SaveChangesAsync();
        }

        private async Task FetchAndSaveTraitsAsync()
        {
            var url = _url.Replace("{type}", "traits");
            var traits = await _TFTDataService.FetchDataAsync<Trait>(url, "traits");

            var imageDirectory = _baseImageDirectory + "/traits";

            await _TFTDataService.SaveImagesAsync(traits, trait => trait.ImageUrl, trait => trait.IngameKey, imageDirectory);

            var persistedTraits = ProcessTraits(traits);

            foreach (var trait in persistedTraits)
            {
                var existingTrait = await _context.Traits
                    .Include(t => t.Tiers)
                    .FirstOrDefaultAsync(t => t.InGameKey == trait.InGameKey);
                if (existingTrait != null)
                {
                    _mapper.Map(trait, existingTrait);
                }
                else
                {
                    await _context.Traits.AddAsync(trait);
                }
            }
            await _context.SaveChangesAsync();
        }

        private List<PersistedUnit> ProcessChampions(List<Champion> champions)
        {
            return champions
                .Where(champion => !champion.IsHiddenGuide.GetValueOrDefault())
                .Select(champion => _mapper.Map<PersistedUnit>(champion)).ToList();
        }

        private List<PersistedItem> ProcessItems(List<Item> items)
        {
            List<string> tagOrder = [ "fromitem", "normal", "radiant", "artifact", "support", "unique" ];

            return  [..items.Select(item =>
            {
                item.FromDesc = ProcessDescription(item.FromDesc);
                if (item.Tags == null || item.Tags.Count == 0)
                {
                    item.Tags = item.Desc.Contains("Support item") ? ["support"] : ["artifact"];
                }
                if(item.Compositions != null && item.Compositions.Count > 0){
                    item.Compositions = item.Compositions
                    .Select(comp =>
                    {
                        var matchingItem = items.FirstOrDefault(i => i.Key == comp);
                        return matchingItem != null ? matchingItem.IngameKey : comp;
                    })
                    .ToList();
                }
                return _mapper.Map<PersistedItem>(item);
            })
            .Where(item => item != null)
            .OrderBy(item => tagOrder.IndexOf(item.Tags.FirstOrDefault() ?? "unique"))];
        }

        private List<PersistedAugment> ProcessAugments(List<Augment> augments)
        {
            return _mapper.Map<List<PersistedAugment>>(augments);
        }

        private List<PersistedTrait> ProcessTraits(List<Trait> traits)
        {
            var persistedTraits = _mapper.Map<List<PersistedTrait>>(traits);
            foreach (var trait in persistedTraits)
            {
                foreach (var tier in trait.Tiers)
                {
                    if(trait.Stats.TryGetValue(tier.Level.ToString(), out var desc))
                    {
                        tier.Desc = desc;
                    } else
                    {
                        tier.Desc = "Unknown";
                    }
                }
            }
            return persistedTraits;
        }

        private static string ProcessDescription(string desc)
        {
            if (string.IsNullOrEmpty(desc)) return desc;

            desc = desc.Replace("Helath", "Health")
                        .Replace("Critical Strike", "Crit")
                        .Replace("Resistance", "Resist")
                        .Replace("Amor", "Armor")
                        .Replace("Crit.", "Crit");
            desc = MyRegex().Replace(desc, "Crit Chance");
            return desc;
        }

        
    }
}
