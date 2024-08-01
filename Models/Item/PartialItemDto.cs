namespace TFT_API.Models.Item
{
    public class PartialItemDto
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = [];
        public List<string>? Recipe { get; set; }
        public string Desc { get; set; } = string.Empty;
        public string? AffectedTraitKey { get; set; }
        public bool? IsComponent { get; set; }
    }
}
