namespace TFT_API.Models.Votes
{
    public class VoteResponse
    {
        public bool? IsUpvote { get; set; }
        public int DownVotes { get; set; }
        public int UpVotes { get; set; }
    }
}
