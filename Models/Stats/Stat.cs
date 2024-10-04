using TFT_API.Models.Stats.AugmentStats;

namespace TFT_API.Models.Stats
{
    public class Stat
    {
        public int Id { get; set; }
        public int Games { get; set; }
        public int Place { get; set; }
        public int Top4 { get; set; }
        public int Win { get; set; }
        public double Delta { get; set; }
        public AugmentStat? AugmentStat { get; set; }
    }
}
