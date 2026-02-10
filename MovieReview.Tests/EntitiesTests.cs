using NUnit.Framework;
using MovieReview.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieReview.Tests
{
    [TestFixture]
    public class EntitiesTests
    {
        [Test]
        public void Movie_Initialization_SetsProperties()
        {
            var movie = new Movie
            {
                Id = 1,
                Title = "Inception",
                Description = "Dream within a dream",
                ReleaseYear = 2010
            };

            Assert.That(movie.Id, Is.EqualTo(1));
            Assert.That(movie.Title, Is.EqualTo("Inception"));
            Assert.That(movie.Description, Is.EqualTo("Dream within a dream"));
            Assert.That(movie.ReleaseYear, Is.EqualTo(2010));
            Assert.That(movie.Reviews, Is.Not.Null);
            Assert.That(movie.Reviews, Is.Empty);
        }

        [Test]
        public void Review_Initialization_SetsProperties()
        {
            var review = new Review
            {
                Id = 1,
                Content = "Great movie",
                Rating = 9,
                MovieId = 1,
                UserId = 1
            };

            Assert.That(review.Id, Is.EqualTo(1));
            Assert.That(review.Content, Is.EqualTo("Great movie"));
            Assert.That(review.Rating, Is.EqualTo(9));
            Assert.That(review.MovieId, Is.EqualTo(1));
            Assert.That(review.UserId, Is.EqualTo(1));
        }

        [Test]
        public void User_Initialization_SetsProperties()
        {
            var user = new User
            {
                Id = 1,
                UserName = "alice",
                Email = "alice@test.com"
            };

            Assert.That(user.Id, Is.EqualTo(1));
            Assert.That(user.UserName, Is.EqualTo("alice"));
            Assert.That(user.Email, Is.EqualTo("alice@test.com"));
            Assert.That(user.Reviews, Is.Not.Null);
            Assert.That(user.Reviews, Is.Empty);
        }

        [Test]
        public void Review_Validation_Fails_WhenRatingOutOfRange()
        {
            var review = new Review
            {
                Content = "Bad rating",
                Rating = 11, // Invalid
                MovieId = 1,
                UserId = 1
            };

            var context = new ValidationContext(review);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(review, context, results, true);

            Assert.That(isValid, Is.False);
            Assert.That(results, Has.Some.Property("MemberNames").Contain("Rating"));
        }
    }
}
