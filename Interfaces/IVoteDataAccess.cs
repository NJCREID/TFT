using TFT_API.Models.Votes;

namespace TFT_API.Interfaces
{
    public interface IVoteDataAccess
    {
        Task<List<VoteDto>> GetVotesByUserIdAsync(int userId);
        Task<Vote?> GetVoteStatusAsync(int userId, int postId);
        Task<VoteDto> AddVoteAsync(Vote vote);
        Task<VoteDto?> GetVoteByIdAsync(int id);
        Task<VoteDto?> UpdateVoteAsync(Vote vote);
        Task DeleteVoteAsync(int id);
    }
}
