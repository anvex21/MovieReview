using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using MovieReview.Data;
using MovieReview.Models.DTOs;
using MovieReview.Models.Entities;
using MovieReview.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieReview.Tests
{
    [TestFixture]
    public class MovieRepositoryTests
    {
        private MovieReviewDbContext _context = null!;
        private MovieRepository _repository = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<MovieReviewDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB per test
                .Options;

            _context = new MovieReviewDbContext(options);
            _repository = new MovieRepository(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task GetAllAsync_ReturnsAllMovies()
        {
            _context.Movies.Add(new Movie { Title = "A", Description = "D", ReleaseYear = 2020 });
            _context.Movies.Add(new Movie { Title = "B", Description = "D", ReleaseYear = 2021 });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllAsync_WithQuery_FiltersByName()
        {
            _context.Movies.Add(new Movie { Title = "Matrix", Description = "D", ReleaseYear = 1999 });
            _context.Movies.Add(new Movie { Title = "Inception", Description = "D", ReleaseYear = 2010 });
            await _context.SaveChangesAsync();

            var query = new MovieQueryDto { Name = "Matrix", PageNumber = 1, PageSize = 10 };
            var result = await _repository.GetAllAsync(query);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("Matrix"));
        }

        [Test]
        public async Task GetAllAsync_WithQuery_SortsByReleaseYearDescending()
        {
            _context.Movies.Add(new Movie { Title = "A", Description = "D", ReleaseYear = 2000 });
            _context.Movies.Add(new Movie { Title = "B", Description = "D", ReleaseYear = 2010 });
            await _context.SaveChangesAsync();

            var query = new MovieQueryDto { SortBy = "ReleaseYear", IsDescending = true, PageNumber = 1, PageSize = 10 };
            var result = (await _repository.GetAllAsync(query)).ToList();

            Assert.That(result[0].ReleaseYear, Is.EqualTo(2010));
            Assert.That(result[1].ReleaseYear, Is.EqualTo(2000));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsMovie_WhenExists()
        {
            var movie = new Movie { Title = "A", Description = "D", ReleaseYear = 2020 };
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(movie.Id);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Title, Is.EqualTo("A"));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
        {
            var result = await _repository.GetByIdAsync(999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateAsync_AddsMovie()
        {
            var movie = new Movie { Title = "New", Description = "D", ReleaseYear = 2022 };
            await _repository.CreateAsync(movie);

            var inDb = await _context.Movies.FindAsync(movie.Id);
            Assert.That(inDb, Is.Not.Null);
            Assert.That(inDb!.Title, Is.EqualTo("New"));
        }

        [Test]
        public async Task UpdateAsync_UpdatesMovie()
        {
            var movie = new Movie { Title = "Old", Description = "D", ReleaseYear = 2020 };
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            movie.Title = "Updated";
            await _repository.UpdateAsync(movie);

            var inDb = await _context.Movies.FindAsync(movie.Id);
            Assert.That(inDb!.Title, Is.EqualTo("Updated"));
        }

        [Test]
        public async Task DeleteAsync_RemovesMovie()
        {
            var movie = new Movie { Title = "DeleteMe", Description = "D", ReleaseYear = 2020 };
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();

            await _repository.DeleteAsync(movie);

            var inDb = await _context.Movies.FindAsync(movie.Id);
            Assert.That(inDb, Is.Null);
        }

        [Test]
        public async Task GetTopRatedAsync_ReturnsHighestRatedMovies()
        {
            var m1 = new Movie { Title = "Low", Description = "D", ReleaseYear = 2020, Reviews = new List<Review> { new Review { Rating = 2, Content = "Bad" } } };
            var m2 = new Movie { Title = "High", Description = "D", ReleaseYear = 2020, Reviews = new List<Review> { new Review { Rating = 10, Content = "Good" } } };
            _context.Movies.AddRange(m1, m2);
            await _context.SaveChangesAsync();

            var result = (await _repository.GetTopRatedAsync(1)).ToList();

            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("High"));
        }

        [Test]
        public async Task GetByYearAsync_ReturnsMoviesForYear()
        {
            _context.Movies.Add(new Movie { Title = "2010", Description = "D", ReleaseYear = 2010 });
            _context.Movies.Add(new Movie { Title = "2020", Description = "D", ReleaseYear = 2020 });
            await _context.SaveChangesAsync();

            var result = await _repository.GetByYearAsync(2010);

            Assert.That(result.Count(), Is.EqualTo(1));
            Assert.That(result.First().Title, Is.EqualTo("2010"));
        }
    }
}
