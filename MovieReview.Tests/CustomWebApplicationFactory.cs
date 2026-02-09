using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieReview.Data;
using MovieReview.Models.Entities;

namespace MovieReview.Tests;

/// <summary>
/// Test host that uses an in-memory database instead of MySQL.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<MovieReview.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove DbContextOptions<T> first: AddDbContext uses TryAdd for options,
            // so the original MySQL registration would otherwise remain and our in-memory config would be ignored.
            var optionsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<MovieReviewDbContext>));
            if (optionsDescriptor != null)
                services.Remove(optionsDescriptor);

            var contextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(MovieReviewDbContext));
            if (contextDescriptor != null)
                services.Remove(contextDescriptor);

            var provider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            services.AddDbContext<MovieReviewDbContext>(options =>
            {
                options.UseInMemoryDatabase("MovieReviewTestDb");
                options.UseInternalServiceProvider(provider);
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MovieReviewDbContext>();
            db.Database.EnsureCreated();
            if (!db.Movies.Any())
            {
                db.Movies.Add(new Movie { Id = 1, Title = "Test Movie", Description = "For integration test", ReleaseYear = 2020 });
                db.SaveChanges();
            }
        });
    }
}
