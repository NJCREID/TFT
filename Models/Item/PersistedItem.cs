namespace TFT_API.Models.Item
{
    public class PersistedItem
    {
        public int Id { get; set; }
        public string InGameKey { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = [];
        public List<string>? Recipe { get; set; }
        public string Desc { get; set; } = string.Empty;
        public string ShortDesc { get; set; } = string.Empty;
        public string FromDesc { get; set; } = string.Empty;
        public bool? IsEmblem { get; set; }
        public bool? IsComponent { get; set; }
        public bool? IsHidden { get; set; }
        public string? AffectedTraitKey { get; set; }
        public string? UnitCompatabilityKey { get; set; }
        public string? TraitCompatabilityKey { get; set; }
    }
}
