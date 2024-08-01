namespace TFT_API.Models.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int GuidesCount { get; set; }
        public int CommentsCount { get; set; }
        public int UpVotesCount { get; set; }
        public int DownVotesCount { get; set; }
    }
}
