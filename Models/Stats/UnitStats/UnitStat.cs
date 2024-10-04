using TFT_API.Models.Stats.CompStats;

namespace TFT_API.Models.Stats.UnitStats
{
    public class UnitStat
    {
        public int Id { get; set; }
        public BaseUnitStat BaseUnitStat { get; set; } = new BaseUnitStat();
        public string Name { get; set; } = string.Empty;
        public string InGameKey { get; set; } = string.Empty;
        public Stat Stat { get; set; } = new Stat();
    }
}
