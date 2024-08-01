using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TFT_API.Data;
using TFT_API.Interfaces;
using TFT_API.Models.User;

namespace TFT_API.Persistence
{
    public class UserRepository : IUserDataAccess
    {
        private readonly TFTContext _context;
        public UserRepository(TFTContext context)
        {
            _context = context;
        }
        public PersistedUser AddUser(PersistedUser user)
        {
            _context.Users.Add(user);
            return Save(user);
        }

        public void DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if(user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }

        public PersistedUser? GetUserById(int id)
        {
            return _context.Users.Find(id);
        }

        public PersistedUser? GetUserByEmail(string email)
        {
            return _context.Users.AsNoTracking().FirstOrDefault(u => u.Email == email);
        }

        public List<PersistedUser> GetUsers()
        {
            var users = _context.Users.AsNoTracking().ToList();
            return users;
        }

        public PersistedUser? UpdateUser(PersistedUser updatedUser)
        {
            var currentUser = GetUserById(updatedUser.Id);
            if(currentUser != null)
            {
                _context.Entry(currentUser).CurrentValues.SetValues(updatedUser);
                return Save(updatedUser);
            }
            return null;     
        }

        public PersistedUser Save(PersistedUser user)
        {
            _context.SaveChanges();
            return user;
        }
    }
}
