﻿namespace TFT_API.Models.Stats.UnitStats
{
    public class StarredUnitStatDto
    {
        public string Name { get; set; } = string.Empty;
        public string InGameKey { get; set; } = string.Empty;
        public StatDto Stat { get; set; } = new StatDto();
    }
}
