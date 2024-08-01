namespace TFT_API.Models.Votes
{
    public class Vote
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int UserGuideId { get; set; }
        public bool IsUpvote { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set;}
    }
}
