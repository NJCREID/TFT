namespace TFT_API.Models.Augments
{
    public class AugmentDto
    {
        public string InGameKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public int Tier { get; set; }
    }
}
