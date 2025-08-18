using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieReview.Models.Entities;

namespace MovieReview.Data
{
    public class MovieReviewDbContext: IdentityDbContext<User, IdentityRole<long>, long>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public MovieReviewDbContext(DbContextOptions<MovieReviewDbContext> options) : base(options)
        {
            
        }

        /// <summary>
        /// Movies
        /// </summary>
        public DbSet<Movie> Movies { get; set; }

        /// <summary>
        /// Reviews
        /// </summary>
        public DbSet<Review> Reviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationships
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MovieId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            // Seed movies
            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    Id = 1,
                    Title = "Inception",
                    Description = "A mind-bending thriller about dream invasion.",
                    ReleaseYear = 2010
                },
                new Movie
                {
                    Id = 2,
                    Title = "The Matrix",
                    Description = "A hacker discovers the true nature of reality.",
                    ReleaseYear = 1999
                },
                new Movie
                {
                    Id = 3,
                    Title = "Interstellar",
                    Description = "A team travels through a wormhole to save humanity.",
                    ReleaseYear = 2014
                }
            );

            // Seed data for reviews
            modelBuilder.Entity<Review>().HasData(
                new Review
                {
                    Id = 1,
                    Content = "Amazing movie, loved every second!",
                    Rating = 9,
                    MovieId = 1,
                    UserId = 1
                },
                new Review
                {
                    Id = 2,
                    Content = "Not bad, but could have been better.",
                    Rating = 6,
                    MovieId = 2,
                    UserId = 2
                },
                new Review
                {
                    Id = 3,
                    Content = "Great cast and visuals, weak story.",
                    Rating = 7,
                    MovieId = 3,
                    UserId = 3
                },
                new Review
                {
                    Id = 4,
                    Content = "Revolutionary sci-fi, a true classic.",
                    Rating = 10,
                    MovieId = 2, 
                    UserId = 1
                },
                new Review
                {
                    Id = 5,
                    Content = "Great action but the sequels ruined it a bit.",
                    Rating = 8,
                    MovieId = 2,
                    UserId = 2
                },
                new Review
                {
                    Id = 6,
                    Content = "Never gets old, still one of my favorites.",
                    Rating = 9,
                    MovieId = 2,
                    UserId = 3
                },
                new Review
                {
                    Id = 7,
                    Content = "Visually stunning and emotional.",
                    Rating = 9,
                    MovieId = 3, 
                    UserId = 1
                },
                new Review
                {
                    Id = 8,
                    Content = "The science parts were too heavy but still good.",
                    Rating = 7,
                    MovieId = 3,
                    UserId = 2
                },
                new Review
                {
                    Id = 9,
                    Content = "Masterpiece, Nolan at his best.",
                    Rating = 10,
                    MovieId = 3,
                    UserId = 3
                }
            );
        }
    }
}
