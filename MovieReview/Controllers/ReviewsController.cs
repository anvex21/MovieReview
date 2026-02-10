using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> GetById(long id)
        {
            var review = await _reviewService.GetByIdAsync(id);
            return Ok(review);
        }

        [HttpGet("GetByMovieId/{movieId}")]
        public async Task<IActionResult> GetByMovie(long movieId)
        {
            var reviews = await _reviewService.GetByMovieIdAsync(movieId);
            return Ok(reviews);
        }

        [HttpGet("GetByUserId/{userId}")]
        public async Task<IActionResult> GetByUser(long userId)
        {
            var reviews = await _reviewService.GetByUserIdAsync(userId);
            return Ok(reviews);
        }

        [HttpPost("AddReview")]
        public async Task<IActionResult> Create([FromBody] ReviewCreateDto dto)
        {
            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var review = await _reviewService.AddAsync(dto, userId);
            return CreatedAtAction(nameof(GetById), new { id = review.Id }, review);
        }

        [HttpPut("UpdateReview/{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] ReviewUpdateDto dto)
        {
            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _reviewService.UpdateAsync(id, dto, userId);
            return NoContent();
        }

        [HttpDelete("DeleteReview/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            long userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _reviewService.DeleteAsync(id, userId);
            return NoContent();
        }
    }
}
