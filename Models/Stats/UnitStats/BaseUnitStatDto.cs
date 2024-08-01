namespace TFT_API.Models.Stats.UnitStats
{
    public class BaseUnitStatDto
    {
        public int Games { get; set; }
        public List<UnitStatDto> UnitStats { get; set; } = [];
        public List<StarredUnitStatDto> StarredUnitStats { get; set; } = [];
    }
}
