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

        /// <summary>
        /// Retrieves votes associated with the current user.
        /// </summary>
        /// <returns>A list of votes made by the user</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/votes/user
        ///
        /// </remarks>
        /// <response code="200">Returns a list of votes made by the user</response>
        /// <response code="401">If the user is not authorized</response>
        /// <response code="404">If no votes are found for the user</response>
        [Authorize]
        [Authorize]
        [HttpGet("user")]
        public async Task<ActionResult<List<VoteDto>>> GetVotesByUserId()
        {
            if (!int.TryParse(User.FindFirstValue("UserId"), out int userId)) return Unauthorized();
            var votes = await _voteRepo.GetVotesByUserIdAsync(userId);
            if(votes == null || votes.Count == 0) return NotFound("No Votes Found.");
            return Ok(votes);
        }

        /// <summary>
        /// Retrieves the voting status for a specified user and post.
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="postId">The ID of the post</param>
        /// <returns>The voting status for the specified user and post</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/votes/status?userId=1&postId=2
        ///
        /// </remarks>
        /// <response code="200">Returns the voting status</response>
        /// <response code="400">If the userId or postId is invalid</response>
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


        /// <summary>
        /// Adds or updates a vote for a specified user and guide.
        /// </summary>
        /// <param name="request">The vote request containing vote information</param>
        /// <returns>The updated vote response</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// POST /api/votes
        /// {
        ///     "UserId": 1,
        ///     "UserGuideId": 2,
        ///     "IsUpvote": true
        /// }
        /// </remarks>
        /// <response code="200">Returns the updated vote response</response>
        /// <response code="404">If the specified guide or user is not found</response>
        /// <response code="400">If the request isnt valid</response>
        /// <response code="500">If there is an internal server error</response>
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
            try
            {
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
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to update vote. Error: " + ex.Message);
            }
        }

        /// <summary>
        /// Retrieves a vote by its ID.
        /// </summary>
        /// <param name="id">The ID of the vote</param>
        /// <returns>The vote with the specified ID</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/votes/1
        ///
        /// </remarks>
        /// <response code="200">Returns the vote with the specified ID</response>
        /// <response code="404">If the specified vote ID is not found</response>
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
