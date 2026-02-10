using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieReview.Models.DTOs;
using MovieReview.Services;
using System.Security.Claims;

namespace MovieReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<ReviewDto>> GetById(long id)
        {
            ReviewDto? review = await _reviewService.GetByIdAsync(id);
            if (review == null) return NotFound();
            return Ok(review);
        }

        [HttpGet("GetByMovieId/{movieId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetByMovie(long movieId)
        {
            return Ok(await _reviewService.GetByMovieIdAsync(movieId));
        }

        [HttpGet("GetByUserId/{userId}")]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetByUser(long userId)
        {
            return Ok(await _reviewService.GetByUserIdAsync(userId));
        }

        [HttpPost("AddReview")]
        public async Task<ActionResult<ReviewDto>> Create([FromBody] ReviewCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("NameIdentifier claim missing"));
            ReviewDto review = await _reviewService.AddAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
        }

        [HttpPut("UpdateReview/{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] ReviewUpdateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("NameIdentifier claim missing"));
            await _reviewService.UpdateAsync(id, dto, userId);
            return NoContent();
        }

        [HttpDelete("DeleteReview/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new InvalidOperationException("NameIdentifier claim missing"));
            await _reviewService.DeleteAsync(id, userId);
            return NoContent();
        }
    }
}
