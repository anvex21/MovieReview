using Moq;
using NUnit.Framework;
using MovieReview.Services;
using MovieReview.Repositories;
using MovieReview.Models.Entities;
using MovieReview.Models.DTOs;

namespace MovieReview.Tests
{
    [TestFixture]
    public class MovieServiceTests
    {
        private Mock<IMovieRepository> _repoMock;
        private MovieService _service;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IMovieRepository>();
            _service = new MovieService(_repoMock.Object);
        }

        [Test]
        public async Task UpdateAsync_ExistingMovie_ReturnsTrue()
        {
            Movie movie = new Movie { Id = 1, Title = "Old", Description = "Desc", ReleaseYear = 2000 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(movie);

            MovieUpdateDto dto = new MovieUpdateDto { Title = "New", Description = "NewDesc", ReleaseYear = 2001 };
            bool result = await _service.UpdateAsync(1, dto);

            Assert.IsTrue(result);
            Assert.AreEqual("New", movie.Title);
        }

        [Test]
        public async Task UpdateAsync_NonExistingMovie_ReturnsFalse()
        {
            _repoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Movie)null);

            MovieUpdateDto dto = new MovieUpdateDto { Title = "New", Description = "NewDesc", ReleaseYear = 2001 };
            bool result = await _service.UpdateAsync(2, dto);

            Assert.IsFalse(result);
        }
    }
}
