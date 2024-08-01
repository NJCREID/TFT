using System.ComponentModel.DataAnnotations;

namespace TFT_API.Models.User
{
    public class AddUserRequest
    {
        public required string Username { get; set; }
        public required string Name { get; set; } 
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
