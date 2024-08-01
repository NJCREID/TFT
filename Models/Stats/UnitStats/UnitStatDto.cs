namespace TFT_API.Models.Stats.UnitStats
{
    public class UnitStatDto
    {
        public string Name { get; set; } = string.Empty;
        public StatDto Stat { get; set; } = new StatDto();
    }
}
