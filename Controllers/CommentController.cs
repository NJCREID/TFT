using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TFT_API.Interfaces;
using TFT_API.Models.UserGuides;

namespace TFT_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {

        private readonly ICommentDataAccess _commentRepo;
        private readonly IUserDataAccess _userRepo; 
        private readonly IMapper _mapper;

        public CommentController(ICommentDataAccess commentRepo,IUserDataAccess userRepo, IMapper mapper)
        {
            _commentRepo = commentRepo;
            _userRepo = userRepo;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("{id}", Name = "GetComment")]
        public ActionResult<CommentDto> GetCommentById(int id)
        {
            var comment = _commentRepo.GetCommentById(id);
            if (comment == null) return NotFound();
            return Ok(_mapper.Map<CommentDto>(comment));
        }

        [Authorize]
        [HttpPost]
        public ActionResult<CommentDto> AddComment(CommentRequest request)
        {
            if (request == null) return BadRequest();
            var existingUser = _userRepo.GetUserById(request.UserId);
            if (existingUser == null) return BadRequest();
            var comment = _mapper.Map<Comment>(request);
            comment.Author = existingUser.Username;
            comment.UpdatedAt = DateTime.Now;
            comment.CreatedAt = DateTime.Now;
            var result = _mapper.Map<CommentDto>(_commentRepo.AddComment(comment));
            return CreatedAtRoute("GetComment", new { id = result.Id }, result);
        }
    }
}
