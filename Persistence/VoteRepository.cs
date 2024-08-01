using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Votes;

namespace TFT_API.Persistence
{
    public class VoteRepository : IVoteDataAccess
    {
        private readonly TFTContext _context;

        public VoteRepository(TFTContext context)
        {
            _context = context;
        }

        public Vote AddVote(Vote vote)
        {
            _context.Votes.Add(vote);
            return Save(vote);
        }

        public void DeleteVote(int id)
        {
            var vote = _context.Votes.Find(id);
            if (vote != null)
            {
                _context.Votes.Remove(vote);
                _context.SaveChanges();  
            }
        }
        public Vote? GetVoteById(int id)
        {
            return _context.Votes.Find(id);
        }

        public List<Vote> GetVotesByUserId(int userId)
        {
            return _context.Votes.AsNoTracking().Where(v => v.UserId == userId).ToList();
        }

        public Vote? GetVoteStatus(int userId, int userGuideId)
        {
            return _context.Votes.AsNoTracking().FirstOrDefault(v => v.UserId == userId && v.UserGuideId == userGuideId);
        }

        public Vote? UpdateVote(Vote vote)
        {
            var currentVote = GetVoteById(vote.Id);
            if (currentVote != null)
            {
                _context.Entry(currentVote).CurrentValues.SetValues(vote);
                return Save(vote);
            }
            return null;
        }

        public Vote Save(Vote vote)
        {
            _context.SaveChanges();
            return vote;
        }

    }
}
