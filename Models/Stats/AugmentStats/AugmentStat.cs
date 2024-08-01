namespace TFT_API.Models.Stats.AugmentStats
{
    public class AugmentStat
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Stat> Stats { get; set; } = [];
    }
}
