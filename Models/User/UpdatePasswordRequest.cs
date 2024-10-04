using System.ComponentModel.DataAnnotations;

namespace TFT_API.Models.User
{
    public class UpdatePasswordRequest
    {
        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(
            @"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*(),.?""':{}|<>]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter, one number, and one special character."
        )]
        public required string Password { get; set; }
    }
}
