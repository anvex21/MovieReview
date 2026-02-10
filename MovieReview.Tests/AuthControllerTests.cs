using Moq;
using NUnit.Framework;
using MovieReview.Controllers;
using MovieReview.Exceptions;
using MovieReview.Models.DTOs;
using MovieReview.Services;
using Microsoft.AspNetCore.Mvc;

namespace MovieReview.Tests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _serviceMock = null!;
        private AuthController _controller = null!;

        [SetUp]
        public void Setup()
        {
            _serviceMock = new Mock<IAuthService>();
            _controller = new AuthController(_serviceMock.Object);
        }

        [Test]
        public async Task Register_ReturnsOk_WhenSuccess()
        {
            var dto = new RegisterDto { Username = "alice", Email = "alice@test.com", Password = "P@ssw0rd!" };
            var authResult = new AuthResultDto { Success = true, Token = "jwt-token", Message = "Registration successful" };
            _serviceMock.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(authResult);

            var result = await _controller.Register(dto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(authResult));
        }

        [Test]
        public void Register_ThrowsInvalidOperationException_WhenServiceThrows()
        {
            var dto = new RegisterDto { Username = "alice", Email = "alice@test.com", Password = "weak" };
            _serviceMock.Setup(s => s.RegisterAsync(dto)).ThrowsAsync(new InvalidOperationException("Username already exists"));

            Assert.ThrowsAsync<InvalidOperationException>(async () => await _controller.Register(dto));
        }

        [Test]
        public async Task Login_ReturnsOk_WhenSuccess()
        {
            var dto = new LoginDto { Username = "alice", Password = "P@ssw0rd!" };
            var authResult = new AuthResultDto { Success = true, Token = "jwt-token", Message = "Login successful" };
            _serviceMock.Setup(s => s.LoginAsync(dto)).ReturnsAsync(authResult);

            var result = await _controller.Login(dto) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(authResult));
        }

        [Test]
        public void Login_ThrowsUnauthorizedException_WhenServiceThrows()
        {
            var dto = new LoginDto { Username = "alice", Password = "wrong" };
            _serviceMock.Setup(s => s.LoginAsync(dto)).ThrowsAsync(new UnauthorizedException("Invalid username or password"));

            Assert.ThrowsAsync<UnauthorizedException>(async () => await _controller.Login(dto));
        }
    }
}
