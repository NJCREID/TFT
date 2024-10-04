using TFT_API.Models.User;

namespace TFT_API.Models.UserGuides
{
    public class Comment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public PersistedUser User { get; set; } = new PersistedUser();
        public string Author { get; set; } = string.Empty;
        public int UserGuideId { get; set; }
        public UserGuide UserGuide { get; set; } = new UserGuide();
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
