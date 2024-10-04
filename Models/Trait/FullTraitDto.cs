namespace TFT_API.Models.Trait
{
    public class FullTraitDto
    {
        public string Name { get; set; } = string.Empty;
        public string InGameKey { get; set; } = string.Empty;
        public string TierString { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public Dictionary<string, string> Stats { get; set; } = [];
        public List<TraitTierDto> Tiers { get; set; } = [];
    }
}
