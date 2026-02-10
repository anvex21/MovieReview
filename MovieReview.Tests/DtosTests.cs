using NUnit.Framework;
using MovieReview.Models.DTOs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieReview.Tests
{
    [TestFixture]
    public class DtosTests
    {
        private bool ValidateModel(object model, out List<ValidationResult> results)
        {
            var context = new ValidationContext(model);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(model, context, results, true);
        }

        [Test]
        public void RegisterDto_Valid_ReturnsTrue()
        {
            var dto = new RegisterDto { Username = "alice", Email = "alice@test.com", Password = "password" };
            Assert.That(ValidateModel(dto, out _), Is.True);
        }

        [Test]
        public void RegisterDto_InvalidEmail_ReturnsFalse()
        {
            var dto = new RegisterDto { Username = "alice", Email = "invalid-email", Password = "password" };
            Assert.That(ValidateModel(dto, out var results), Is.False);
            Assert.That(results, Has.Some.Property("MemberNames").Contain("Email"));
        }

        [Test]
        public void RegisterDto_ShortPassword_ReturnsFalse()
        {
            var dto = new RegisterDto { Username = "alice", Email = "alice@test.com", Password = "123" };
            Assert.That(ValidateModel(dto, out var results), Is.False);
            Assert.That(results, Has.Some.Property("MemberNames").Contain("Password"));
        }

        [Test]
        public void LoginDto_Valid_ReturnsTrue()
        {
            var dto = new LoginDto { Username = "alice", Password = "password" };
            Assert.That(ValidateModel(dto, out _), Is.True);
        }

        [Test]
        public void LoginDto_MissingFields_ReturnsFalse()
        {
            var dto = new LoginDto { Username = null!, Password = null! }; // Intentionally null to test Required
            Assert.That(ValidateModel(dto, out var results), Is.False);
        }

        [Test]
        public void MovieCreateDto_Valid_ReturnsTrue()
        {
            var dto = new MovieCreateDto { Title = "Inception", Description = "Dream", ReleaseYear = 2010 };
            Assert.That(ValidateModel(dto, out _), Is.True);
        }

        [Test]
        public void MovieCreateDto_InvalidYear_ReturnsFalse()
        {
            var dto = new MovieCreateDto { Title = "Inception", Description = "Dream", ReleaseYear = 1700 };
            Assert.That(ValidateModel(dto, out var results), Is.False);
            Assert.That(results, Has.Some.Property("MemberNames").Contain("ReleaseYear"));
        }

        [Test]
        public void MovieUpdateDto_Valid_ReturnsTrue()
        {
            var dto = new MovieUpdateDto { Title = "Inception", Description = "Dream", ReleaseYear = 2010 };
            Assert.That(ValidateModel(dto, out _), Is.True);
        }

        [Test]
        public void MovieUpdateDto_InvalidYear_ReturnsFalse()
        {
            var dto = new MovieUpdateDto { Title = "Inception", Description = "Dream", ReleaseYear = 1700 };
            Assert.That(ValidateModel(dto, out var results), Is.False);
            Assert.That(results, Has.Some.Property("MemberNames").Contain("ReleaseYear"));
        }

        [Test]
        public void ReviewCreateDto_Valid_ReturnsTrue()
        {
            var dto = new ReviewCreateDto { Content = "Great", Rating = 9, MovieId = 1 };
            Assert.That(ValidateModel(dto, out _), Is.True);
        }

        [Test]
        public void ReviewCreateDto_InvalidRating_ReturnsFalse()
        {
            var dto = new ReviewCreateDto { Content = "Great", Rating = 11, MovieId = 1 };
            Assert.That(ValidateModel(dto, out var results), Is.False);
            Assert.That(results, Has.Some.Property("MemberNames").Contain("Rating"));
        }

        [Test]
        public void ReviewUpdateDto_Valid_ReturnsTrue()
        {
            var dto = new ReviewUpdateDto { Content = "Great", Rating = 9 };
            Assert.That(ValidateModel(dto, out _), Is.True);
        }

        [Test]
        public void ReviewUpdateDto_InvalidRating_ReturnsFalse()
        {
            var dto = new ReviewUpdateDto { Content = "Great", Rating = 11 };
            Assert.That(ValidateModel(dto, out var results), Is.False);
            Assert.That(results, Has.Some.Property("MemberNames").Contain("Rating"));
        }

        [Test]
        public void MovieQueryDto_Defaults_AreCorrect()
        {
            var dto = new MovieQueryDto();
            Assert.That(dto.PageNumber, Is.EqualTo(1));
            Assert.That(dto.PageSize, Is.EqualTo(10));
            Assert.That(dto.IsDescending, Is.False);
            Assert.That(dto.Name, Is.Null);
            Assert.That(dto.SortBy, Is.Null);
        }
    }
}
