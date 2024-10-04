namespace TFT_API.Models.Stats.ItemStats
{
    public class BaseItemStat
    {
        public int Id { get; set; }
        public int Games { get; set; }
        public List<ItemStat> ItemStats { get; set; } = [];
        public string League { get; set; } = string.Empty;

    }
}
