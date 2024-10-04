using System.ComponentModel.DataAnnotations;

namespace TFT_API.Models.Votes
{
    public class VoteRequest
    {
        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "User Guide ID is required.")]
        public int UserGuideId { get; set; }

        [Required(ErrorMessage = "Vote type (IsUpvote) is required.")]
        public bool IsUpvote { get; set; }
    }
}
