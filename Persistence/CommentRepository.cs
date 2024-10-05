using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.UserGuides;

namespace TFT_API.Persistence
{
    public class CommentRepository(TFTContext context) : ICommentDataAccess
    {
        private readonly TFTContext _context = context;

        // Adds a new comment to the database and returns the mapped CommentDto
        public async Task<CommentDto> AddCommentAsync(Comment comment)
        {
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
            return MapCommentToDto(comment);
        }

        // Gets a comment by its ID and returns it as a CommentDto
        public async Task<CommentDto?> GetCommentByIdAsync(int id)
        {
            var comment = await _context.Comments
                .Where(c => c.Id == id)
                .FirstOrDefaultAsync();
            if (comment == null) return null;
            return MapCommentToDto(comment);
        }

        // Maps a Comment entity to a CommentDto
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
