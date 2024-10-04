namespace TFT_API.Models.UserGuides
{
    public class GuideUnitDto
    {
        public string InGameKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Tier { get; set; }
        public List<int> Cost { get; set; } = [];
        public List<PartialUnitTraitDto> Traits { get; set; } = [];
    }
}
