using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.UserGuides;

namespace TFT_API.Persistence
{
    public class CommentRepository: ICommentDataAccess
    {
        private readonly TFTContext _context;

        public CommentRepository(TFTContext context)
        {
            _context = context;
        }

        public Comment AddComment(Comment comment)
        {
            _context.Comments.Add(comment);
            return Save(comment);
        }

        public Comment? GetCommentById(int id)
        {
            return _context.Comments.AsNoTracking().FirstOrDefault(c => c.Id == id);  
        }

        public Comment Save(Comment comment)
        {
            _context.SaveChanges();
            return comment;
        }
    }
}
