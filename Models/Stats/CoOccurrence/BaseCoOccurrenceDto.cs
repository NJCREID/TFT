namespace TFT_API.Models.Stats.CoOccurrence
{
    public class BaseCoOccurrenceDto
    {
        public int Games { get; set; }
        public List<CoOccurrenceDto> CoOccurrences { get; set; } = [];
    }
}
