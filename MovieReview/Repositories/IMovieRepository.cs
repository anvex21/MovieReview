using MovieReview.Models.Entities;

namespace MovieReview.Repositories
{
    public interface IMovieRepository
    {
        Task<IEnumerable<Movie>> GetAllAsync();
        Task<Movie> GetByIdAsync(long id);
        Task<Movie> GetByIdWithReviewsAndRatingsAsync(long id);
        Task CreateAsync(Movie movie);
        Task UpdateAsync(Movie movie);
        Task DeleteAsync(Movie movie);
    }
}
