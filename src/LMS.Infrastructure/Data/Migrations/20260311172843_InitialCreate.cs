using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LMS.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "authors",
                columns: table => new
                {
                    author_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    author_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    biography = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_authors", x => x.author_id);
                });

            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    category_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    category_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.category_id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    role_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    role_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.role_id);
                });

            migrationBuilder.CreateTable(
                name: "books",
                columns: table => new
                {
                    book_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    isbn = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    category_id = table.Column<int>(type: "int", nullable: true),
                    publisher = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    publish_year = table.Column<int>(type: "int", nullable: true),
                    description = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Available"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_books", x => x.book_id);
                    table.ForeignKey(
                        name: "FK_books_categories_category_id",
                        column: x => x.category_id,
                        principalTable: "categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    role_id = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Active"),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "book_authors",
                columns: table => new
                {
                    book_id = table.Column<int>(type: "int", nullable: false),
                    author_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_book_authors", x => new { x.book_id, x.author_id });
                    table.ForeignKey(
                        name: "FK_book_authors_authors_author_id",
                        column: x => x.author_id,
                        principalTable: "authors",
                        principalColumn: "author_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_book_authors_books_book_id",
                        column: x => x.book_id,
                        principalTable: "books",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "borrow_records",
                columns: table => new
                {
                    borrow_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    member_id = table.Column<int>(type: "int", nullable: false),
                    librarian_id = table.Column<int>(type: "int", nullable: true),
                    borrow_date = table.Column<DateOnly>(type: "date", nullable: false),
                    due_date = table.Column<DateOnly>(type: "date", nullable: false),
                    return_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Borrowing"),
                    total_fine = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_borrow_records", x => x.borrow_id);
                    table.ForeignKey(
                        name: "FK_borrow_records_users_librarian_id",
                        column: x => x.librarian_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_borrow_records_users_member_id",
                        column: x => x.member_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "borrow_details",
                columns: table => new
                {
                    borrow_detail_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    borrow_id = table.Column<int>(type: "int", nullable: true),
                    book_id = table.Column<int>(type: "int", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    return_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Borrowed")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_borrow_details", x => x.borrow_detail_id);
                    table.ForeignKey(
                        name: "FK_borrow_details_books_book_id",
                        column: x => x.book_id,
                        principalTable: "books",
                        principalColumn: "book_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_borrow_details_borrow_records_borrow_id",
                        column: x => x.borrow_id,
                        principalTable: "borrow_records",
                        principalColumn: "borrow_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "fines",
                columns: table => new
                {
                    fine_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    borrow_detail_id = table.Column<int>(type: "int", nullable: true),
                    days_overdue = table.Column<int>(type: "int", nullable: false),
                    fine_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    paid_status = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    paid_date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fines", x => x.fine_id);
                    table.ForeignKey(
                        name: "FK_fines_borrow_details_borrow_detail_id",
                        column: x => x.borrow_detail_id,
                        principalTable: "borrow_details",
                        principalColumn: "borrow_detail_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "roles",
                columns: new[] { "role_id", "description", "role_name" },
                values: new object[,]
                {
                    { 1, "System administrator", "Admin" },
                    { 2, "Library staff", "Librarian" },
                    { 3, "Library member", "Member" }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "user_id", "created_at", "email", "full_name", "password_hash", "phone", "role_id", "status", "username" },
                values: new object[] { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@lms.com", "System Administrator", "$2a$11$CjFWpqhB/5WJkQrdWPhXiOBCCL4X1RDt8cqIvn.p2cpCxIlWuPMpC", null, 1, "Active", "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_book_authors_author_id",
                table: "book_authors",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_books_category_id",
                table: "books",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_books_isbn",
                table: "books",
                column: "isbn",
                unique: true,
                filter: "[isbn] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_borrow_details_book_id",
                table: "borrow_details",
                column: "book_id");

            migrationBuilder.CreateIndex(
                name: "IX_borrow_details_borrow_id",
                table: "borrow_details",
                column: "borrow_id");

            migrationBuilder.CreateIndex(
                name: "IX_borrow_records_librarian_id",
                table: "borrow_records",
                column: "librarian_id");

            migrationBuilder.CreateIndex(
                name: "IX_borrow_records_member_id",
                table: "borrow_records",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "IX_fines_borrow_detail_id",
                table: "fines",
                column: "borrow_detail_id",
                unique: true,
                filter: "[borrow_detail_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_email",
                table: "users",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_username",
                table: "users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "book_authors");

            migrationBuilder.DropTable(
                name: "fines");

            migrationBuilder.DropTable(
                name: "authors");

            migrationBuilder.DropTable(
                name: "borrow_details");

            migrationBuilder.DropTable(
                name: "books");

            migrationBuilder.DropTable(
                name: "borrow_records");

            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "roles");
        }
    }
}
