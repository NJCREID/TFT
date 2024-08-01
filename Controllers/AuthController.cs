using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TFT_API.Interfaces;
using TFT_API.Models.User;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly IPasswordHasher _passwordHasher;
        private readonly IConfiguration _config;
        private readonly IUserDataAccess _userRepo;
        private readonly IMapper _mapper;


        public AuthController(IPasswordHasher passwordHasher, IConfiguration config, IUserDataAccess userRepo, IMapper mapper)
        {
            _passwordHasher = passwordHasher;
            _config = config;
            _mapper = mapper;
            _userRepo = userRepo;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(UserLoginRequest request)
        {
            var user = Authenticate(request);

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
        public ActionResult<UserDto> Register(AddUserRequest request)
        {
            var currentUsers = _userRepo.GetUsers();
            if (request == null)
                return BadRequest();
            if (currentUsers.Any(c => c.Email == request.Email))
                return Conflict("A user with the same username already exists.");

            var user = _mapper.Map<PersistedUser>(request);
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
            user.CreatedDate = DateTime.Now;
            user.ModifiedDate = DateTime.Now;
            var result = _userRepo.AddUser(user);
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
        private PersistedUser? Authenticate(UserLoginRequest userLogin)
        {
            var currentUser = _userRepo.GetUserByEmail(userLogin.Email);
            if (currentUser == null) return null;
            var passwordVerificationResult = _passwordHasher.VerifyPassword(userLogin.Password, currentUser.PasswordHash);
            if (passwordVerificationResult) return currentUser;
            return null;
        }
    }    
}
