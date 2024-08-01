namespace TFT_API.Models.Trait
{
    public class TraitDto
    {
        public string Name { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string TierString { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public List<TraitTier> Tiers { get; set; } = [];
    }
}
