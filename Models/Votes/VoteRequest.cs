namespace TFT_API.Models.Votes
{
    public class VoteRequest
    {
        public int UserId { get; set; }
        public int UserGuideId { get; set; }
        public bool IsUpvote { get; set; }
    }
}
