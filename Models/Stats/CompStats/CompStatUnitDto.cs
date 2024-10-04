using TFT_API.Models.Trait;

namespace TFT_API.Models.Stats.CompStats
{
    public class CompStatUnitDto
    {
        public string InGameKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Tier { get; set; }
    }
}
