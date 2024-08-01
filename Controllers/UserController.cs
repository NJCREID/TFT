using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TFT_API.Interfaces;
using TFT_API.Models.User;

namespace TFT_API.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserDataAccess _userRepo;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;


        public UserController(IUserDataAccess userRepo, IMapper mapper, IPasswordHasher passwordHasher)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            
        }

        [Authorize]
        [HttpGet]
        public ActionResult<List<UserDto>> GetAllUsers()
        {
            var users = _userRepo.GetUsers();
            return Ok(_mapper.Map<List<UserDto>>(users));
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetUser")]
        public ActionResult<UserDto> GetUserById(int id)
        {
            var user = _userRepo.GetUserById(id);
            if (user == null) return NotFound();
            return Ok(_mapper.Map<UserDto>(user));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var user = _userRepo.GetUserById(id);
            if (user == null) return NotFound();
            _userRepo.DeleteUser(id);
            return NoContent();
        }

        [Authorize]
        [HttpPatch("{id}")]
        public ActionResult<UserDto> UpdateUserLogin(int id, UserLoginRequest request)
        {
            if (request == null)
                return BadRequest();
            var user = _userRepo.GetUserById(id);
            if (user == null) return NotFound();
            if (_userRepo.GetUsers().Any(c => c.Email == request.Email && c.Id != id))
                return Conflict("A user with the same username already exists.");
            try
            {
                var updatedUser = _mapper.Map<PersistedUser>(request);
                updatedUser.Id = user.Id;
                updatedUser.Name = user.Name;
                updatedUser.Username = user.Username;
                updatedUser.PasswordHash = _passwordHasher.HashPassword(request.Password);
                updatedUser.CreatedDate = user.CreatedDate;
                updatedUser.ModifiedDate = DateTime.Now;
                var result = _userRepo.UpdateUser(updatedUser);
                return Ok(_mapper.Map<UserDto>(result));
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to modify the user. Error: " + ex.Message);
            }
        }
    }
}
