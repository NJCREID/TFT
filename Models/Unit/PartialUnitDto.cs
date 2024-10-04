using TFT_API.Models.Trait;

namespace TFT_API.Models.Unit
{
    public class PartialUnitDto
    {
        public string InGameKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Tier { get; set; }
        public List<int> Cost { get; set; } = [];
        public List<string> RecommendedItems { get; set; } = [];
        public List<PartialTraitDto> Traits { get; set; } = [];
        public bool? IsItemIncompatible { get; set; }
        public string? CompatabilityType { get; set; }
        public bool? IsTriggerUnit { get; set; }
        public string? UniqueItemKey { get; set; }
    }
}
