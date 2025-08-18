using MovieReview.Models.DTOs;
using MovieReview.Models.Entities;
using MovieReview.Repositories;

namespace MovieReview.Services
{
    public class MovieService: IMovieService
    {
        private readonly IMovieRepository _repository;

        public MovieService(IMovieRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<MovieReadDto>> GetAllAsync()
        {
            IEnumerable<Movie> movies = await _repository.GetAllAsync();
            return movies.Select(m => new MovieReadDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                ReleaseYear = m.ReleaseYear,
                ReviewCount = m.Reviews?.Count ?? 0,
                AverageRating = m.Reviews.Any() ? m.Reviews.Average(r => r.Rating) : 0
            });
        }

        public async Task<MovieReadDto> GetByIdAsync(long id)
        {
            Movie movie = await _repository.GetByIdWithReviewsAndRatingsAsync(id);
            if (movie == null) return null;

            return new MovieReadDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                ReviewCount = movie.Reviews?.Count ?? 0,
                AverageRating = movie.Reviews.Any() ? movie.Reviews.Average(r => r.Rating) : 0
            };
        }

        public async Task<MovieReadDto> CreateAsync(MovieCreateDto dto)
        {
            Movie movie = new Movie
            {
                Title = dto.Title,
                Description = dto.Description,
                ReleaseYear = dto.ReleaseYear
            };
            await _repository.CreateAsync(movie);

            return new MovieReadDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                ReviewCount = 0,
                AverageRating = 0
            };
        }

        public async Task<bool> UpdateAsync(long id, MovieUpdateDto dto)
        {
            Movie movie = await _repository.GetByIdAsync(id);
            if (movie == null) return false;

            movie.Title = dto.Title;
            movie.Description = dto.Description;
            movie.ReleaseYear = dto.ReleaseYear;

            await _repository.UpdateAsync(movie);
            return true;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            Movie movie = await _repository.GetByIdAsync(id);
            if (movie == null) return false;

            await _repository.DeleteAsync(movie);
            return true;
        }
    }
}
