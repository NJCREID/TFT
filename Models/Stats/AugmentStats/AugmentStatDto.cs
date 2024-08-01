namespace TFT_API.Models.Stats.AugmentStats
{
    public class AugmentStatDto
    {
        public string Name { get; set; } = string.Empty;
        public List<StatDto> Stats { get; set; } = [];
    }
}
