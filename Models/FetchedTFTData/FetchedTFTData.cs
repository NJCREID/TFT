using TFT_API.Models.Unit;

namespace TFT_API.Models.FetchedTFTData
{
    public class Champion
    {
        public string Key { get; set; } = string.Empty;
        public string IngameKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string OriginalImageUrl { get; set; } = string.Empty;
        public string SplashUrl { get; set; } = string.Empty;
        public List<string> Traits { get; set; } = [];
        public List<int> Cost { get; set; } = [];
        public List<double> Health { get; set; } = [];
        public List<int> AttackDamage { get; set; } = [];
        public List<int> DamagePerSecond { get; set; } = [];
        public int AttackRange { get; set; }
        public double AttackSpeed { get; set; }
        public int Armor { get; set; }
        public int MagicalResistance { get; set; }
        public Skill Skill { get; set; } = new();
        public bool? IsHidden { get; set; }
        public bool? IsHiddenGuide { get; set; }
        public bool? IsHiddenLanding { get; set; }
        public bool? IsHiddenTeamBuilder { get; set; }
        public List<string> Tags { get; set; } = [];
    }

    public class Trait
    {
        public string Key { get; set; } = string.Empty;
        public string IngameKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string BlackImageUrl { get; set; } = string.Empty;
        public string WhiteImageUrl { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<TraitStyle> Styles { get; set; } = [];
        public List<StageStyle> StageStyles { get; set; } = [];
        public Dictionary<string, string> Stats { get; set; } = [];
        public bool? IsHidden { get; set; }
    }

    public class TraitStyle
    {
        public string Style { get; set; } = string.Empty;
        public int Min { get; set; }
        public int? Max { get; set; }
    }

    public class StageStyle
    {
        public string Style { get; set; } = string.Empty;
        public int Min { get; set; }
        public int? Max { get; set; }
    }

    public class Item
    {
        public string Key { get; set; } = string.Empty;
        public string IngameKey { get; set; } = string.Empty;
        public string IngameIcon { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public string ShortDesc { get; set; } = string.Empty;
        public string FromDesc { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> Compositions { get; set; } = [];
        public bool? IsNormal { get; set; }
        public List<string> Tags { get; set; } = [];
        public bool? IsSupport { get; set; }
        public bool? IsUnique { get; set; }
        public bool? IsFromItem { get; set; }
        public bool? IsArtifact { get; set; }
        public bool? IsNew { get; set; }
        public bool? IsRadiant { get; set; }
        public bool? IsHidden { get; set; }
        public string AffectedTraitKey { get; set; } = string.Empty;
        public bool? IsEmblem { get; set; }
        public bool? IsInkshadow { get; set; }
        public bool? IsStoryweaver { get; set; }
    }

    public class Augment
    {
        public string Key { get; set; } = string.Empty;
        public string IngameKey { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int Tier { get; set; }
        public string? ChampionIngameKey { get; set; }
        public bool? IsHidden { get; set; }
        public List<string> Tags { get; set; } = [];
        public List<string> LegendCodes { get; set; } = [];
        public bool? IsNew { get; set; }
    }
}
