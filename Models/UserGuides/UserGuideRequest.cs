using System.ComponentModel.DataAnnotations;
using TFT_API.Models.Augments;

namespace TFT_API.Models.UserGuides
{
    public class UserGuideRequest
    {
        [Required(ErrorMessage = "Patch is required.")]
        public string Patch { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(30, ErrorMessage = "Guide name can't be longer than 30 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stage 2 description is required.")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "Stage 2 description must be between 1 and 300 characters.")]
        public string Stage2Desc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stage 3 description is required.")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "Stage 3 description must be between 1 and 300 characters.")]
        public string Stage3Desc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stage 4 description is required.")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "Stage 4 description must be between 1 and 300 characters.")]
        public string Stage4Desc { get; set; } = string.Empty;

        [Required(ErrorMessage = "General description is required.")]
        [StringLength(300, MinimumLength = 1, ErrorMessage = "General description must be between 1 and 300 characters.")]
        public string GeneralDesc { get; set; } = string.Empty;

        [Required(ErrorMessage = "Difficulty level is required.")]
        [RegularExpression(@"^(Easy|Medium|Hard)$", ErrorMessage = "Invalid difficulty level.")]
        public string DifficultyLevel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Play style is required.")]
        [RegularExpression(@"^(Default|Level 7 Slow Roll|Level 5 Slow Roll|Level 6 Slow Roll|Fast 9|Fast 8)$", ErrorMessage = "Invalid play style.")]
        public string PlayStyle { get; set; } = string.Empty;

        [Required(ErrorMessage = "At least one hex is required.")]
        [MinLength(1, ErrorMessage = "At least one hex is required.")]
        public List<HexDto> Hexes { get; set; } = [];

        [Required(ErrorMessage = "Traits are required.")]
        [MinLength(1, ErrorMessage = "At least one trait is required.")]
        public List<GuideTrait> Traits { get; set; } = [];

        [Required(ErrorMessage = "At least three augments are required.")]
        [MinLength(3, ErrorMessage = "At least three augments are required.")]
        public List<AugmentDto> Augments { get; set; } = [];
    }
}
