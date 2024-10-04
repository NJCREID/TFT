using TFT_API.Models.Augments;

namespace TFT_API.Models.UserGuides
{
    public class GuideAugment
    {
        public int Id { get; set; }
        public PersistedAugment Augment { get; set; } = new PersistedAugment();
        public int UserGuideId { get; set; }
        public UserGuide UserGuide { get; set; } = new UserGuide();
    }
}
