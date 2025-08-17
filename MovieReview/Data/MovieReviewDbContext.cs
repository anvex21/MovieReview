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

        /// <summary>
        /// Ratings
        /// </summary>
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            // manage relationships

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MovieId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Ratings)
                .HasForeignKey(r => r.MovieId);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
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
        }

    }
}
