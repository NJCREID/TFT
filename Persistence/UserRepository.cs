using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.User;

namespace TFT_API.Persistence
{
    public class UserRepository(TFTContext context) : IUserDataAccess
    {
        private readonly TFTContext _context = context;

        // Adds a new user to the database and returns the mapped UserDto
        public async Task<UserDto> AddUserAsync(PersistedUser user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapUserToDto(user);
        }

        // Deletes a user by their ID
        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        // Gets a user by their ID
        public async Task<PersistedUser?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }

        // Gets a user by their email address
        public async Task<PersistedUser?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }

        // Gets a list of all users as UserDto
        public async Task<List<UserDto>> GetUsersAsync()
        {
            return await _context.Users
                .Select(u => MapUserToDto(u))
                .ToListAsync();
        }

        // Updates an existing user and returns the updated UserDto
        public async Task<UserDto?> UpdateUserAsync(PersistedUser updatedUser)
        {
            var currentUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == updatedUser.Id);
            if (currentUser != null)
            {
                _context.Entry(currentUser).CurrentValues.SetValues(updatedUser);
                await _context.SaveChangesAsync();
                return MapUserToDto(currentUser);
            }
            return null;
        }

        // Checks if an email already exists in the database
        public async Task<bool> CheckIfEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        // Checks if a username already exists in the database
        public async Task<bool> CheckIfUsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        // Maps a PersistedUser to a UserDto
        private static UserDto MapUserToDto(PersistedUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                GuidesCount = user.GuidesCount,
                CommentsCount = user.CommentsCount,
                UpVotesCount = user.UpVotesCount,
                DownVotesCount = user.DownVotesCount
            };
        }
    }
}
