namespace TFT_API.Models.Stats.TraitStats
{
    public class BaseTraitStat
    {
        public int Id { get; set; }
        public int Games { get; set; }
        public List<TraitStat> TraitStats { get; set; } = [];
        public string League { get; set; } = string.Empty;
    }
}
