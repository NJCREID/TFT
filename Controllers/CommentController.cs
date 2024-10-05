using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TFT_API.Filters;
using TFT_API.Interfaces;
using TFT_API.Models.UserGuides;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ValidateRequestBody]
    public class CommentController(ICommentDataAccess commentRepo, IUserDataAccess userRepo, IGuideDataAccess guideRepo, IMapper mapper) : ControllerBase
    {

        private readonly ICommentDataAccess _commentRepo = commentRepo;
        private readonly IUserDataAccess _userRepo = userRepo; 
        private readonly IMapper _mapper = mapper;
        private readonly IGuideDataAccess _guideRepo = guideRepo;

        /// <summary>
        /// Retrieves a comment by its ID.
        /// </summary>
        /// <param name="id">The ID of the comment</param>
        /// <returns>The comment with the specified ID</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// GET /api/comments/1
        ///
        /// </remarks>
        /// <response code="200">Returns the comment with the specified ID</response>
        /// <response code="404">If the specified comment ID is not found</response>
        [Authorize]
        [HttpGet("{id}", Name = "GetComment")]
        public async Task<ActionResult<CommentDto>> GetCommentByIdAsync([FromRoute] int id)
        {
            var comment = await  _commentRepo.GetCommentByIdAsync(id);
            if (comment == null) return NotFound($"Comment with ID {id} not found.");
            return Ok(_mapper.Map<CommentDto>(comment));
        }

        /// <summary>
        /// Adds a new comment to a user guide.
        /// </summary>
        /// <param name="request">The request containing the comment details</param>
        /// <returns>The created comment</returns>
        /// <remarks>
        /// Sample request:
        ///
        /// POST /api/comments
        /// {
        ///     "userId": 1,
        ///     "userGuideId": 2,
        ///     "content": "This is a comment."
        /// }
        /// </remarks>
        /// <response code="201">Returns the created comment</response>
        /// <response code="404">If the specified user or guide is not found</response>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CommentDto>> AddCommentAsync([FromBody] CommentRequest request)
        {
            var existingUser = await _userRepo.GetUserByIdAsync(request.UserId);
            if (existingUser == null) return NotFound("User not found.");
            var existingGuide = await _guideRepo.GetFullUserGuideByIdAsync(request.UserGuideId);
            if (existingGuide == null) return NotFound("Guide not found.");

            var comment = _mapper.Map<Comment>(request);
            comment.User = existingUser;
            comment.UserGuide = existingGuide;
            comment.Author = existingUser.Username;
            comment.UpdatedAt = DateTime.Now;
            comment.CreatedAt = DateTime.Now;
            var result = _mapper.Map<CommentDto>(await _commentRepo.AddCommentAsync(comment));

            existingUser.CommentsCount++;
            await _userRepo.UpdateUserAsync(existingUser);

            return CreatedAtRoute("GetComment", new { id = result.Id }, result);
        }
    }
}
