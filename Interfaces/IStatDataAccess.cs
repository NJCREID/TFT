using TFT_API.Models.Item;
using TFT_API.Models.Stats.AugmentStats;
using TFT_API.Models.Stats.CompStats;
using TFT_API.Models.Stats.CoOccurrence;
using TFT_API.Models.Stats.ItemStats;
using TFT_API.Models.Stats.TraitStats;
using TFT_API.Models.Stats.UnitStats;

namespace TFT_API.Interfaces
{
    public interface IStatDataAccess
    {
        Task<BaseUnitStatDto?> GetUnitStatsAsync(string league);
        Task<BaseItemStatDto?> GetItemStatsAsync(string league);
        Task<BaseTraitStatDto?> GetTraitStatsAsync(string league);
        Task<BaseAugmentStatDto?> GetAugmentStatsAsync(string league);
        Task<BaseCompStatDto?> GetCompStatsAsync(string league);
        Task<BaseCoOccurrenceDto?> GetCoOccurrenceStatsAsync(string type, string key);
    }
}
