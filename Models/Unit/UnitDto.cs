using TFT_API.Models.Trait;

namespace TFT_API.Models.Unit
{
    public class UnitDto
    {
        public string Key { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string SplashImageUrl { get; set; } = string.Empty;
        public string CenteredImageUrl { get; set; } = string.Empty;
        public int Tier { get; set; }
        public List<int> Cost { get; set; } = [];
        public List<string> RecommendedItems { get; set; } = [];
        public List<int> Health { get; set; } = [];
        public List<int> AttackDamage { get; set; } = [];
        public List<int> DamagePerSecond { get; set; } = [];
        public int AttackRange { get; set; }
        public double AttackSpeed { get; set; }
        public int Armor { get; set; }
        public int MagicalResistance { get; set; }
        public Skill Skill { get; set; } = new Skill();
        public List<TraitDto> Traits { get; set; } = [];
    }
}
