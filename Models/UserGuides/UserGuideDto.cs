using TFT_API.Models.Augments;
using TFT_API.Interfaces;

namespace TFT_API.Models.UserGuides
{
    public class UserGuideDto : IGuideDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UsersUsername { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Stage2Desc { get; set; } = string.Empty;
        public string Stage3Desc { get; set; } = string.Empty;
        public string Stage4Desc { get; set; } = string.Empty;
        public string GeneralDesc { get; set; } = string.Empty;
        public string DifficultyLevel { get; set; } = string.Empty;
        public string PlayStyle {  get; set; } = string.Empty;
        public List<HexDto> Hexes { get; set; } = [];
        public List<GuideTraitDto> Traits { get; set; } = [];
        public List<GuideAugmentDto> Augments { get; set; } = [];
        public List<CommentDto> Comments { get; set; } = [];
        public bool? IsUpvote { get; set; }
    }
}
