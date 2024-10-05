using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;
using System.Text.RegularExpressions;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Augments;
using TFT_API.Models.FetchedTFTData;
using TFT_API.Models.Item;
using TFT_API.Models.Trait;
using TFT_API.Models.Unit;
using Trait = TFT_API.Models.FetchedTFTData.Trait;

namespace TFT_API.Services
{
    /// <summary>
    /// A service for updating the TFT set data.
    /// </summary>
    public partial class TFTUpdateDataService(ITFTDataService TFTDataService, IMapper mapper, TFTContext context, IConfiguration configuration, IMemoryCache memoryCache)
    {
        private readonly TFTContext _context = context;
        private readonly ITFTDataService _TFTDataService = TFTDataService;
        private readonly IMapper _mapper = mapper;
        private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly string _url = $"https://tft.dakgg.io/api/v1/data/{{type}}?hl=en&season=set{configuration["TFT:Set"]}";
        private readonly string _baseImageDirectory = $"wwwroot/images/set{configuration["TFT:Set"]}";

        /// <summary>
        /// A compiled regular expression to match the word 'Crit' not followed by 'Chance'.
        /// </summary>
        [GeneratedRegex(@"\bCrit\b(?! Chance)")]
        private static partial Regex MyRegex();

        /// <summary>
        /// A compiled regular expression to match non-alphanumeric characters.
        /// </summary>
        [GeneratedRegex("[^a-zA-Z0-9]")]
        private static partial Regex MyRegex1();

        /// <summary>
        /// Updates the set data by fetching augments, champions, items, and traits from the external service, and clears related cache entries.
        /// </summary>
        public async Task UpdateSetDataAsync()
        {
            await FetchAndSaveAugmentsAsync();
            await FetchAndSaveChampionsAsync();
            await FetchAndSaveItemsAsync();
            await FetchAndSaveTraitsAsync();
            ClearCacheEntries();
        }

        /// <summary>
        /// Clears the cache entries for augments, units, items, and traits.
        /// </summary>
        private void ClearCacheEntries()
        {
            _memoryCache.Remove("augments");
            _memoryCache.Remove("units_full");
            _memoryCache.Remove("units_partial");
            _memoryCache.Remove("items_full");
            _memoryCache.Remove("items_partial");
            _memoryCache.Remove("traits");
        }

        /// <summary>
        /// Fetches champion data from the external API, saves their images, processes them, and persists the changes to the database.
        /// </summary>
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


        /// <summary>
        /// Fetches item data from the external API, saves their images, processes them, and persists the changes to the database.
        /// </summary>
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

        /// <summary>
        /// Fetches augment data from the external API, saves their images, processes them, and persists the changes to the database.
        /// </summary>
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

        /// <summary>
        /// Fetches trait data from the external API, saves their images, processes them, and persists the changes to the database.
        /// </summary>
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

        /// <summary>
        /// Processes a list of champions and converts them to a list of persisted units.
        /// </summary>
        /// <param name="champions">The list of champions to process.</param>
        /// <returns>A list of persisted units.</returns>
        private List<PersistedUnit> ProcessChampions(List<Champion> champions)
        {
            return champions
                .Where(champion => !champion.IsHiddenGuide.GetValueOrDefault())
                .Select(champion => _mapper.Map<PersistedUnit>(champion)).ToList();
        }

        /// <summary>
        /// Processes a list of items, updating their tags and descriptions, and converts them to a list of persisted items.
        /// </summary>
        /// <param name="items">The list of items to process.</param>
        /// <returns>A list of persisted items.</returns>
        private List<PersistedItem> ProcessItems(List<Item> items)
        {
            List<string> tagOrder = [ "fromitem", "normal", "radiant", "artifact", "support", "unique" ];

            return  [..items.Select(item =>
            {
                item.FromDesc = ProcessDescription(item.FromDesc);
                var properties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    var value = property.GetValue(item)?.ToString();

                    if (property.Name.StartsWith("Is", StringComparison.OrdinalIgnoreCase) && value != null)
                    {
                        string tag = property.Name.Substring(2).ToLower();
                        if (!item.Tags.Contains(tag))
                        {
                            item.Tags.Add(tag);
                        }
                    }
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

        /// <summary>
        /// Processes a list of augments and converts them to a list of persisted augments.
        /// </summary>
        /// <param name="augments">The list of augments to process.</param>
        /// <returns>A list of persisted augments.</returns>
        private List<PersistedAugment> ProcessAugments(List<Augment> augments)
        {
            return _mapper.Map<List<PersistedAugment>>(augments);
        }

        /// <summary>
        /// Processes a list of traits and converts them to a list of persisted traits, updating their tier descriptions.
        /// </summary>
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

        /// <summary>
        /// Processes a description by correcting common misspellings and normalising certain terms.
        /// </summary>
        /// <param name="desc">The description to process.</param>
        /// <returns>The processed description.</returns>
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
