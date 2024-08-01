using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using TFT_API.Interfaces;
using TFT_API.Models.Votes;

namespace TFT_API.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class VoteController : ControllerBase
    {
        private readonly IVoteDataAccess _voteRepo;
        private readonly IGuideDataAccess _guideRepo;
        private readonly IUserDataAccess _userRepo;
        private readonly IMapper _mapper;
        public VoteController(IVoteDataAccess voteRepo, IMapper mapper, IGuideDataAccess guideRepo, IUserDataAccess userRepo)
        {
            _voteRepo = voteRepo;
            _guideRepo = guideRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }
        [HttpGet("user/{userId}")]
        public ActionResult<List<VoteDto>> GetVotesByUserId(int userId)
        {
            var votes = _voteRepo.GetVotesByUserId(userId);
            return Ok(_mapper.Map<List<VoteDto>>(votes));        
        }

        [HttpGet("status")]
        public ActionResult<VoteDto> GetVoteStatus([FromQuery] int userId, [FromQuery] int postId)
        {
            if (userId <= 0 || postId <= 0)
            {
                return BadRequest("Invalid userId or postId.");
            }
            var vote = _voteRepo.GetVoteStatus(userId, postId);

            var voteDto = vote == null
            ? new VoteDto
            {
                UserId = userId,
                UserGuideId = postId,
                IsUpvote = null,
            }
            : _mapper.Map<VoteDto>(vote); 
            return Ok(voteDto);
        }

        [Authorize]
        [HttpPost]
        public ActionResult AddVote(VoteRequest request)
        {
            if(!IsAuthorized(request.UserId)) 
            {
                return Unauthorized();
            }
            if (request == null) return BadRequest("Request can not be null");

            var existingVote = _voteRepo.GetVoteStatus(request.UserId, request.UserGuideId);
            if (existingVote == null)
            {
                var newVote = _mapper.Map<Vote>(request);
                newVote.CreatedAt = DateTime.Now;
                newVote.UpdatedAt = DateTime.Now;
                var result = _voteRepo.AddVote(newVote);

                var guide = _guideRepo.GetUserGuideById(request.UserGuideId);
                if (guide != null)
                {
                    if (request.IsUpvote)
                    {
                        guide.UpVotes++;
                    }
                    else
                    {
                        guide.DownVotes++;
                    }
                    _guideRepo.UpdateUserGuide(guide);
                }
                var user = _userRepo.GetUserById(request.UserId);
                if (user != null)
                {
                    if (request.IsUpvote)
                    {
                        user.UpVotesCount++;
                    }
                    else
                    {
                        user.DownVotesCount++;
                    }
                    _userRepo.UpdateUser(user);
                }
                return Ok(new VoteResponse { UpVotes = guide?.UpVotes ?? 0, DownVotes = guide?.DownVotes ?? 0, IsUpvote = request.IsUpvote });
            } else
            {
                if (existingVote.IsUpvote == request.IsUpvote)
                {
                    _voteRepo.DeleteVote(existingVote.Id);
                    var guide = _guideRepo.GetUserGuideById(request.UserGuideId);
                    if (guide != null)
                    {
                        if (request.IsUpvote)
                        {
                            guide.UpVotes--;
                        }
                        else
                        {
                            guide.DownVotes--;
                        }
                        _guideRepo.UpdateUserGuide(guide);
                    }
                    var user = _userRepo.GetUserById(request.UserId);
                    if (user != null)
                    {
                        if (request.IsUpvote)
                        {
                            user.UpVotesCount--;
                        }
                        else
                        {
                            user.DownVotesCount--;
                        }
                        _userRepo.UpdateUser(user);
                    }
                    return Ok(new VoteResponse { UpVotes = guide?.UpVotes ?? 0, DownVotes = guide?.DownVotes ?? 0, IsUpvote = null });
                }
                else
                {
                    existingVote.IsUpvote = request.IsUpvote;
                    existingVote.UpdatedAt = DateTime.Now;
                    var result = _voteRepo.UpdateVote(existingVote);

                    var guide = _guideRepo.GetUserGuideById(request.UserGuideId);
                    if (guide != null)
                    {
                        if (request.IsUpvote)
                        {
                            guide.UpVotes++;
                            guide.DownVotes--;
                        }
                        else
                        {
                            guide.DownVotes++;
                            guide.UpVotes--;
                        }
                        _guideRepo.UpdateUserGuide(guide);
                    }
                    var user = _userRepo.GetUserById(request.UserId);
                    if (user != null)
                    {
                        if (request.IsUpvote)
                        {
                            user.UpVotesCount++;
                            user.DownVotesCount--;
                        }
                        else
                        {
                            user.DownVotesCount++;
                            user.UpVotesCount--;
                        }
                        _userRepo.UpdateUser(user);
                    }
                    return Ok(new VoteResponse { UpVotes = guide?.UpVotes ?? 0, DownVotes = guide?.DownVotes ?? 0, IsUpvote = request.IsUpvote });
                }
            }
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetVote")]
        public ActionResult<VoteDto> GetVoteById(int id)
        {
            var vote = _voteRepo.GetVoteById(id);
            return Ok(_mapper.Map<VoteDto>(vote));
        }

        [Authorize]
        [HttpPut("{userId}")]
        public ActionResult<VoteDto> UpdateVote(int userId,VoteDto request)
        {
            if (!IsAuthorized(userId))
            {
                return Unauthorized();
            }
            if (request == null) return BadRequest();
            var currentVote = _voteRepo.GetVoteById(request.Id);
            if (currentVote == null) return NotFound();
            try
            {
                var updatedVote = _mapper.Map<Vote>(request);
                updatedVote.CreatedAt = currentVote.CreatedAt;
                updatedVote.UpdatedAt = DateTime.Now;
                var result = _voteRepo.UpdateVote(updatedVote);
                return Ok(result);
            }catch (Exception ex)
            {
                return BadRequest("Failed to modify the vote. Error: " + ex.Message);
            }
        }
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteVote(int id)
        {
            var vote = _voteRepo.GetVoteById(id);
            if (vote == null) return NotFound();
            if (!IsAuthorized(vote.UserId))
            {
                return Unauthorized();
            }
            _voteRepo.DeleteVote(id);
            return NoContent();
        }
        private bool IsAuthorized(int userId)
        {
            var userIdentityClaim = HttpContext.User.FindFirst("UserId");
            if (userIdentityClaim == null || !int.TryParse(userIdentityClaim.Value, out var userIdFromToken))
            {
                return false;
            }
            return userIdFromToken == userId;
        }
    }
}
