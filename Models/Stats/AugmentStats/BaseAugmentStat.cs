namespace TFT_API.Models.Stats.AugmentStats
{
    public class BaseAugmentStat
    {
        public int Id { get; set; }
        public int Games { get; set; }
        public List<AugmentStat> AugmentStats { get; set; } = [];
    }
}
