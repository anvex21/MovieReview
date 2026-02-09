using Moq;
using NUnit.Framework;
using MovieReview.Controllers;
using MovieReview.Services;
using MovieReview.Models.DTOs;
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
        public async Task Register_ReturnsBadRequest_WhenNotSuccess()
        {
            var dto = new RegisterDto { Username = "alice", Email = "alice@test.com", Password = "weak" };
            var authResult = new AuthResultDto { Success = false, Message = "Username already exists" };
            _serviceMock.Setup(s => s.RegisterAsync(dto)).ReturnsAsync(authResult);

            var result = await _controller.Register(dto) as BadRequestObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(authResult.Message));
        }

        [Test]
        public async Task Register_ReturnsBadRequest_WhenModelStateInvalid()
        {
            var dto = new RegisterDto { Username = "alice", Email = "alice@test.com", Password = "weak" };
            _controller.ModelState.AddModelError("Password", "Too weak");

            var result = await _controller.Register(dto);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
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
        public async Task Login_ReturnsUnauthorized_WhenNotSuccess()
        {
            var dto = new LoginDto { Username = "alice", Password = "wrong" };
            var authResult = new AuthResultDto { Success = false, Message = "Invalid username or password" };
            _serviceMock.Setup(s => s.LoginAsync(dto)).ReturnsAsync(authResult);

            var result = await _controller.Login(dto) as UnauthorizedObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Value, Is.EqualTo(authResult.Message));
        }

        [Test]
        public async Task Login_ReturnsBadRequest_WhenModelStateInvalid()
        {
            var dto = new LoginDto { Username = "alice", Password = "" };
            _controller.ModelState.AddModelError("Password", "Required");

            var result = await _controller.Login(dto);

            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }
    }
}
