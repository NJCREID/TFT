namespace TFT_API.Models.FetchResponse
{

    public class FetchedSummoner
    {
        public string id { get; set; } = string.Empty;
        public string accountId { get; set; } = string.Empty;
        public string puuid { get; set; } = string.Empty;
        public int profileIconId { get; set; }
        public long revisionDate { get; set; }
        public int summonerLevel { get; set; }
    }

    public class FetchedLeague
    {
        public string tier { get; set; } = string.Empty;
        public string leagueId { get; set; } = string.Empty;
        public string queue { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public List<Entry> entries { get; set; } = [];
    }

    public class Entry
    {
        public string summonerId { get; set; } = string.Empty;
        public int leaguePoints { get; set; }
        public string rank { get; set; } = string.Empty;
        public int wins { get; set; }
        public int losses { get; set; }
        public bool veteran { get; set; }
        public bool inactive { get; set; }
        public bool freshBlood { get; set; }
        public bool hotStreak { get; set; }
    }
    public class FetchedMatch
    {
        public Metadata metadata { get; set; }  = new Metadata();
        public Info info { get; set; } = new Info();
    }

    public class Metadata
    {
        public string data_version { get; set; } = string.Empty;
        public string match_id { get; set; } = string.Empty;
        public List<string> participants { get; set; } = [];
    }

    public class Info
    {
        public string endOfGameResult { get; set; } = string.Empty;
        public long gameCreation { get; set; }
        public long gameId { get; set; }
        public long game_datetime { get; set; }
        public double game_length { get; set; }
        public string game_version { get; set; } = string.Empty;
        public long mapId { get; set; }
        public List<Participant> participants { get; set; } = [];
        public long queueId { get; set; }
        public long queue_id { get; set; }
        public string tft_game_type { get; set; } = string.Empty;
        public string tft_set_core_name { get; set; } = string.Empty;
        public long tft_set_number { get; set; }
    }

    public class Participant
    {
        public List<string> augments { get; set; } = [];
        public Companion companion { get; set; } = new Companion();
        public int gold_left { get; set; }
        public int last_round { get; set; }
        public int level { get; set; }
        public Missions missions { get; set; } = new Missions();
        public int partner_group_id { get; set; }
        public int placement { get; set; }
        public int players_eliminated { get; set; }
        public string puuid { get; set; } = string.Empty;
        public double time_eliminated { get; set; }
        public int total_damage_to_players { get; set; }
        public List<Trait> traits { get; set; } = [];
        public List<Unit> units { get; set; } = [];
    }

    public class Companion
    {
        public string content_ID { get; set; } = string.Empty;
        public int item_ID { get; set; }
        public int skin_ID { get; set; }
        public string species { get; set; } = string.Empty;
    }

    public class Missions
    {
        public int PlayerScore2 { get; set; }
    }

    public class Trait
    {
        public string name { get; set; } = string.Empty;
        public int num_units { get; set; }
        public int style { get; set; }
        public int tier_current { get; set; }
        public int tier_total { get; set; }
    }

    public class Unit
    {
        public string character_id { get; set; } = string.Empty;
        public List<string> itemNames { get; set; } = [];
        public string name { get; set; } = string.Empty;
        public int rarity { get; set; }
        public int tier { get; set; }
    }
}
