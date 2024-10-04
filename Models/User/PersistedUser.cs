using TFT_API.Models.UserGuides;
using TFT_API.Models.Votes;

namespace TFT_API.Models.User
{
    public class PersistedUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int GuidesCount { get; set; }
        public int CommentsCount { get; set; }
        public int UpVotesCount { get; set; }
        public int DownVotesCount { get; set; }
        public string ProfileImageUrl { get; set; } = string.Empty;
        public List<UserGuide> UserGuides { get; set; } = [];
        public List<Comment> Comments { get; set; } = [];
        public List<Vote> Votes { get; set; } = [];
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
