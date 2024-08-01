using TFT_API.Models.Unit;

namespace TFT_API.Models.Stats.CompStats
{
    public class CompStatUnit
    {
        public int Id { get; set; }
        public int CompStatId { get; set; }
        public PersistedUnit Unit { get; set; } = new PersistedUnit();
    }
}
