namespace TFT_API.Models.Stats.UnitStats
{
    public class BaseUnitStat
    {
        public int Id { get; set; }
        public int Games { get; set; }
        public List<UnitStat> UnitStats { get; set; } = [];
        public List<StarredUnitStat> StarredUnitStats { get; set; } = [];
        public string League { get; set; } = string.Empty;
    }
}
