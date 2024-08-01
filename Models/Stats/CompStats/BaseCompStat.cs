
namespace TFT_API.Models.Stats.CompStats
{
    public class BaseCompStat
    {
        public int Id { get; set; }
        public int Games { get; set; }
        public List<CompStat> CompStats { get; set; } = [];
    }
}
