using System.ComponentModel.DataAnnotations;

namespace TFT_API.Models.UserGuides
{
    public class CommentRequest
    {
        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "UserGuideId is required.")]
        public int UserGuideId { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        [StringLength(1000, ErrorMessage = "Content cannot be longer than 1000 characters.")]
        public string Content { get; set; } = string.Empty;
    }
}
