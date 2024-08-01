namespace TFT_API.Models.Stats.ItemStats
{
    public class ItemStatDto
    {
        public string Name { get; set; } = string.Empty;
        public StatDto Stat { get; set; } = new StatDto();
    }
}
