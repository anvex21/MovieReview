using Moq;
using MovieReview.Controllers;
using MovieReview.Services;
using MovieReview.Models.DTOs;
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
        public async Task GetAll_ReturnsNotFound_WhenNoMoviesExist()
        {
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<MovieReadDto>());

            var result = await _controller.GetAll();

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
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
        public async Task GetById_ReturnsNotFound_WhenMovieDoesNotExist()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((MovieReadDto?)null);

            var result = await _controller.GetById(1);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
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
        public async Task GetMoviesByYear_ReturnsNotFound_WhenNoMoviesForYear()
        {
            _serviceMock.Setup(s => s.GetByYearAsync(1900)).ReturnsAsync(new List<MovieReadDto>());

            var result = await _controller.GetMoviesByYear(1900);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
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
        public async Task Create_ReturnsBadRequest_WhenModelStateInvalid()
        {
            var dto = new MovieCreateDto { Title = "Matrix", Description = "Sci-fi", ReleaseYear = 1999 };
            _controller.ModelState.AddModelError("Title", "Required");

            var result = await _controller.Create(dto);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Update_ReturnsNoContent_WhenSuccess()
        {
            var dto = new MovieUpdateDto { Title = "New", Description = "NewDesc", ReleaseYear = 2001 };
            _serviceMock.Setup(s => s.UpdateAsync(1, dto)).ReturnsAsync(true);

            var result = await _controller.Update(1, dto);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task Update_ReturnsNotFound_WhenMovieDoesNotExist()
        {
            var dto = new MovieUpdateDto { Title = "New", Description = "NewDesc", ReleaseYear = 2001 };
            _serviceMock.Setup(s => s.UpdateAsync(999, dto)).ReturnsAsync(false);

            var result = await _controller.Update(999, dto);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task Update_ReturnsBadRequest_WhenModelStateInvalid()
        {
            var dto = new MovieUpdateDto { Title = "New", Description = "NewDesc", ReleaseYear = 2001 };
            _controller.ModelState.AddModelError("Title", "Required");

            var result = await _controller.Update(1, dto);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Delete_ReturnsNoContent_WhenSuccess()
        {
            _serviceMock.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task Delete_ReturnsNotFound_WhenMovieDoesNotExist()
        {
            _serviceMock.Setup(s => s.DeleteAsync(999)).ReturnsAsync(false);

            var result = await _controller.Delete(999);

            Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
        }
    }
}
