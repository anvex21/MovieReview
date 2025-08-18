using MovieReview.Models.DTOs;
using MovieReview.Models.Entities;
using MovieReview.Repositories;

namespace MovieReview.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        // constructor
        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        // get a review by its id
        public async Task<ReviewDto> GetByIdAsync(long id)
        {
            Review review = await _reviewRepository.GetByIdAsync(id);
            return review == null ? null : MapToDto(review);
        }

        // get a review by its movie id
        public async Task<IEnumerable<ReviewDto>> GetByMovieIdAsync(long movieId)
        {
            IEnumerable<Review> reviews = await _reviewRepository.GetByMovieIdAsync(movieId);
            return reviews.Select(MapToDto);
        }

        // get a review by its user id
        public async Task<IEnumerable<ReviewDto>> GetByUserIdAsync(long userId)
        {
            IEnumerable<Review> reviews = await _reviewRepository.GetByUserIdAsync(userId);
            return reviews.Select(MapToDto);
        }

        // add a review
        public async Task<ReviewDto> AddAsync(ReviewCreateDto dto, long userId)
        {
            Review review = new Review
            {
                Content = dto.Content,
                Rating = dto.Rating,
                MovieId = dto.MovieId,
                UserId = userId
            };

            await _reviewRepository.AddAsync(review);
            return MapToDto(review);
        }

        // update a review
        public async Task UpdateAsync(long id, ReviewUpdateDto dto, long userId)
        {
            Review review = await _reviewRepository.GetByIdAsync(id);
            if (review == null || review.UserId != userId)
                throw new UnauthorizedAccessException("You cannot edit this review.");

            review.Content = dto.Content;
            review.Rating = dto.Rating;

            await _reviewRepository.UpdateAsync(review);
        }

        // delete a review
        public async Task DeleteAsync(long id, long userId)
        {
            Review review = await _reviewRepository.GetByIdAsync(id);
            if (review == null || review.UserId != userId)
                throw new UnauthorizedAccessException("You cannot delete this review.");

            await _reviewRepository.DeleteAsync(review);
        }

        // mapping
        private static ReviewDto MapToDto(Review review) =>
            new ReviewDto
            {
                Id = review.Id,
                Content = review.Content,
                Rating = review.Rating,
                MovieId = review.MovieId,
                UserId = review.UserId
            };
    }
}
