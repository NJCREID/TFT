using TFT_API.Models.Unit;

namespace TFT_API.Models.Stats.CompStats
{
    public class CompStat
    {
        public int Id { get; set; }
        public BaseCompStat BaseCompStat { get; set; } = new BaseCompStat();
        public string Name { get; set; } = string.Empty;

        public string InGameKey { get; set; } = string.Empty;
        public List<CompStatUnit> Units { get; set; } = [];
        public Stat Stat { get; set; } = new Stat();
    }
}
