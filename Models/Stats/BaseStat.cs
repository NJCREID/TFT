using TFT_API.Models.Stats.AugmentStats;
using TFT_API.Models.Stats.ItemStats;
using TFT_API.Models.Stats.TraitStats;
using TFT_API.Models.Stats.UnitStats;

namespace TFT_API.Models.Stats
{
    public class BaseStat
    {
        public int Games { get; set; }
        public int Place { get; set; }
        public int Top4 { get; set; }
        public int Win { get; set; }
        public List<UnitStat> UnitStats { get; set; } = [];
        public List<StarredUnitStat> StarredUnitStats { get; set; } = [];
        public List<ItemStat> ItemStats { get; set; } = [];
        public List<AugmentStat> AugmentStats { get; set; } = [];
        public List<TraitStat> TraitStats { get; set; } = [];
    }
}
