using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TFT_API.Filters;
using TFT_API.Interfaces;
using TFT_API.Models.Votes;

namespace TFT_API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [ValidateRequestBody]
    public class VoteController(IVoteDataAccess voteRepo, IMapper mapper, IGuideDataAccess guideRepo, IUserDataAccess userRepo) : ControllerBase
    {
        private readonly IVoteDataAccess _voteRepo = voteRepo;
        private readonly IGuideDataAccess _guideRepo = guideRepo;
        private readonly IUserDataAccess _userRepo = userRepo;
        private readonly IMapper _mapper = mapper;

        [Authorize]
        [HttpGet("user")]
        public async Task<ActionResult<List<VoteDto>>> GetVotesByUserId()
        {
            if (!int.TryParse(User.FindFirstValue("UserId"), out int userId)) return Unauthorized();
            var votes = await _voteRepo.GetVotesByUserIdAsync(userId);
            if(votes == null || votes.Count == 0) return NotFound("No Votes Found.");
            return Ok(votes);
        }

        [Authorize]
        [HttpGet("status")]
        public async Task<ActionResult<VoteDto>> GetVoteStatus([FromQuery] int userId, [FromQuery] int postId)
        {
            if (userId <= 0 || postId <= 0)
            {
                return BadRequest("Invalid userId or postId.");
            }
            var vote = await _voteRepo.GetVoteStatusAsync(userId, postId);

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
        public async Task<ActionResult> AddVote([FromBody] VoteRequest request)
        {
            var existingVote = await _voteRepo.GetVoteStatusAsync(request.UserId, request.UserGuideId);
            var guide = await _guideRepo.GetFullUserGuideByIdAsync(request.UserGuideId);
            if (guide == null) return NotFound("No Guide Found.");

            var user = await _userRepo.GetUserByIdAsync(request.UserId);
            if (user == null) return NotFound("No User Found");
            bool? isUpvote = request.IsUpvote;

            if (existingVote == null)
            {
                var newVote = _mapper.Map<Vote>(request);
                newVote.User = user;
                newVote.UserGuide = guide;
                newVote.CreatedAt = DateTime.Now;
                newVote.UpdatedAt = DateTime.Now;
                await _voteRepo.AddVoteAsync(newVote);

                if (request.IsUpvote)
                {
                    guide.UpVotes++;
                    user.UpVotesCount++;
                }
                else
                {
                    guide.DownVotes++;
                    user.DownVotesCount++;
                }
            }
            else
            {
                if (existingVote.IsUpvote == request.IsUpvote)
                {
                    await _voteRepo.DeleteVoteAsync(existingVote.Id);

                    if (request.IsUpvote)
                    {
                        guide.UpVotes--;
                        user.UpVotesCount--;    
                    }
                    else
                    {
                        guide.DownVotes--;
                        user.DownVotesCount--;
                    }
                    isUpvote = null;
                }
                else
                {
                    existingVote.IsUpvote = request.IsUpvote;
                    existingVote.UpdatedAt = DateTime.Now;
                    await _voteRepo.UpdateVoteAsync(existingVote);

                    if (request.IsUpvote)
                    {
                        guide.UpVotes++;
                        guide.DownVotes--;
                        user.UpVotesCount++;
                        user.DownVotesCount--;
                    }
                    else
                    {
                        guide.DownVotes++;
                        guide.UpVotes--;
                        user.DownVotesCount++;
                        user.UpVotesCount--;
                    }
                }
            }

            await _guideRepo.UpdateUserGuideAsync(guide);
            await _userRepo.UpdateUserAsync(user);

            return Ok(new VoteResponse { UpVotes = guide.UpVotes, DownVotes = guide.DownVotes, IsUpvote = isUpvote });
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetVote")]
        public async Task<ActionResult<VoteDto>> GetVoteById([FromRoute] int id)
        {
            var vote = await _voteRepo.GetVoteByIdAsync(id);
            if (vote == null) return NotFound("No vote found.");
            return Ok(vote);
        }
    }
}
