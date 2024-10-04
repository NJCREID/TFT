namespace TFT_API.Models.UserGuides
{
    public class GuideItemDto
    {
        public string InGameKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<string>? Recipe { get; set; }
        public string Desc { get; set; } = string.Empty;
        public string ShortDesc { get; set; } = string.Empty;
        public string FromDesc { get; set; } = string.Empty; 
    }
}
