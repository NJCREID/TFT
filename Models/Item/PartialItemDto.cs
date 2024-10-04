namespace TFT_API.Models.Item
{
    public class PartialItemDto
    {
        public string InGameKey { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = [];
        public List<string>? Recipe { get; set; }
        public string Desc { get; set; } = string.Empty;
        public string ShortDesc { get; set; } = string.Empty;
        public string FromDesc { get; set; } = string.Empty;
        public bool? IsComponent { get; set; }
    }
}
