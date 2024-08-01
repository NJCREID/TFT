using TFT_API.Models.Unit;

namespace TFT_API.Models.Stats.CompStats
{
    public class CompStat
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<CompStatUnit> Units { get; set; } = [];
        public Stat Stat { get; set; } = new Stat();
    }
}
