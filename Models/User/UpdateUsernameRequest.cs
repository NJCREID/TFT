using System.ComponentModel.DataAnnotations;

namespace TFT_API.Models.User
{
    public class UpdateUsernameRequest
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(25, ErrorMessage = "Username can't be longer than 50 characters.")]
        public required string Username { get; set; }
    }
}
