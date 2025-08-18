using Moq;
using NUnit.Framework;
using MovieReview.Services;
using MovieReview.Repositories;
using MovieReview.Models.Entities;
using MovieReview.Models.DTOs;

namespace MovieReview.Tests
{
    [TestFixture]
    public class ReviewServiceTests
    {
        private Mock<IReviewRepository> _repoMock;
        private ReviewService _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IReviewRepository>();
            _service = new ReviewService(_repoMock.Object);
        }

        [Test]
        public async Task AddAsync_ValidReview_ReturnsReviewDto()
        {
            ReviewCreateDto dto = new ReviewCreateDto
            {
                Content = "Great movie!",
                Rating = 9,
                MovieId = 1
            };

            var review = new Review { Id = 1, UserId = 1, Content = "Test", Rating = 5, MovieId = 1 };
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Review>())).ReturnsAsync(review);

            var result = await _service.AddAsync(dto, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(dto.Content, result.Content);
            Assert.AreEqual(dto.Rating, result.Rating);
        }
    }
}
