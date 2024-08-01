namespace TFT_API.Models.Stats.TraitStats
{
    public class TraitStat
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int NumUnits { get; set; }
        public Stat Stat { get; set; } = new Stat();

    }
}
