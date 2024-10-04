using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TFT_API.Filters;
using TFT_API.Interfaces;
using TFT_API.Models.User;

namespace TFT_API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [ValidateRequestBody]
    public class UserController(IUserDataAccess userRepo, IMapper mapper, IPasswordHasher passwordHasher) : ControllerBase
    {
        private readonly IUserDataAccess _userRepo = userRepo;
        private readonly IMapper _mapper = mapper;
        private readonly IPasswordHasher _passwordHasher = passwordHasher;

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _userRepo.GetUsersAsync();
            if (users == null || users.Count == 0) return NotFound("No users found.");
            return Ok(users);
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<ActionResult<UserDto>> GetUserById([FromRoute] int id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found.");
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found.");
            await _userRepo.DeleteUserAsync(id);
            return NoContent();
        }

        [Authorize]
        [HttpPost("update-image")]
        public async Task<IActionResult> UpdateProfileImage([FromBody] UpdateImageRequest request)
        {
            if (!int.TryParse(User.FindFirstValue("UserId"), out var userId)) 
                return Unauthorized("Invalid user ID.");
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null) return NotFound("User not found.");
            if (!string.IsNullOrEmpty(user.ProfileImageUrl))
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "assets", "images", "uploads", "profile-images", user.ProfileImageUrl);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "assets", "images", "uploads", "profile-images");
            Directory.CreateDirectory(uploadsFolderPath);

            var fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(request.ProfileImage.FileName)}";
            var filePath = Path.Combine(uploadsFolderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.ProfileImage.CopyToAsync(fileStream);
            }

            user.ProfileImageUrl = "image/uploads/profile-images/" + fileName;
            var userDto = await _userRepo.UpdateUserAsync(user);
            return Ok(userDto);   
        }

        [Authorize]
        [HttpPatch("update-username")]
        public async Task<ActionResult<UserDto>> UpdateUsersUsername([FromBody] UpdateUsernameRequest request)
        {
            if(!int.TryParse(User.FindFirstValue("UserId"), out var userId))
                return Unauthorized("Invalid user ID.");
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null) return NotFound("User not found.");
            var usernameExists = await _userRepo.CheckIfUsernameExistsAsync(request.Username);
            if (usernameExists) return Conflict("A user with the same username already exists.");
            try
            {
                user.Username = request.Username;
                var userDto = _userRepo.UpdateUserAsync(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to modify the user. Error: " + ex.Message);
            }
        }

        [Authorize]
        [HttpPatch("update-name")]
        public async Task<ActionResult<UserDto>> UpdateUsersName([FromBody] UpdateNameRequest request)
        {
            if (!int.TryParse(User.FindFirstValue("UserId"), out var userId))
                return Unauthorized("Invalid user ID.");

            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null) return NotFound("User not found.");
            try
            {
                user.Name = request.Name;
                var userDto = _userRepo.UpdateUserAsync(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to modify the user. Error: " + ex.Message);
            }
        }

        [Authorize]
        [HttpPatch("update-email")]
        public async Task<ActionResult<UserDto>> UpdateUsersEmail([FromBody] UpdateEmailRequest request)
        {
            if (!int.TryParse(User.FindFirstValue("UserId"), out var userId))
                return Unauthorized("Invalid user ID.");
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null) return NotFound("User not found.");
            var emailExists = await _userRepo.CheckIfEmailExistsAsync(request.Email);
            if (emailExists) return Conflict("A user with the same email already exists.");
            try
            {
                user.Email = request.Email;
                var userDto = _userRepo.UpdateUserAsync(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to modify the user. Error: " + ex.Message);
            }
        }

        [Authorize]
        [HttpPatch("update-password")]
        public async Task<ActionResult<UserDto>> UpdateUsersPassword([FromBody] UpdatePasswordRequest request)
        {
            if (!int.TryParse(User.FindFirstValue("UserId"), out var userId))
                return Unauthorized("Invalid user ID.");
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null) return NotFound("User not found.");
            try
            {
                user.PasswordHash =  _passwordHasher.HashPassword(request.Password);
                var userDto = _userRepo.UpdateUserAsync(user);
                return Ok(userDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to modify the user. Error: " + ex.Message);
            }
        }
    }
}
