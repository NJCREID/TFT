using TFT_API.Models.Trait;

namespace TFT_API.Models.Unit
{
    public class PartialUnitDto
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string SplashImageUrl { get; set; } = string.Empty;
        public string CenteredImageUrl { get; set; } = string.Empty;
        public int Tier { get; set; }
        public List<int> Cost { get; set; } = [];
        public List<string> RecommendedItems { get; set; } = [];
        public List<PartialTraitDto> Traits { get; set; } = [];
    }
}
