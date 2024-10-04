using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Votes;

namespace TFT_API.Persistence
{
    public class VoteRepository(TFTContext context) : IVoteDataAccess
    {
        private readonly TFTContext _context = context;

        public async Task<VoteDto> AddVoteAsync(Vote vote)
        {
            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();

            return MapVoteToDto(vote);
        }

        public async Task DeleteVoteAsync(int id)
        {
            var vote = await _context.Votes.FindAsync(id);
            if (vote != null)
            {
                _context.Votes.Remove(vote);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<VoteDto?> GetVoteByIdAsync(int id)
        {
            return await _context.Votes
                .Where(v => v.Id == id)
                .Select(v => MapVoteToDto(v))
                .FirstOrDefaultAsync();
        }

        public async Task<List<VoteDto>> GetVotesByUserIdAsync(int userId)
        {
            return await _context.Votes
                .Where(v => v.UserId == userId)
                .Select(v => MapVoteToDto(v))
                .ToListAsync();
        }

        public async Task<Vote?> GetVoteStatusAsync(int userId, int userGuideId)
        {
            return await _context.Votes
                .Where(v => v.UserId == userId && v.UserGuideId == userGuideId)
                .FirstOrDefaultAsync();
        }

        public async Task<VoteDto?> UpdateVoteAsync(Vote vote)
        {
            var currentVote = await _context.Votes.FindAsync(vote.Id);
            if (currentVote != null)
            {
                _context.Entry(currentVote).CurrentValues.SetValues(vote);
                await _context.SaveChangesAsync();
                return MapVoteToDto(vote);
            }
            return null;
        }

        private static VoteDto MapVoteToDto(Vote vote)
        {
            return new VoteDto
            {
                Id = vote.Id,
                UserId = vote.UserId,
                UserGuideId = vote.UserGuideId,
                IsUpvote = vote.IsUpvote
            };
        }
    }
}
