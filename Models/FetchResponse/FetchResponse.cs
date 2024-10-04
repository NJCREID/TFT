namespace TFT_API.Models.FetchResponse
{

    public class FetchedSummoner
    {
        public string Id { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public string Puuid { get; set; } = string.Empty;
        public int ProfileIconId { get; set; }
        public long RevisionDate { get; set; }
        public int SummonerLevel { get; set; }
    }

    public class FetchedLeague
    {
        public string Tier { get; set; } = string.Empty;
        public string LeagueId { get; set; } = string.Empty;
        public string Queue { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<Entry> Entries { get; set; } = [];
    }

    public class Entry
    {
        public string SummonerId { get; set; } = string.Empty;
        public int LeaguePoints { get; set; }
        public string Rank { get; set; } = string.Empty;
        public int Wins { get; set; }
        public int Losses { get; set; }
        public bool Veteran { get; set; }
        public bool Inactive { get; set; }
        public bool FreshBlood { get; set; }
        public bool HotStreak { get; set; }
    }
    public class FetchedMatch
    {
        public Metadata Metadata { get; set; }  = new Metadata();
        public Info Info { get; set; } = new Info();
        
    }

    public class Metadata
    {
        public string DataVersion { get; set; } = string.Empty;
        public string MatchId { get; set; } = string.Empty;
        public List<string> Participants { get; set; } = [];
    }

    public class Info
    {
        public string EndOfGameResult { get; set; } = string.Empty;
        public long GameCreation { get; set; }
        public long GameId { get; set; }
        public long GameDatetime { get; set; }
        public double GameLength { get; set; }
        public string GameVersion { get; set; } = string.Empty;
        public long MapId { get; set; }
        public List<Participant> Participants { get; set; } = [];
        public long QueueId { get; set; }
        public long Queue_id { get; set; }
        public string TftGameType { get; set; } = string.Empty;
        public string TftSetCoreName { get; set; } = string.Empty;
        public long TftSetNumber { get; set; }
    }

    public class Participant
    {
        public List<string> Augments { get; set; } = [];
        public Companion Companion { get; set; } = new Companion();
        public int GoldLeft { get; set; }
        public int LastRound { get; set; }
        public int Level { get; set; }
        public Missions Missions { get; set; } = new Missions();
        public int PartnerGroupId { get; set; }
        public int Placement { get; set; }
        public int PlayersEliminated { get; set; }
        public string Puuid { get; set; } = string.Empty;
        public double TimeEliminated { get; set; }
        public int TotalDamageToPlayers { get; set; }
        public List<Trait> Traits { get; set; } = [];
        public List<Unit> Units { get; set; } = [];
    }

    public class Companion
    {
        public string ContentID { get; set; } = string.Empty;
        public int ItemID { get; set; }
        public int SkinID { get; set; }
        public string Species { get; set; } = string.Empty;
    }

    public class Missions
    {
        public int PlayerScore2 { get; set; }
    }

    public class Trait
    {
        public string Name { get; set; } = string.Empty;
        public int NumUnits { get; set; }
        public int Style { get; set; }
        public int TierCurrent { get; set; }
        public int TierTotal { get; set; }
    }

    public class Unit
    {
        public string CharacterId { get; set; } = string.Empty;
        public List<string> ItemNames { get; set; } = [];
        public string Name { get; set; } = string.Empty;
        public int Rarity { get; set; }
        public int Tier { get; set; }
    }
}
