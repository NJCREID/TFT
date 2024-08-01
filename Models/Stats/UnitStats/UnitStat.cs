namespace TFT_API.Models.Stats.UnitStats
{
    public class UnitStat
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Stat Stat { get; set; } = new Stat();
    }
}
