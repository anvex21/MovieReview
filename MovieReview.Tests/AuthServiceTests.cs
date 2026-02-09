using Moq;
using NUnit.Framework;
using MovieReview.Services;
using MovieReview.Models.DTOs;
using MovieReview.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieReview.Tests
{
    [TestFixture]
    public class AuthServiceTests
    {
        private Mock<UserManager<User>> _userManagerMock = null!;
        private Mock<IConfiguration> _configMock = null!;
        private AuthService _service = null!;

        [SetUp]
        public void Setup()
        {
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
            _configMock = new Mock<IConfiguration>();
            var jwtSection = new Mock<IConfigurationSection>();
            jwtSection.Setup(s => s["Key"]).Returns("this-is-a-very-long-secret-key-for-jwt-signing");
            jwtSection.Setup(s => s["Issuer"]).Returns("TestIssuer");
            jwtSection.Setup(s => s["Audience"]).Returns("TestAudience");
            _configMock.Setup(c => c.GetSection("Jwt")).Returns(jwtSection.Object);
            _service = new AuthService(_userManagerMock.Object, _configMock.Object);
        }

        [Test]
        public async Task RegisterAsync_ReturnsSuccess_WhenUserDoesNotExist()
        {
            var dto = new RegisterDto { Username = "alice", Email = "alice@test.com", Password = "P@ssw0rd!" };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync((User?)null);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), dto.Password)).ReturnsAsync(IdentityResult.Success);

            var result = await _service.RegisterAsync(dto);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Token, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Message, Is.EqualTo("Registration successful"));
        }

        [Test]
        public async Task RegisterAsync_ReturnsFailure_WhenUsernameExists()
        {
            var dto = new RegisterDto { Username = "alice", Email = "alice@test.com", Password = "P@ssw0rd!" };
            var existingUser = new User { Id = 1, UserName = dto.Username };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync(existingUser);

            var result = await _service.RegisterAsync(dto);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Username already exists"));
        }

        [Test]
        public async Task RegisterAsync_ReturnsFailure_WhenCreateFails()
        {
            var dto = new RegisterDto { Username = "alice", Email = "alice@test.com", Password = "weak" };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync((User?)null);
            var errors = new[] { new IdentityError { Description = "Password too short" } };
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), dto.Password)).ReturnsAsync(IdentityResult.Failed(errors));

            var result = await _service.RegisterAsync(dto);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Does.Contain("Password too short"));
        }

        [Test]
        public async Task LoginAsync_ReturnsSuccess_WhenCredentialsValid()
        {
            var dto = new LoginDto { Username = "alice", Password = "P@ssw0rd!" };
            var user = new User { Id = 1, UserName = dto.Username };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);

            var result = await _service.LoginAsync(dto);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Token, Is.Not.Null.And.Not.Empty);
            Assert.That(result.Message, Is.EqualTo("Login successful"));
        }

        [Test]
        public async Task LoginAsync_ReturnsFailure_WhenUserNotFound()
        {
            var dto = new LoginDto { Username = "unknown", Password = "P@ssw0rd!" };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync((User?)null);

            var result = await _service.LoginAsync(dto);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Invalid username or password"));
        }

        [Test]
        public async Task LoginAsync_ReturnsFailure_WhenPasswordInvalid()
        {
            var dto = new LoginDto { Username = "alice", Password = "wrong" };
            var user = new User { Id = 1, UserName = dto.Username };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username)).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(false);

            var result = await _service.LoginAsync(dto);

            Assert.That(result.Success, Is.False);
            Assert.That(result.Message, Is.EqualTo("Invalid username or password"));
        }
    }
}
