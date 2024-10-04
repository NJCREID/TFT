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
