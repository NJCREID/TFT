namespace TFT_API.Models.Unit
{
    public class PersistedUnit
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string InGameKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
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
        public List<UnitTrait> Traits { get; set; } = [];
        public bool? IsHidden { get; set; }
        public string? UniqueItemKey { get; set; }
        public bool? IsItemIncompatible { get; set; }
        public string? CompatabilityType { get; set; }
        public bool? IsTriggerUnit { get; set; }
    }
}
