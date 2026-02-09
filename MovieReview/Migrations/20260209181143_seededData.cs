using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MovieReview.Migrations
{
    /// <inheritdoc />
    public partial class seededData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "Description", "ReleaseYear", "Title" },
                values: new object[,]
                {
                    { 1L, "A mind-bending thriller about dream invasion.", 2010, "Inception" },
                    { 2L, "A hacker discovers the true nature of reality.", 1999, "The Matrix" },
                    { 3L, "A team travels through a wormhole to save humanity.", 2014, "Interstellar" }
                });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "Content", "MovieId", "Rating", "UserId" },
                values: new object[,]
                {
                    { 1L, "Amazing movie, loved every second!", 1L, 9, 1L },
                    { 2L, "Not bad, but could have been better.", 2L, 6, 2L },
                    { 3L, "Great cast and visuals, weak story.", 3L, 7, 3L },
                    { 4L, "Revolutionary sci-fi, a true classic.", 2L, 10, 1L },
                    { 5L, "Great action but the sequels ruined it a bit.", 2L, 8, 2L },
                    { 6L, "Never gets old, still one of my favorites.", 2L, 9, 3L },
                    { 7L, "Visually stunning and emotional.", 3L, 9, 1L },
                    { 8L, "The science parts were too heavy but still good.", 3L, 7, 2L },
                    { 9L, "Masterpiece, Nolan at his best.", 3L, 10, 3L }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 4L);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 5L);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 6L);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 7L);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 8L);

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: 9L);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.DeleteData(
                table: "Movies",
                keyColumn: "Id",
                keyValue: 3L);
        }
    }
}
