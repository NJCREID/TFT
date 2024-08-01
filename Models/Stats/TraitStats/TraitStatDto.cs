namespace TFT_API.Models.Stats.TraitStats
{
    public class TraitStatDto
    {
        public string Name { get; set; } = string.Empty;
        public int NumUnits { get; set; }
        public StatDto Stat { get; set; } = new StatDto();
    }
}
