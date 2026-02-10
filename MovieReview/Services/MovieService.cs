using MovieReview.Models.DTOs;
using MovieReview.Models.Entities;
using MovieReview.Repositories;

namespace MovieReview.Services
{
    public class MovieService: IMovieService
    {
        private readonly IMovieRepository _repository;
        private readonly IExternalMovieService _externalMovieService;

        // constructor
        public MovieService(IMovieRepository repository, IExternalMovieService externalMovieService)
        {
            _repository = repository;
            _externalMovieService = externalMovieService;
        }

        // get all movies
        public async Task<IEnumerable<MovieReadDto>> GetAllAsync()
        {
            IEnumerable<Movie> movies = await _repository.GetAllAsync();
            var tasks = movies.Select(async m => new MovieReadDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                ReleaseYear = m.ReleaseYear,
                ReviewCount = m.Reviews?.Count ?? 0,
                AverageRating = (m.Reviews != null && m.Reviews.Any()) ? m.Reviews.Average(r => r.Rating) : 0,
                ImdbRating = await _externalMovieService.GetImdbRatingAsync(m.Title)
            });

            return await Task.WhenAll(tasks);
        }

        // get all movies with parameters
        public async Task<IEnumerable<MovieReadDto>> GetAllAsync(MovieQueryDto queryParams)
        {
            IEnumerable<Movie> movies = await _repository.GetAllAsync(queryParams);

            var tasks = movies.Select(async m => new MovieReadDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                ReleaseYear = m.ReleaseYear,
                ReviewCount = m.Reviews?.Count ?? 0,
                AverageRating = m.Reviews != null && m.Reviews.Any() ? m.Reviews.Average(r => r.Rating) : 0,
                ImdbRating = await _externalMovieService.GetImdbRatingAsync(m.Title)
            });

            return await Task.WhenAll(tasks);
        }

        // get a movie by its id
        public async Task<MovieReadDto?> GetByIdAsync(long id)
        {
            Movie? movie = await _repository.GetByIdWithReviewsAndRatingsAsync(id);
            if (movie == null) return null;

            var imdbRating = await _externalMovieService.GetImdbRatingAsync(movie.Title);

            return new MovieReadDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                ReviewCount = movie.Reviews?.Count ?? 0,
                AverageRating = (movie.Reviews != null && movie.Reviews.Any()) ? movie.Reviews.Average(r => r.Rating) : 0,
                ImdbRating = imdbRating
            };
        }

        // add a movie
        public async Task<MovieReadDto> CreateAsync(MovieCreateDto dto)
        {
            Movie movie = new Movie
            {
                Title = dto.Title,
                Description = dto.Description,
                ReleaseYear = dto.ReleaseYear
            };
            await _repository.CreateAsync(movie);

            var imdbRating = await _externalMovieService.GetImdbRatingAsync(movie.Title);

            return new MovieReadDto
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                ReviewCount = 0,
                AverageRating = 0,
                ImdbRating = imdbRating
            };
        }

        // update a movie
        public async Task<bool> UpdateAsync(long id, MovieUpdateDto dto)
        {
            Movie? movie = await _repository.GetByIdAsync(id);
            if (movie == null) return false;

            movie.Title = dto.Title;
            movie.Description = dto.Description;
            movie.ReleaseYear = dto.ReleaseYear;

            await _repository.UpdateAsync(movie);
            return true;
        }

        // delete a movie
        public async Task<bool> DeleteAsync(long id)
        {
            Movie? movie = await _repository.GetByIdAsync(id);
            if (movie == null) return false;

            await _repository.DeleteAsync(movie);
            return true;
        }

        // get top rated movies
        public async Task<IEnumerable<MovieReadDto>> GetTopRatedAsync(int count)
        {
            IEnumerable<Movie> movies = await _repository.GetTopRatedAsync(count);
            var tasks = movies.Select(async m => new MovieReadDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                ReleaseYear = m.ReleaseYear,
                ReviewCount = m.Reviews?.Count ?? 0,
                AverageRating = (m.Reviews != null && m.Reviews.Any()) ? m.Reviews.Average(r => r.Rating) : 0,
                ImdbRating = await _externalMovieService.GetImdbRatingAsync(m.Title)
            });

            return await Task.WhenAll(tasks);
        }

        // get movies released in a certain year
        public async Task<IEnumerable<MovieReadDto>> GetByYearAsync(int year)
        {
            IEnumerable<Movie> movies = await _repository.GetByYearAsync(year);
            var tasks = movies.Select(async m => new MovieReadDto
            {
                Id = m.Id,
                Title = m.Title,
                Description = m.Description,
                ReleaseYear = m.ReleaseYear,
                ReviewCount = m.Reviews?.Count ?? 0,
                AverageRating = (m.Reviews != null && m.Reviews.Any()) ? m.Reviews.Average(r => r.Rating) : 0,
                ImdbRating = await _externalMovieService.GetImdbRatingAsync(m.Title)
            });

            return await Task.WhenAll(tasks);
        }
    }
}
