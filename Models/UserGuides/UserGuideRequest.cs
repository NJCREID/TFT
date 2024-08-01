using System.ComponentModel.DataAnnotations;
using TFT_API.Models.Augments;

namespace TFT_API.Models.UserGuides
{
    public class UserGuideRequest
    {
        public string Patch { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Stage2Desc { get; set; } = string.Empty;
        public string Stage3Desc { get; set; } = string.Empty;
        public string Stage4Desc { get; set; } = string.Empty;
        public string GeneralDesc { get; set; } = string.Empty;
        public string DifficultyLevel { get; set; } = string.Empty;
        public string PlayStyle { get; set; } = string.Empty;
        public List<HexDto> Hexes { get; set; } = [];
        public  List<GuideTrait> Traits { get; set; } = [];
        public List<AugmentDto> Augments { get; set; } = [];
    }
}
