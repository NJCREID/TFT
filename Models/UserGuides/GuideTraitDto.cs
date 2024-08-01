using TFT_API.Models.Trait;

namespace TFT_API.Models.UserGuides
{
    public class GuideTraitDto
    {
        public int Value { get; set; }
        public int Tier { get; set; }
        public PartialTraitDto Trait { get; set; } = new PartialTraitDto();
    }
}
