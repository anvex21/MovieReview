using Moq;
using NUnit.Framework;
using MovieReview.Services;
using MovieReview.Repositories;
using MovieReview.Models.Entities;
using MovieReview.Models.DTOs;
using System.Collections.Generic;

namespace MovieReview.Tests
{
    [TestFixture]
    public class ReviewServiceTests
    {
        private Mock<IReviewRepository> _repoMock = null!;
        private ReviewService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IReviewRepository>();
            _service = new ReviewService(_repoMock.Object);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsDto_WhenReviewExists()
        {
            var review = new Review { Id = 1, Content = "Great!", Rating = 9, MovieId = 1, UserId = 1, User = new User { UserName = "alice" } };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);

            var result = await _service.GetByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.Content, Is.EqualTo("Great!"));
            Assert.That(result.Rating, Is.EqualTo(9));
            Assert.That(result.UserName, Is.EqualTo("alice"));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenReviewDoesNotExist()
        {
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Review?)null);

            var result = await _service.GetByIdAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByMovieIdAsync_ReturnsMappedDtos()
        {
            var reviews = new List<Review> { new Review { Id = 1, Content = "Good", Rating = 8, MovieId = 1, UserId = 1 } };
            _repoMock.Setup(r => r.GetByMovieIdAsync(1)).ReturnsAsync(reviews);

            var result = (await _service.GetByMovieIdAsync(1)).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Content, Is.EqualTo("Good"));
            Assert.That(result[0].Rating, Is.EqualTo(8));
        }

        [Test]
        public async Task GetByUserIdAsync_ReturnsMappedDtos()
        {
            var reviews = new List<Review> { new Review { Id = 1, Content = "Good", Rating = 8, MovieId = 1, UserId = 1 } };
            _repoMock.Setup(r => r.GetByUserIdAsync(1)).ReturnsAsync(reviews);

            var result = (await _service.GetByUserIdAsync(1)).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].UserId, Is.EqualTo(1));
        }

        [Test]
        public async Task AddAsync_ValidReview_ReturnsReviewDto()
        {
            var dto = new ReviewCreateDto { Content = "Great movie!", Rating = 9, MovieId = 1 };
            var review = new Review { Id = 1, UserId = 1, Content = dto.Content, Rating = dto.Rating, MovieId = dto.MovieId };
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Review>())).ReturnsAsync(review);

            var result = await _service.AddAsync(dto, 1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Content, Is.EqualTo(dto.Content));
            Assert.That(result.Rating, Is.EqualTo(dto.Rating));
            Assert.That(result.MovieId, Is.EqualTo(1));
            Assert.That(result.UserId, Is.EqualTo(1));
        }

        [Test]
        public async Task UpdateAsync_OwnReview_CompletesSuccessfully()
        {
            var review = new Review { Id = 1, Content = "Old", Rating = 5, MovieId = 1, UserId = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);
            var dto = new ReviewUpdateDto { Content = "Updated", Rating = 8 };

            await _service.UpdateAsync(1, dto, 1);

            Assert.That(review.Content, Is.EqualTo("Updated"));
            Assert.That(review.Rating, Is.EqualTo(8));
        }

        [Test]
        public void UpdateAsync_ReviewNotFound_ThrowsUnauthorizedAccessException()
        {
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Review?)null);
            var dto = new ReviewUpdateDto { Content = "Updated", Rating = 8 };

            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.UpdateAsync(999, dto, 1));
        }

        [Test]
        public void UpdateAsync_NotOwner_ThrowsUnauthorizedAccessException()
        {
            var review = new Review { Id = 1, Content = "Old", Rating = 5, MovieId = 1, UserId = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);
            var dto = new ReviewUpdateDto { Content = "Updated", Rating = 8 };

            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.UpdateAsync(1, dto, 999));
        }

        [Test]
        public async Task DeleteAsync_OwnReview_CompletesSuccessfully()
        {
            var review = new Review { Id = 1, Content = "Old", Rating = 5, MovieId = 1, UserId = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);

            await _service.DeleteAsync(1, 1);

            _repoMock.Verify(r => r.DeleteAsync(review), Times.Once);
        }

        [Test]
        public void DeleteAsync_ReviewNotFound_ThrowsUnauthorizedAccessException()
        {
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Review?)null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.DeleteAsync(999, 1));
        }

        [Test]
        public void DeleteAsync_NotOwner_ThrowsUnauthorizedAccessException()
        {
            var review = new Review { Id = 1, Content = "Old", Rating = 5, MovieId = 1, UserId = 1 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);

            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _service.DeleteAsync(1, 999));
        }
    }
}
