using System.ComponentModel.DataAnnotations;

namespace TFT_API.Models.User
{
    public class UpdateNameRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(30, ErrorMessage = "Name can't be longer than 50 characters.")]
        public required string Name { get; set; }
    }
}
