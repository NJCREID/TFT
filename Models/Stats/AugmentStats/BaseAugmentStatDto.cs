namespace TFT_API.Models.Stats.AugmentStats
{
    public class BaseAugmentStatDto
    {
        public int Games { get; set; }
        public List<AugmentStatDto> AugmentStats { get; set; } = [];
    }
}
