using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MovieReview.Migrations
{
    /// <inheritdoc />
    public partial class Seedmoviedata : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
