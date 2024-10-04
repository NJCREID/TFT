using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Migrations;
using TFT_API.Models.User;

namespace TFT_API.Persistence
{
    public class UserRepository(TFTContext context) : IUserDataAccess
    {
        private readonly TFTContext _context = context;

        public async Task<UserDto> AddUserAsync(PersistedUser user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return MapUserToDto(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<PersistedUser?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<PersistedUser?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Where(u => u.Email == email)
                .FirstOrDefaultAsync();
        }
        
        public async Task<List<UserDto>> GetUsersAsync()
        {
            return await _context.Users
                .Select(u => MapUserToDto(u))
                .ToListAsync();
        }

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

        public async Task<Boolean> CheckIfEmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }
        
        public async Task<Boolean> CheckIfUsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

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
