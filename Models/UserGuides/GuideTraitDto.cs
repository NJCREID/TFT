using TFT_API.Models.Trait;

namespace TFT_API.Models.UserGuides
{
    public class GuideTraitDto
    {
        public int Value { get; set; }
        public int Tier { get; set; }
        public string Name { get; set; } = string.Empty;
        public string InGameKey { get; set; } = string.Empty;
        public List<TraitTierDto> Tiers { get; set; } = [];
    }
}
