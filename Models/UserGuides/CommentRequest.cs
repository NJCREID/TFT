namespace TFT_API.Models.UserGuides
{
    public class CommentRequest
    {
        public int UserId { get; set; }
        public int UserGuideId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
