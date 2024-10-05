using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.Votes;

namespace TFT_API.Persistence
{
    public class VoteRepository(TFTContext context) : IVoteDataAccess
    {
        private readonly TFTContext _context = context;

        // Adds a new vote to the database and returns the mapped VoteDto
        public async Task<VoteDto> AddVoteAsync(Vote vote)
        {
            _context.Votes.Add(vote);
            await _context.SaveChangesAsync();

            return MapVoteToDto(vote);
        }

        // Deletes a vote by its ID
        public async Task DeleteVoteAsync(int id)
        {
            var vote = await _context.Votes.FindAsync(id);
            if (vote != null)
            {
                _context.Votes.Remove(vote);
                await _context.SaveChangesAsync();
            }
        }

        // Gets a vote by its ID and maps it to VoteDto
        public async Task<VoteDto?> GetVoteByIdAsync(int id)
        {
            return await _context.Votes
                .Where(v => v.Id == id)
                .Select(v => MapVoteToDto(v))
                .FirstOrDefaultAsync();
        }

        // Gets a list of votes by user ID and maps them to VoteDto
        public async Task<List<VoteDto>> GetVotesByUserIdAsync(int userId)
        {
            return await _context.Votes
                .Where(v => v.UserId == userId)
                .Select(v => MapVoteToDto(v))
                .ToListAsync();
        }

        // Gets the vote status for a specific user and guide
        public async Task<Vote?> GetVoteStatusAsync(int userId, int userGuideId)
        {
            return await _context.Votes
                .Where(v => v.UserId == userId && v.UserGuideId == userGuideId)
                .FirstOrDefaultAsync();
        }

        // Updates an existing vote and returns the updated VoteDto
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

        // Maps a Vote to a VoteDto
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
