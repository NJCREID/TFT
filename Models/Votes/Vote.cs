using TFT_API.Models.User;
using TFT_API.Models.UserGuides;

namespace TFT_API.Models.Votes
{
    public class Vote
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public PersistedUser User { get; set; } = new PersistedUser();
        public int UserGuideId { get; set; }
        public UserGuide UserGuide { get; set; } = new UserGuide();
        public bool IsUpvote { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set;}
    }
}
