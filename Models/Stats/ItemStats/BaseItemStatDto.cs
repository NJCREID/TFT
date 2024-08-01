namespace TFT_API.Models.Stats.ItemStats
{
    public class BaseItemStatDto
    {
        public int Games { get; set; }
        public List<ItemStatDto> ItemStats { get; set; } = [];
    }
}
