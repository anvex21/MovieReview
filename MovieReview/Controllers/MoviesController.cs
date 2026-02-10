using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieReview.Models.DTOs;
using MovieReview.Services;

namespace MovieReview.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        private readonly IMovieService _service;

        public MoviesController(IMovieService service)
        {
            _service = service;
        }

        [HttpGet("GetAllMovies")]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _service.GetAllAsync();
            return Ok(movies);
        }

        [HttpGet("GetAllMoviesWithQuery")]
        public async Task<IActionResult> GetAllMovies([FromQuery] MovieQueryDto queryParams)
        {
            var movies = await _service.GetAllAsync(queryParams);
            return Ok(movies);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var movie = await _service.GetByIdAsync(id);
            return Ok(movie);
        }

        [HttpGet("GetTopRatedMovies")]
        public async Task<IActionResult> GetTopRatedMovies(int count)
        {
            var movies = await _service.GetTopRatedAsync(count);
            return Ok(movies);
        }

        [HttpGet("GetMoviesByYear/{year}")]
        public async Task<IActionResult> GetMoviesByYear(int year)
        {
            var movies = await _service.GetByYearAsync(year);
            return Ok(movies);
        }

        [HttpPost("AddMovie")]
        public async Task<IActionResult> Create([FromBody] MovieCreateDto dto)
        {
            var movie = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);
        }

        [HttpPut("UpdateMovie/{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] MovieUpdateDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("DeleteMovie/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
