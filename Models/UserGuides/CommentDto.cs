using TFT_API.Models.User;

namespace TFT_API.Models.UserGuides
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Author { get; set; } = string.Empty;
        public int UserGuideId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
