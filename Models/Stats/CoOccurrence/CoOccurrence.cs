namespace TFT_API.Models.Stats.CoOccurrence
{
    public class CoOccurrence
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Key1 { get; set; } = string.Empty;
        public string Key2 { get; set; } = string.Empty;
        public string Name1 { get; set; } = string.Empty;
        public string Name2 { get; set; } = string.Empty;
        public Stat Stat { get; set; } = new Stat();
        public int BaseCoOccurrenceId { get; set; }
        public BaseCoOccurrence BaseCoOccurrence { get; set; } = new BaseCoOccurrence();
    }
}
