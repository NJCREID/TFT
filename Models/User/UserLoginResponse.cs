namespace TFT_API.Models.User
{
    public class UserLoginResponse
    {
        public UserDto User { get; set; } = new UserDto();
        public string Token { get; set; } = string.Empty;
    }
}
