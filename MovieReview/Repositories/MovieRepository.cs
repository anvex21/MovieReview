using Microsoft.EntityFrameworkCore;
using MovieReview.Data;
using MovieReview.Models.Entities;

namespace MovieReview.Repositories
{
    public class MovieRepository : IMovieRepository
    {
        private readonly MovieReviewDbContext _context;

        public MovieRepository(MovieReviewDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Movie>> GetAllAsync()
        {
            return await _context.Movies
                .Include(m=>m.Reviews)
                .ToListAsync();
        }

        public async Task<Movie> GetByIdAsync(long id)
        {
            return await _context.Movies.FindAsync(id);
        }

        public async Task<Movie> GetByIdWithReviewsAndRatingsAsync(long id)
        {
            return await _context.Movies
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task CreateAsync(Movie movie)
        {
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Movie movie)
        {
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Movie movie)
        {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }
    }
}
