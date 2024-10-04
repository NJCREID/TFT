using TFT_API.Models.UserGuides;

namespace TFT_API.Interfaces
{
    public interface ICommentDataAccess
    {
        Task<CommentDto> AddCommentAsync(Comment comment);
        Task<CommentDto?> GetCommentByIdAsync(int id);
    }
}
