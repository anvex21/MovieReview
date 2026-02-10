using Moq;
using MovieReview.Controllers;
using MovieReview.Exceptions;
using MovieReview.Models.DTOs;
using MovieReview.Services;
using Microsoft.AspNetCore.Mvc;

namespace MovieReview.Tests
{
    [TestFixture]
    public class MoviesControllerTests
    {
        private Mock<IMovieService> _serviceMock = null!;
        private MoviesController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IMovieService>();
            _controller = new MoviesController(_serviceMock.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOk_WhenMoviesExist()
        {
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<MovieReadDto> { new MovieReadDto { Id = 1, Title = "Inception", Description = "" } });

            var result = await _controller.GetAll();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetAll_ReturnsOk_WhenNoMoviesExist()
        {
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<MovieReadDto>());

            var result = await _controller.GetAll();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(((OkObjectResult)result).Value, Is.Not.Null);
            Assert.That((IEnumerable<MovieReadDto>)((OkObjectResult)result).Value!, Is.Empty);
        }

        [Test]
        public async Task GetAllMovies_WithQuery_ReturnsOk()
        {
            var query = new MovieQueryDto { PageNumber = 1, PageSize = 10 };
            _serviceMock.Setup(s => s.GetAllAsync(query)).ReturnsAsync(new List<MovieReadDto> { new MovieReadDto { Id = 1, Title = "Inception", Description = "" } });

            var result = await _controller.GetAllMovies(query);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetById_ReturnsOk_WhenMovieExists()
        {
            var movie = new MovieReadDto { Id = 1, Title = "Inception", Description = "Desc", ReleaseYear = 2010 };
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(movie);

            var result = await _controller.GetById(1) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(movie));
        }

        [Test]
        public void GetById_ThrowsNotFoundException_WhenMovieDoesNotExist()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ThrowsAsync(new NotFoundException("No such movie found."));

            Assert.ThrowsAsync<NotFoundException>(async () => await _controller.GetById(1));
        }

        [Test]
        public async Task GetTopRatedMovies_ReturnsOk()
        {
            _serviceMock.Setup(s => s.GetTopRatedAsync(5)).ReturnsAsync(new List<MovieReadDto> { new MovieReadDto { Id = 1, Title = "Inception", Description = "", AverageRating = 9 } });

            var result = await _controller.GetTopRatedMovies(5);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetMoviesByYear_ReturnsOk_WhenMoviesExist()
        {
            _serviceMock.Setup(s => s.GetByYearAsync(2010)).ReturnsAsync(new List<MovieReadDto> { new MovieReadDto { Id = 1, Title = "Inception", Description = "", ReleaseYear = 2010 } });

            var result = await _controller.GetMoviesByYear(2010);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetMoviesByYear_ReturnsOk_WhenNoMoviesForYear()
        {
            _serviceMock.Setup(s => s.GetByYearAsync(1900)).ReturnsAsync(new List<MovieReadDto>());

            var result = await _controller.GetMoviesByYear(1900);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task Create_ReturnsCreatedAtAction_WhenValid()
        {
            var dto = new MovieCreateDto { Title = "Matrix", Description = "Sci-fi", ReleaseYear = 1999 };
            var created = new MovieReadDto { Id = 1, Title = dto.Title, Description = dto.Description, ReleaseYear = dto.ReleaseYear };

            _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

            var result = await _controller.Create(dto) as CreatedAtActionResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(created));
            Assert.That(result.ActionName, Is.EqualTo(nameof(MoviesController.GetById)));
        }

        [Test]
        public async Task Update_ReturnsNoContent_WhenSuccess()
        {
            var dto = new MovieUpdateDto { Title = "New", Description = "NewDesc", ReleaseYear = 2001 };
            _serviceMock.Setup(s => s.UpdateAsync(1, dto)).Returns(Task.CompletedTask);

            var result = await _controller.Update(1, dto);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public void Update_ThrowsNotFoundException_WhenMovieDoesNotExist()
        {
            var dto = new MovieUpdateDto { Title = "New", Description = "NewDesc", ReleaseYear = 2001 };
            _serviceMock.Setup(s => s.UpdateAsync(999, dto)).ThrowsAsync(new NotFoundException("No such movie found."));

            Assert.ThrowsAsync<NotFoundException>(async () => await _controller.Update(999, dto));
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenSuccess()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1)).Returns(Task.CompletedTask);

            var result = await _controller.Delete(1);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public void Delete_ThrowsNotFoundException_WhenMovieDoesNotExist()
        {
            _serviceMock.Setup(s => s.DeleteAsync(999)).ThrowsAsync(new NotFoundException("No such movie found."));

            Assert.ThrowsAsync<NotFoundException>(async () => await _controller.Delete(999));
        }
    }
}
