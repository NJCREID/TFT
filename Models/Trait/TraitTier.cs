namespace TFT_API.Models.Trait
{
    public class TraitTier
    {
        public int Id { get; set; }
        public int Level { get; set; }
        public int Rarity { get; set; }
        public string Desc { get; set; } = string.Empty;
        public PersistedTrait Trait { get; set; } = new PersistedTrait();
    }
}
