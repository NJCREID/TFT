namespace TFT_API.Models.Augments
{
    public class PersistedAugment
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string InGameKey {  get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public int Tier { get; set; }
        public bool? IsHidden { get; set; }
    }
}
