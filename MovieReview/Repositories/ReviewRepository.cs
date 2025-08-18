using Microsoft.EntityFrameworkCore;
using MovieReview.Data;
using MovieReview.Models.Entities;

namespace MovieReview.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly MovieReviewDbContext _context;

        public ReviewRepository(MovieReviewDbContext context)
        {
            _context = context;
        }

        public async Task<Review> GetByIdAsync(long id)
        {
            return await _context.Reviews
                .Include(r => r.Movie)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Review>> GetByMovieIdAsync(long movieId)
        {
            return await _context.Reviews
                .Where(r => r.MovieId == movieId)
                .Include(r => r.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByUserIdAsync(long userId)
        {
            return await _context.Reviews
                .Where(r => r.UserId == userId)
                .Include(r => r.Movie)
                .ToListAsync();
        }

        public async Task<Review> AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Review review)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
