namespace TFT_API.Models.Stats.AugmentStats
{
    public class AugmentStat
    {
        public int Id { get; set; }
        public BaseAugmentStat BaseAugmentStat { get; set; } = new BaseAugmentStat();
        public string Name { get; set; } = string.Empty;
        public string InGameKey { get; set; } = string.Empty;
        public List<Stat> Stats { get; set; } = [];
    }
}
