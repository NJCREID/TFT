﻿using TFT_API.Models.Item;
using TFT_API.Models.Unit;

namespace TFT_API.Models.UserGuides
{
    public class Hex
    {
        public int Id { get; set; }
        public PersistedUnit Unit { get; set; } = new PersistedUnit();
        public List<HexItem> CurrentItems { get; set; } = [];
        public bool IsStarred { get; set; }
        public int Coordinates { get; set; }
        public int UserGuideId { get; set; }
        public UserGuide UserGuide { get; set; } = new UserGuide();
    }
}
