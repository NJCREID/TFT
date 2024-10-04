using System.ComponentModel.DataAnnotations;

namespace TFT_API.Models.User
{
    public class UpdateImageRequest
    {
        [Required(ErrorMessage = "Profile image is required.")]
        [DataType(DataType.Upload)]
        public required FormFile ProfileImage { get; set; }
    }
}
