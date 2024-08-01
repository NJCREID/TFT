namespace TFT_API.Models.Stats.ItemStats
{
    public class ItemStat
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Stat Stat { get; set; } = new Stat();
    }
}
