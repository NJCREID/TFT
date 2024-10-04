namespace TFT_API.Models.Match
{
    public class Match
    {
        public int Id { get; set; }
        public string Puuid { get; set; } = string.Empty;
        public int Placement { get; set; }
        public List<MatchUnit> Units { get; set; } = [];
        public List<MatchTrait> Traits { get; set; } = [];
        public List<string> Augments { get; set; } = [];
        public string? League { get; set; } = string.Empty;
    }

    public class MatchUnit
    {
        public int Id { get; set; }
        public string CharacterId { get; set; } = string.Empty;
        public List<string> ItemNames { get; set; } = [];
        public string Name { get; set; } = string.Empty;
        public int Rarity { get; set; }
        public int Tier { get; set; }
    }

    public class MatchTrait
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int NumUnits { get; set; }
        public int Style { get; set; }
        public int TierCurrent { get; set; }
        public int TierTotal { get; set; }
        public int Tier { get; set; }
    }

    public class MatchDto
    {
        public string? League { get; set; }
        public int Placement { get; set; }
        public List<string> Augments { get; set; } = [];
        public List<MatchUnitDto> Units { get; set; } = [];
        public List<MatchTraitDto> Traits { get; set; } = [];
    }
    public class MatchTraitDto
    {
        public string Name { get; set; } = string.Empty;
        public int NumUnits { get; set; }
    }

    public class MatchUnitDto
    {
        public string CharacterId { get; set; } = string.Empty;
        public List<string> ItemNames { get; set; } = [];
        public int Tier { get; set; }
    }
}
