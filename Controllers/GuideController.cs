﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TFT_API.Interfaces;
using TFT_API.Models.UserGuides;
using TFT_API.Models.AutoGeneratedGuide;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GuideController : ControllerBase
    {
        private readonly IGuideDataAccess _userGuideRepo;
        private readonly IMapper _mapper;
        private readonly IUserDataAccess _userRepo;
        public GuideController(IGuideDataAccess userGuideRepo, IMapper mapper, IUserDataAccess userRepo)
        {
            _userGuideRepo = userGuideRepo;
            _mapper = mapper;
            _userRepo = userRepo;
        }

        [HttpGet("type/{type}")]
        public ActionResult<List<UserGuideDto>> GetUserGuides(string type, [FromQuery] int userId = 0)
        {
            var userGuides = _userGuideRepo.GetUserGuides(type);
            var userGuideDtos = _mapper.Map<List<UserGuideDto>>(userGuides);

            if(userId > 0)
            {
                AddUserVotes(userGuideDtos, userId);
            }

            return Ok(_mapper.Map<List<UserGuideDto>>(userGuides));
        }

        [HttpGet("user/{userId}")]
        public ActionResult<List<UserGuideDto>> GetUserGuidesByUserId(int userId)
        {
            var userGuides = _userGuideRepo.GetUserGuidesByUserId(userId);
            var userGuideDtos = _mapper.Map<List<UserGuideDto>>(userGuides);

            AddUserVotes(userGuideDtos, userId);

            return Ok(userGuideDtos);
        }

        [HttpGet("commented/{userId}")]
        public ActionResult<List<UserGuideDto>> GetCommentedUserGuides(int userId)
        {
            var userGuides = _userGuideRepo.GetCommentedUserGuides(userId);
            var userGuideDtos = _mapper.Map<List<UserGuideDto>>(userGuides);

            AddUserVotes(userGuideDtos, userId);

            return Ok(userGuideDtos);
        }

        [HttpGet("upvoted/{userId}")]
        public ActionResult<List<UserGuideDto>> GetUpvotedUserGuides(int userId)
        {
            var userGuides = _userGuideRepo.GetUpvotedUserGuides(userId);
            var userGuideDtos = _mapper.Map<List<UserGuideDto>>(userGuides);

            AddUserVotes(userGuideDtos, userId);

            return Ok(userGuideDtos);
        }

        [HttpGet("downvoted/{userId}")]
        public ActionResult<List<UserGuideDto>> GetDownvotedUserGuides(int userId)
        {
            var userGuides = _userGuideRepo.GetDownvotedUserGuides(userId);
            var userGuideDtos = _mapper.Map<List<UserGuideDto>>(userGuides);

            AddUserVotes(userGuideDtos, userId);

            return Ok(userGuideDtos);
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetUserGuide")]
        public ActionResult<UserGuideDto> GetUserGuideById(int id)
        {
            var userGuide = _userGuideRepo.GetUserGuideById(id);
            if (userGuide == null) return NotFound();
            return Ok(_mapper.Map<UserGuideDto>(userGuide));
        }

        [Authorize]
        [HttpPost("{userId}")]
        public ActionResult<UserGuideDto> AddUserGuide(int userId, UserGuideRequest request)
        {
            if (!IsAuthorized(userId))
            {
                return Unauthorized();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request == null) return BadRequest("Userguide is invalid.");
            var user = _userRepo.GetUserById(userId);
            if (user == null) return NotFound("User was not found.");
            var newUserGuide = _mapper.Map<UserGuide>(request);
            newUserGuide.UsersUsername = user.Username;
            newUserGuide.UserId = userId;
            newUserGuide.CreatedAt = DateTime.Now;
            newUserGuide.UpdatedAt = DateTime.Now;

            if (string.IsNullOrEmpty(newUserGuide.Stage2Desc) ||
                string.IsNullOrEmpty(newUserGuide.Stage3Desc) ||
                string.IsNullOrEmpty(newUserGuide.Stage4Desc) ||
                string.IsNullOrEmpty(newUserGuide.GeneralDesc) ||
                string.IsNullOrEmpty(newUserGuide.DifficultyLevel) ||
                string.IsNullOrEmpty(newUserGuide.PlayStyle))
            {
                newUserGuide.IsDraft = true;
            }

            var result = _userGuideRepo.AddUserGuide(newUserGuide);
            return CreatedAtRoute("GetUserGuide", new { id = result.Id }, result);
        }

        [Authorize]
        [HttpPut("{userGuideId}")]
        public ActionResult<UserGuideDto> UpdateUserGuide(int userGuideId, UserGuideDto userGuide)
        {
            if (!IsAuthorized(userGuide.UserId))
            {
                return Unauthorized();
            }
            if (userGuide == null) return BadRequest();
            var currentUserGuide = _userGuideRepo.GetUserGuideById(userGuideId);
            if(currentUserGuide == null) return NotFound();
            try
            {
                var updatedUserGuide = _mapper.Map(userGuide, currentUserGuide);
                updatedUserGuide.UpdatedAt = DateTime.Now;
                var result = _userGuideRepo.UpdateUserGuide(updatedUserGuide);
                return Ok(result);
            }catch (Exception ex)
            {
                return BadRequest("Failed to modify the user guide. Error: " + ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteUserGuide(int id)
        {
            var userGuide = _userGuideRepo.GetUserGuideById(id);
            if (userGuide == null) return NotFound();
            if (!IsAuthorized(userGuide.UserId))
            {
                return Unauthorized();
            }    
            _userGuideRepo.DeleteUserGuide(id);
            return NoContent();
        }

        [HttpGet("autogenerated")]
        public ActionResult<List<AutoGeneratedGuideDto>> GetAutoGeneratedGuides([FromQuery] int userId = 0)
        {
            var autoGeneratedGuides = _userGuideRepo.GetAutoGeneratedGuides();
            var autoGeneratedGuideDtos = _mapper.Map<List<AutoGeneratedGuideDto>>(autoGeneratedGuides);

            if (userId > 0)
            {
                AddUserVotes(autoGeneratedGuideDtos, userId);
            }

            return Ok(autoGeneratedGuideDtos);
        }

        private void AddUserVotes<T>(List<T> guides, int userId) where T : IGuideDto
        {
            var userVotes = _userGuideRepo.GetUserVotes(userId);
            var guideVotes = userVotes.ToDictionary(v => v.UserGuideId, v => v.IsUpvote);

            foreach (var guide in guides)
            {
                if (guideVotes.TryGetValue(guide.Id, out bool value))
                {
                    guide.IsUpvote = value;
                }
                else
                {
                    guide.IsUpvote = null;
                }
            }
        }

        private bool IsAuthorized(int? userId)
        {
            if(userId == null) return false;
            var userIdentityClaim = HttpContext.User.FindFirst("UserId");
            if (userIdentityClaim == null || !int.TryParse(userIdentityClaim.Value, out var userIdFromToken))
            {
                return false;
            }
            return userIdFromToken == userId;
        }
    }   
}
