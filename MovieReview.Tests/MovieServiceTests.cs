using Moq;
using NUnit.Framework;
using MovieReview.Services;
using MovieReview.Repositories;
using MovieReview.Models.Entities;
using MovieReview.Models.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace MovieReview.Tests
{
    [TestFixture]
    public class MovieServiceTests
    {
        private Mock<IMovieRepository> _repoMock = null!;
        private Mock<IExternalMovieService> _externalMock = null!;
        private MovieService _service = null!;

        [SetUp]
        public void Setup()
        {
            _repoMock = new Mock<IMovieRepository>();
            _externalMock = new Mock<IExternalMovieService>();
            _externalMock.Setup(s => s.GetImdbRatingAsync(It.IsAny<string>()))
                .ReturnsAsync("N/A");
            _service = new MovieService(_repoMock.Object, _externalMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            var movies = new List<Movie>
            {
                new Movie { Id = 1, Title = "A", Description = "D1", ReleaseYear = 2020, Reviews = new List<Review> { new Review { Rating = 8 }, new Review { Rating = 10 } } }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(movies);

            var result = (await _service.GetAllAsync()).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Id, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("A"));
            Assert.That(result[0].AverageRating, Is.EqualTo(9));
            Assert.That(result[0].ReviewCount, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllAsync_WithQuery_ReturnsMappedDtos()
        {
            var query = new MovieQueryDto { PageNumber = 1, PageSize = 10 };
            var movies = new List<Movie> { new Movie { Id = 1, Title = "A", Description = "D", ReleaseYear = 2020, Reviews = new List<Review>() } };
            _repoMock.Setup(r => r.GetAllAsync(query)).ReturnsAsync(movies);

            var result = (await _service.GetAllAsync(query)).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].AverageRating, Is.EqualTo(0));
            Assert.That(result[0].ReviewCount, Is.EqualTo(0));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsDto_WhenMovieExists()
        {
            var movie = new Movie { Id = 1, Title = "Inception", Description = "Desc", ReleaseYear = 2010, Reviews = new List<Review> { new Review { Rating = 9 } } };
            _repoMock.Setup(r => r.GetByIdWithReviewsAndRatingsAsync(1)).ReturnsAsync(movie);

            var result = await _service.GetByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(1));
            Assert.That(result.Title, Is.EqualTo("Inception"));
            Assert.That(result.AverageRating, Is.EqualTo(9));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenMovieDoesNotExist()
        {
            _repoMock.Setup(r => r.GetByIdWithReviewsAndRatingsAsync(999)).ReturnsAsync((Movie?)null);

            var result = await _service.GetByIdAsync(999);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsZeroAverageRating_WhenNoReviews()
        {
            var movie = new Movie { Id = 1, Title = "A", Description = "D", ReleaseYear = 2020, Reviews = new List<Review>() };
            _repoMock.Setup(r => r.GetByIdWithReviewsAndRatingsAsync(1)).ReturnsAsync(movie);

            var result = await _service.GetByIdAsync(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.AverageRating, Is.EqualTo(0));
            Assert.That(result.ReviewCount, Is.EqualTo(0));
        }

        [Test]
        public async Task CreateAsync_ReturnsMovieReadDto()
        {
            var dto = new MovieCreateDto { Title = "Matrix", Description = "Sci-fi", ReleaseYear = 1999 };
            Movie? captured = null;
            _repoMock.Setup(r => r.CreateAsync(It.IsAny<Movie>())).Callback<Movie>(m => { m.Id = 1; captured = m; }).Returns(Task.CompletedTask);

            var result = await _service.CreateAsync(dto);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Title, Is.EqualTo(dto.Title));
            Assert.That(result.Description, Is.EqualTo(dto.Description));
            Assert.That(result.ReleaseYear, Is.EqualTo(dto.ReleaseYear));
            Assert.That(result.ReviewCount, Is.EqualTo(0));
            Assert.That(result.AverageRating, Is.EqualTo(0));
        }

        [Test]
        public async Task UpdateAsync_ExistingMovie_ReturnsTrue()
        {
            var movie = new Movie { Id = 1, Title = "Old", Description = "Desc", ReleaseYear = 2000 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(movie);

            var dto = new MovieUpdateDto { Title = "New", Description = "NewDesc", ReleaseYear = 2001 };
            var result = await _service.UpdateAsync(1, dto);

            Assert.That(result, Is.True);
            Assert.That(movie.Title, Is.EqualTo("New"));
        }

        [Test]
        public async Task UpdateAsync_NonExistingMovie_ReturnsFalse()
        {
            _repoMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Movie?)null);

            var dto = new MovieUpdateDto { Title = "New", Description = "NewDesc", ReleaseYear = 2001 };
            var result = await _service.UpdateAsync(2, dto);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteAsync_ExistingMovie_ReturnsTrue()
        {
            var movie = new Movie { Id = 1, Title = "A", Description = "D", ReleaseYear = 2020 };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(movie);

            var result = await _service.DeleteAsync(1);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DeleteAsync_NonExistingMovie_ReturnsFalse()
        {
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Movie?)null);

            var result = await _service.DeleteAsync(999);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetTopRatedAsync_ReturnsMappedDtos()
        {
            var movies = new List<Movie> { new Movie { Id = 1, Title = "A", Description = "D", ReleaseYear = 2020, Reviews = new List<Review> { new Review { Rating = 10 } } } };
            _repoMock.Setup(r => r.GetTopRatedAsync(5)).ReturnsAsync(movies);

            var result = (await _service.GetTopRatedAsync(5)).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].AverageRating, Is.EqualTo(10));
        }

        [Test]
        public async Task GetByYearAsync_ReturnsMappedDtos()
        {
            var movies = new List<Movie> { new Movie { Id = 1, Title = "A", Description = "D", ReleaseYear = 2010, Reviews = new List<Review>() } };
            _repoMock.Setup(r => r.GetByYearAsync(2010)).ReturnsAsync(movies);

            var result = (await _service.GetByYearAsync(2010)).ToList();

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].ReleaseYear, Is.EqualTo(2010));
        }
    }
}
