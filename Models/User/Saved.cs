namespace TFT_API.Models.User
{
    public class Saved
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GuideId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
