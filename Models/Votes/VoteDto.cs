namespace TFT_API.Models.Votes
{
    public class VoteDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int UserGuideId { get; set; }
        public bool? IsUpvote { get; set; }
    }
}
