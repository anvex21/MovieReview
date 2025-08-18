using Microsoft.EntityFrameworkCore;
using MovieReview.Data;
using MovieReview.Models.DTOs;
using MovieReview.Models.Entities;

namespace MovieReview.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieReviewDbContext _context;

        // constructor
        public MovieRepository(MovieReviewDbContext context)
        {
            _context = context;
        }

        // get all movies
        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            return await _context.Movies
                .Include(m=>m.Reviews)
                .ToListAsync();
        }

        // get all movies with parameters for pagination/filtering/sorting
        public async Task<IEnumerable<Movie>> GetAllAsync(MovieQueryDto queryParams)
        {
            IQueryable<Movie> movies = _context.Movies.AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(queryParams.Name))
            {
                movies = movies.Where(m => m.Title.Contains(queryParams.Name));
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(queryParams.SortBy))
            {
                switch (queryParams.SortBy.ToLower())
                {
                    case "name":
                        movies = queryParams.IsDescending
                            ? movies.OrderByDescending(m => m.Title)
                            : movies.OrderBy(m => m.Title);
                        break;
                    case "releaseyear":
                        movies = queryParams.IsDescending
                            ? movies.OrderByDescending(m => m.ReleaseYear)
                            : movies.OrderBy(m => m.ReleaseYear);
                        break;
                    default:
                        break;
                }
            }

            // Pagination
            movies = movies
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize);

            return await movies.ToListAsync();
        }

        // get a movie by its id
        public async Task<Movie> GetByIdAsync(long id)
        {
            return await _context.Movies.FindAsync(id);
        }

        // get a movie by its id including the reviews
        public async Task<Movie> GetByIdWithReviewsAndRatingsAsync(long id)
        {
            return await _context.Movies
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        // add a movie
        public async Task CreateAsync(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
        }

        // update a movie
        public async Task UpdateAsync(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }

        // delete a movie
        public async Task DeleteAsync(Movie movie)
        {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }

        // get top rated movies
        public async Task<IEnumerable<Movie>> GetTopRatedAsync(int count)
        {
            return await _context.Movies
                .Include(m => m.Reviews)
                .OrderByDescending(m => m.Reviews.Any() ? m.Reviews.Average(r => r.Rating) : 0)
                .Take(count)
                .ToListAsync();
        }

        // get movies released in a certain year
        public async Task<IEnumerable<Movie>> GetByYearAsync(int year)
        {
            return await _context.Movies
                .Include(m => m.Reviews)
                .Where(m => m.ReleaseYear == year)
                .ToListAsync();
        }
    }
}
