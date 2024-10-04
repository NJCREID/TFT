﻿using TFT_API.Models.Unit;
using TFT_API.Models.User;
using TFT_API.Models.Votes;

namespace TFT_API.Models.UserGuides
{
    public class UserGuide
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public PersistedUser? User { get; set; } 
        public PersistedUnit? InitialUnit { get; set; } 
        public string? UsersUsername { get; set; } = string.Empty;
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public int Views {  get; set; }
        public string Patch { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Stage2Desc { get; set; } = string.Empty;
        public string Stage3Desc { get; set; } = string.Empty;
        public string Stage4Desc { get; set; } = string.Empty;
        public string GeneralDesc { get; set; } = string.Empty;
        public string DifficultyLevel { get; set; } = string.Empty;
        public string PlayStyle { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = [];
        public List<Hex> Hexes { get; set; } = [];
        public List<GuideTrait> Traits { get; set; } = [];
        public List<GuideAugment> Augments { get; set; } = [];
        public List<Comment> Comments { get; set; } = [];
        public List<Vote> Votes { get; set; } = [];
        public bool? IsDraft { get; set; }
        public bool? IsArchived { get; set; }
        public bool? IsAutoGenerated { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}