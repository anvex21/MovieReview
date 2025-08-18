using Moq;
using NUnit.Framework;
using MovieReview.Controllers;
using MovieReview.Services;
using MovieReview.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieReview.Tests
{
    [TestFixture]
    public class MoviesControllerTests
    {
        private Mock<IMovieService> _serviceMock;
        private MoviesController _controller;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IMovieService>();
            _controller = new MoviesController(_serviceMock.Object);
        }

        [Test]
        public async Task GetAll_ReturnsOk_WhenMoviesExist()
        {
            _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<MovieReadDto> { new MovieReadDto { Id = 1, Title = "Inception" } });

            var result = await _controller.GetAll();

            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task GetById_ReturnsNotFound_WhenMovieDoesNotExist()
        {
            _serviceMock.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((MovieReadDto)null);

            var result = await _controller.GetById(1);

            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task Create_ReturnsCreatedAtAction()
        {
            var dto = new MovieCreateDto { Title = "Matrix", Description = "Sci-fi", ReleaseYear = 1999 };
            var created = new MovieReadDto { Id = 1, Title = dto.Title, Description = dto.Description, ReleaseYear = dto.ReleaseYear };

            _serviceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

            var result = await _controller.Create(dto) as CreatedAtActionResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(created, result.Value);
        }
    }
}
