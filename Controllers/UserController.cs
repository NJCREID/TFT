using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TFT_API.Filters;
using TFT_API.Interfaces;
using TFT_API.Models.User;
using TFT_API.Models.UserGuides;

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

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A list of user DTOs.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/user
        ///
        /// </remarks>
        /// <response code="200">Returns a list of all users</response>
        /// <response code="404">If no users are found</response>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAllUsers()
        {
            var users = await _userRepo.GetUsersAsync();
            if (users == null || users.Count == 0) return NotFound("No users found.");
            return Ok(users);
        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>The user DTO for the specified ID.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/user/{id}
        ///
        /// </remarks>
        /// <response code="200">Returns the user with the specified ID</response>
        /// <response code="404">If the specified user ID is not found</response>
        [Authorize]
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<ActionResult<UserDto>> GetUserById([FromRoute] int id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found.");
            var userDto = _mapper.Map<UserDto>(user);
            return Ok(userDto);
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>No content on successful deletion.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// DELETE /api/user/{id}
        ///
        /// </remarks>
        /// <response code="204">No content, indicating successful deletion</response>
        /// <response code="404">If the specified user ID is not found</response>
        /// <response code="403">If the specified user ID does not match the claim user ID</response>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            if (!int.TryParse(User.FindFirstValue("UserId"), out int userId))
                return Unauthorized("Invalid User ID.");
            // Checks if the user ID on the claim matches the user id.
            if (userId != id)
                return Forbid("Not authorized to delete this user.");
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found.");
            await _userRepo.DeleteUserAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Updates the profile image of the currently authenticated user.
        /// </summary>
        /// <param name="request">The request containing the new profile image.</param>
        /// <returns>The updated user DTO.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// POST /api/user/update-image
        /// </remarks>
        /// <response code="200">Returns the updated user</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="400">If the request isnt valid</response>
        /// <response code="500">If the body</response>
        [Authorize]
        [HttpPost("update-image")]
        public async Task<IActionResult> UpdateProfileImage([FromBody] UpdateImageRequest request)
        {
            
            if (!int.TryParse(User.FindFirstValue("UserId"), out var userId)) 
                return Unauthorized("Invalid user ID.");
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null) return NotFound("User not found.");
            try
            {
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
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to update profile image. Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Updates the username of the currently authenticated user.
        /// </summary>
        /// <param name="request">The request containing the new username.</param>
        /// <returns>The updated user DTO.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// PATCH /api/user/update-username
        /// </remarks>
        /// <response code="200">Returns the updated user</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="409">If the new username is already taken</response>
        /// <response code="400">If the request isnt valid</response>
        /// <response code="500">If there is an error processing the request</response>
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


        /// <summary>
        /// Updates the name of the currently authenticated user.
        /// </summary>
        /// <param name="request">The request containing the new name.</param>
        /// <returns>The updated user DTO.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// PATCH /api/user/update-name
        /// </remarks>
        /// <response code="200">Returns the updated user</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="400">If the request isnt valid</response>
        /// <response code="500">If there is an error processing the request</response>
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

        /// <summary>
        /// Updates the email of the currently authenticated user.
        /// </summary>
        /// <param name="request">The request containing the new email.</param>
        /// <returns>The updated user DTO.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// PATCH /api/user/update-email
        /// </remarks>
        /// <response code="200">Returns the updated user</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="409">If the new email is already taken</response>
        /// <response code="400">If the request isnt valid</response>
        /// <response code="500">If there is an error processing the request</response>
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

        /// <summary>
        /// Updates the password of the currently authenticated user.
        /// </summary>
        /// <param name="request">The request containing the new password.</param>
        /// <returns>The updated user DTO.</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// PATCH /api/user/update-password
        /// </remarks>
        /// <response code="200">Returns the updated user</response>
        /// <response code="401">If the user is not authenticated</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="400">If the request isnt valid</response>
        /// <response code="500">If there is an error processing the request</response>
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
