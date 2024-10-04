namespace TFT_API.Models.Stats.CoOccurrence
{
    public class BaseCoOccurrence
    {
        public int Id { get; set; }
        public int Games { get; set; }
        public List<CoOccurrence> CoOccurrences { get; set; } = [];
    }
}
