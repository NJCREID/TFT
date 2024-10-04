using System.ComponentModel.DataAnnotations;

namespace TFT_API.Models.User
{
    public class UpdateEmailRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }
    }
}
