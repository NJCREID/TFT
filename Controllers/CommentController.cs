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
    public class CommentController(ICommentDataAccess commentRepo, IUserDataAccess userRepo, IMapper mapper) : ControllerBase
    {

        private readonly ICommentDataAccess _commentRepo = commentRepo;
        private readonly IUserDataAccess _userRepo = userRepo; 
        private readonly IMapper _mapper = mapper;

        [Authorize]
        [HttpGet("{id}", Name = "GetComment")]
        public async Task<ActionResult<CommentDto>> GetCommentByIdAsync([FromRoute] int id)
        {
            var comment = await  _commentRepo.GetCommentByIdAsync(id);
            if (comment == null) return NotFound($"Comment with ID {id} not found.");
            return Ok(_mapper.Map<CommentDto>(comment));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<CommentDto>> AddCommentAsync([FromBody] CommentRequest request)
        {
            var existingUser = await _userRepo.GetUserByIdAsync(request.UserId);
            if (existingUser == null) return NotFound("User not found.");

            var comment = _mapper.Map<Comment>(request);
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
