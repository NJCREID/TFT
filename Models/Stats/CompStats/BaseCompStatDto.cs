namespace TFT_API.Models.Stats.CompStats
{
    public class BaseCompStatDto
    {
        public int Games { get; set; }
        public List<CompStatDto> CompStats { get; set; } = [];
    }
}
