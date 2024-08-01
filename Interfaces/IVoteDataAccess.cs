using TFT_API.Models.Votes;

namespace TFT_API.Interfaces
{
    public interface IVoteDataAccess
    {
        List<Vote> GetVotesByUserId(int userId);
        Vote? GetVoteStatus(int userId, int postId);
        Vote AddVote(Vote vote);
        Vote? GetVoteById(int id);
        Vote? UpdateVote(Vote vote);
        void DeleteVote(int id);
    }
}
