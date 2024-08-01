using TFT_API.Models.UserGuides;

namespace TFT_API.Interfaces
{
    public interface ICommentDataAccess
    {
        Comment AddComment(Comment comment);
        Comment? GetCommentById(int id);
    }
}
