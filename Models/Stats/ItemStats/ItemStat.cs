using TFT_API.Models.Stats.CompStats;

namespace TFT_API.Models.Stats.ItemStats
{
    public class ItemStat
    {
        public int Id { get; set; }
        public int BaseItemStatId { get; set; }
        public BaseItemStat BaseItemStat { get; set; } = new BaseItemStat();
        public string Name { get; set; } = string.Empty;
        public string InGameKey { get; set; } = string.Empty;
        public Stat Stat { get; set; } = new Stat();
    }
}
