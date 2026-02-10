using MovieReview.Models.Entities;

namespace MovieReview.Repositories
{
    public interface IReviewRepository
    {
        Task<Review?> GetByIdAsync(long id);
        Task<IEnumerable<Review>> GetByMovieIdAsync(long movieId);
        Task<IEnumerable<Review>> GetByUserIdAsync(long userId);
        Task<Review> AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(Review review);
    }
}
