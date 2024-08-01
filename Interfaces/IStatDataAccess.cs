using TFT_API.Models.Item;
using TFT_API.Models.Stats.AugmentStats;
using TFT_API.Models.Stats.CompStats;
using TFT_API.Models.Stats.ItemStats;
using TFT_API.Models.Stats.TraitStats;
using TFT_API.Models.Stats.UnitStats;

namespace TFT_API.Interfaces
{
    public interface IStatDataAccess
    {
        BaseUnitStat? GetUnitStats();
        BaseItemStat? GetItemStats();
        BaseTraitStat? GetTraitStats();
        BaseAugmentStat? GetAugmentStats();
        BaseCompStat? GetCompStats();
    }
}
