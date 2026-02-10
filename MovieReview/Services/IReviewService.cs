using MovieReview.Models.DTOs;

namespace MovieReview.Services
{
    public interface IReviewService
    {
        Task<ReviewDto?> GetByIdAsync(long id);
        Task<IEnumerable<ReviewDto>> GetByMovieIdAsync(long movieId);
        Task<IEnumerable<ReviewDto>> GetByUserIdAsync(long userId);
        Task<ReviewDto> AddAsync(ReviewCreateDto dto, long userId);
        Task UpdateAsync(long id, ReviewUpdateDto dto, long userId);
        Task DeleteAsync(long id, long userId);
    }
}
