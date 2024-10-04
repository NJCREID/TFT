namespace TFT_API.Models.Trait
{
    public class PartialTraitDto
    {
        public string InGameKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string TierString { get; set; } = string.Empty;
        public List<TraitTierDto> Tiers { get; set; } = [];
    }
}
