namespace TFT_API.Models.Stats.TraitStats
{
    public class BaseTraitStatDto
    {
        public int Games { get; set; }
        public List<TraitStatDto> TraitStats { get; set; } = [];
    }
}
