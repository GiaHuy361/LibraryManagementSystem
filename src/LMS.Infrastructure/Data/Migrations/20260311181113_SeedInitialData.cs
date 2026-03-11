using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "authors",
                columns: new[] { "author_id", "author_name", "biography" },
                values: new object[,]
                {
                    { 1, "Robert C. Martin", "Uncle Bob, software engineer and author" },
                    { 2, "Martin Fowler", "Software developer and author" },
                    { 3, "Frank Herbert", "American science fiction writer" }
                });

            migrationBuilder.InsertData(
                table: "categories",
                columns: new[] { "category_id", "category_name", "description" },
                values: new object[,]
                {
                    { 1, "Information Technology", "Programming, Networking, and Systems" },
                    { 2, "Science Fiction", "Sci-Fi novels and literature" },
                    { 3, "Business", "Management, Finance, and Economics" }
                });

            migrationBuilder.InsertData(
                table: "books",
                columns: new[] { "book_id", "category_id", "created_at", "description", "isbn", "publish_year", "publisher", "quantity", "status", "title" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "9780132350884", 2008, "Prentice Hall", 5, "Available", "Clean Code: A Handbook of Agile Software Craftsmanship" },
                    { 2, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "9780201485677", 1999, "Addison-Wesley", 3, "Available", "Refactoring: Improving the Design of Existing Code" },
                    { 3, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "9780441172719", 1965, "Chilton Books", 10, "Available", "Dune" }
                });

            migrationBuilder.InsertData(
                table: "book_authors",
                columns: new[] { "author_id", "book_id" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "book_authors",
                keyColumns: new[] { "author_id", "book_id" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "book_authors",
                keyColumns: new[] { "author_id", "book_id" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "book_authors",
                keyColumns: new[] { "author_id", "book_id" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "category_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "authors",
                keyColumn: "author_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "authors",
                keyColumn: "author_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "authors",
                keyColumn: "author_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "book_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "book_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "books",
                keyColumn: "book_id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "category_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "categories",
                keyColumn: "category_id",
                keyValue: 2);
        }
    }
}
