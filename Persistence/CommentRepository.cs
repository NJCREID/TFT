using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.UserGuides;

namespace TFT_API.Persistence
{
    public class CommentRepository(TFTContext context) : ICommentDataAccess
    {
        private readonly TFTContext _context = context;

        public async Task<CommentDto> AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return MapCommentToDto(comment);
        }

        public async Task<CommentDto?> GetCommentByIdAsync(int id)
        {
            var comment = await _context.Comments
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
            if (comment == null) return null;
            return MapCommentToDto(comment);
        }

        private static CommentDto MapCommentToDto(Comment comment)
        {
            return new CommentDto
            {
                Id = comment.Id,
                UserId = comment.UserId,
                Author = comment.Author,
                UserGuideId = comment.UserGuideId,
                Content = comment.Content,
            };
        }
    }
}
