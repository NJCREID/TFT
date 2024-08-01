using TFT_API.Models.User;

namespace TFT_API.Interfaces
{
    public interface IUserDataAccess
    {
        PersistedUser AddUser(PersistedUser newMap);
        void DeleteUser(int id);
        PersistedUser? GetUserById(int id);
        PersistedUser? GetUserByEmail(string email);
        List<PersistedUser> GetUsers();
        PersistedUser? UpdateUser(PersistedUser updatedUser);
    }
}
