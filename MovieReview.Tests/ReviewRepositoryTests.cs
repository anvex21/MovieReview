using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using MovieReview.Data;
using MovieReview.Models.Entities;
using MovieReview.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieReview.Tests
{
    [TestFixture]
    public class ReviewRepositoryTests
    {
        private MovieReviewDbContext _context = null!;
        private ReviewRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MovieReviewDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            _context = new MovieReviewDbContext(options);
            _repository = new ReviewRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
        
        [Test]
        public async Task AddAsync_AddsReview()
        {
            var review = new Review { Content = "New", Rating = 8, MovieId = 1, UserId = 1 };
            var result = await _repository.AddAsync(review);

            Assert.That(result.Id, Is.GreaterThan(0));
            var inDb = await _context.Reviews.FindAsync(result.Id);
            Assert.That(inDb, Is.Not.Null);
        }

        [Test]
        public async Task UpdateAsync_UpdatesReview()
        {
            var review = new Review { Content = "Old", Rating = 5, MovieId = 1, UserId = 1 };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            review.Content = "Updated";
            await _repository.UpdateAsync(review);

            var inDb = await _context.Reviews.FindAsync(review.Id);
            Assert.That(inDb!.Content, Is.EqualTo("Updated"));
        }

        [Test]
        public async Task DeleteAsync_RemovesReview()
        {
            var review = new Review { Content = "DeleteMe", Rating = 1, MovieId = 1, UserId = 1 };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(review);

            var inDb = await _context.Reviews.FindAsync(review.Id);
            Assert.That(inDb, Is.Null);
        }
    }
}
