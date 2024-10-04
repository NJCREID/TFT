namespace TFT_API.Models.UserGuides
{
    public class GuideAugmentDto
    {
        public string InGameKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public int Tier { get; set; }
    }
}
