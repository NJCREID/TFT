using TFT_API.Models.User;

namespace TFT_API.Interfaces
{
    public interface IUserDataAccess
    {
        Task<UserDto> AddUserAsync(PersistedUser newUser);
        Task DeleteUserAsync(int id);
        Task<PersistedUser?> GetUserByIdAsync(int id);
        Task<PersistedUser?> GetUserByEmailAsync(string email);
        Task<List<UserDto>> GetUsersAsync();
        Task<UserDto?> UpdateUserAsync(PersistedUser updatedUser);
        Task<bool> CheckIfEmailExistsAsync(string email);
        Task<bool> CheckIfUsernameExistsAsync(string username);
    }
}
