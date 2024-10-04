using TFT_API.Models.Trait;

namespace TFT_API.Models.UserGuides
{
    public class GuideTrait
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public int Tier { get; set; }
        public PersistedTrait Trait { get; set; } = new PersistedTrait();
        public UserGuide UserGuide { get; set; } = new UserGuide();
        public int UserGuideId { get; set; }
    }
}
