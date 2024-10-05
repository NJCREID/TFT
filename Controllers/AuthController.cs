using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TFT_API.Filters;
using TFT_API.Interfaces;
using TFT_API.Models.User;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ValidateRequestBody]
    public class AuthController(IPasswordHasher passwordHasher, IConfiguration config, IUserDataAccess userRepo, IMapper mapper) : ControllerBase
    {

        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        private readonly IConfiguration _config = config;
        private readonly IUserDataAccess _userRepo = userRepo;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Authenticates a user and returns a JWT token upon successful login.
        /// </summary>
        /// <param name="request">The login request containing user credentials</param>
        /// <returns>A response containing user details and a JWT token</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// POST /api/auth/login
        /// {
        ///     "email": "user@example.com",
        ///     "password": "password123"
        /// }
        /// </remarks>
        /// <response code="200">Returns user details and a JWT token</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="400">If there is an error creating the token or the request is invalid</response>
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponse>> Login([FromBody] UserLoginRequest request)
        {
            var user = await Authenticate(request);
            if (user != null)
            {
                var token = Generate(user);
                if (token == null)
                {
                    return BadRequest("Error creating token");
                }
                var userResponse = new UserLoginResponse
                {
                    User = _mapper.Map<UserDto>(user),
                    Token = token,
                };
                return Ok(userResponse);
            }
            return NotFound("User not found");
        }

        /// <summary>
        /// Registers a new user and creates their account.
        /// </summary>
        /// <param name="request">The registration request containing user details</param>
        /// <returns>A response containing the created user details</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// POST /api/auth/register
        /// {
        ///     "email": "user@example.com",
        ///     "password": "password123",
        ///     "name": "John Doe"
        /// }
        /// </remarks>
        /// <response code="201">Returns the created user details</response>
        /// <response code="409">If a user with the same email already exists</response>
        /// <response code="400">If the request is invalid</response>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody] AddUserRequest request)
        {
            var emailExist = await _userRepo.CheckIfEmailExistsAsync(request.Email);
            if (emailExist)
                return Conflict("A user with the same email already exists.");
            var user = _mapper.Map<PersistedUser>(request);
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
            user.CreatedDate = DateTime.Now;
            user.ModifiedDate = DateTime.Now;
            var result = _userRepo.AddUserAsync(user);
            return CreatedAtRoute("GetUser", new { id = result.Id }, result);
        }

        // Generates a JWT token when logging in that lasts 7 days.
        private string? Generate(PersistedUser user)
        {
            var jwtkey = _config["Jwt:Key"];
            if (jwtkey == null)
            {
                return null;
            }
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtkey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("UserId", user.Id.ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.UtcNow.AddDays(7),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Retrieve the user by email and verifies their password
        private async Task<PersistedUser?> Authenticate(UserLoginRequest userLogin)
        {
            var currentUser = await  _userRepo.GetUserByEmailAsync(userLogin.Email);
            if (currentUser == null) return null;
            var passwordVerificationResult = _passwordHasher.VerifyPassword(userLogin.Password, currentUser.PasswordHash);
            if (passwordVerificationResult) return currentUser;
            return null;
        }
    }    
}
