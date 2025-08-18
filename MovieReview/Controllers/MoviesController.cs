using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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


        /// <summary>
        /// Get all movies
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllMovies")]
        public async Task<IActionResult> GetAll()
        {
            var movies = await _service.GetAllAsync();
            if (!movies.Any())
            {
                return NotFound("No movies found.");
            }
            return Ok(movies);
        }

        /// <summary>
        /// Get all with filtering/sorting/pagination
        /// </summary>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        [HttpGet("GetAllMoviesWithQuery")]
        public async Task<IActionResult> GetAllMovies([FromQuery] MovieQueryDto queryParams)
        {
            IEnumerable<MovieReadDto> movies = await _service.GetAllAsync(queryParams);
            return Ok(movies);
        }

        /// <summary>
        /// Get movie by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(long id)
        {
            var movie = await _service.GetByIdAsync(id);
            if (movie is null) return NotFound("No such movie found.");
            return Ok(movie);
        }

        /// <summary>
        /// Add a movie
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("AddMovie")]
        public async Task<IActionResult> Create([FromBody] MovieCreateDto dto)
        {
            var movie = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = movie.Id }, movie);
        }

        /// <summary>
        /// Update a movie
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPut("UpdateMovie/{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] MovieUpdateDto dto)
        {
            var success = await _service.UpdateAsync(id, dto);
            if (!success) return NotFound("Something went wrong.");
            return NoContent();
        }

        /// <summary>
        /// Delete a movie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteMovie/{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound("No such movie.");
            return NoContent();
        }
    }
}
