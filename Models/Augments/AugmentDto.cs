namespace TFT_API.Models.Augments
{
    public class AugmentDto
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Tier { get; set; }
    }
}
