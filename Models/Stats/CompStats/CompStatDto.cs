using TFT_API.Models.Unit;

namespace TFT_API.Models.Stats.CompStats
{
    public class CompStatDto
    {
        public string Name { get; set; } = string.Empty;
        public List<CompStatUnitDto> Units { get; set; } = [];
        public StatDto Stat { get; set; } = new StatDto();
    }
}
