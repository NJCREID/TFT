using TFT_API.Models.Stats.CompStats;

namespace TFT_API.Models.Stats.TraitStats
{
    public class TraitStat
    {
        public int Id { get; set; }
        public BaseTraitStat BaseTraitStat { get; set; } = new BaseTraitStat();
        public string Name { get; set; } = string.Empty;
        public string InGameKey { get; set; } = string.Empty;
        public int NumUnits { get; set; }
        public Stat Stat { get; set; } = new Stat();

    }
}
