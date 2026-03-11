using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        builder.HasKey(r => r.RoleId);
        builder.Property(r => r.RoleId).HasColumnName("role_id").UseIdentityColumn();
        builder.Property(r => r.RoleName).HasColumnName("role_name").HasMaxLength(50).IsRequired();
        builder.Property(r => r.Description).HasColumnName("description").HasMaxLength(255);

        // Seed roles
        builder.HasData(
            new Role { RoleId = 1, RoleName = "Admin", Description = "System administrator" },
            new Role { RoleId = 2, RoleName = "Librarian", Description = "Library staff" },
            new Role { RoleId = 3, RoleName = "Member", Description = "Library member" }
        );
    }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.UserId);
        builder.Property(u => u.UserId).HasColumnName("user_id").UseIdentityColumn();
        builder.Property(u => u.Username).HasColumnName("username").HasMaxLength(50).IsRequired();
        builder.HasIndex(u => u.Username).IsUnique();
        builder.Property(u => u.PasswordHash).HasColumnName("password_hash").HasMaxLength(255).IsRequired();
        builder.Property(u => u.FullName).HasColumnName("full_name").HasMaxLength(100);
        builder.Property(u => u.Email).HasColumnName("email").HasMaxLength(100);
        builder.HasIndex(u => u.Email).IsUnique().HasFilter("[email] IS NOT NULL");
        builder.Property(u => u.Phone).HasColumnName("phone").HasMaxLength(20);
        builder.Property(u => u.RoleId).HasColumnName("role_id");
        builder.Property(u => u.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("Active");
        builder.Property(u => u.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");

        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(u => u.BorrowsAsMember)
            .WithOne(b => b.Member)
            .HasForeignKey(b => b.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.BorrowsAsLibrarian)
            .WithOne(b => b.Librarian)
            .HasForeignKey(b => b.LibrarianId)
            .OnDelete(DeleteBehavior.SetNull);

        // Seed admin user — password: Admin@123
        builder.HasData(new User
        {
            UserId = 1,
            Username = "admin",
            PasswordHash = "$2a$11$M4.5zL9l6TXovQL/nx8OWeJFvAC6KGjucghuGRfxex57AZf17o3ZO",
            FullName = "System Administrator",
            Email = "admin@lms.com",
            RoleId = 1,
            Status = "Active",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.CategoryId);
        builder.Property(c => c.CategoryId).HasColumnName("category_id").UseIdentityColumn();
        builder.Property(c => c.CategoryName).HasColumnName("category_name").HasMaxLength(100).IsRequired();
        builder.Property(c => c.Description).HasColumnName("description").HasMaxLength(255);

        builder.HasData(
            new Category { CategoryId = 1, CategoryName = "Information Technology", Description = "Programming, Networking, and Systems" },
            new Category { CategoryId = 2, CategoryName = "Science Fiction", Description = "Sci-Fi novels and literature" },
            new Category { CategoryId = 3, CategoryName = "Business", Description = "Management, Finance, and Economics" }
        );
    }
}

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("authors");
        builder.HasKey(a => a.AuthorId);
        builder.Property(a => a.AuthorId).HasColumnName("author_id").UseIdentityColumn();
        builder.Property(a => a.AuthorName).HasColumnName("author_name").HasMaxLength(150).IsRequired();
        builder.Property(a => a.Biography).HasColumnName("biography").HasColumnType("NVARCHAR(MAX)");

        builder.HasData(
            new Author { AuthorId = 1, AuthorName = "Robert C. Martin", Biography = "Uncle Bob, software engineer and author" },
            new Author { AuthorId = 2, AuthorName = "Martin Fowler", Biography = "Software developer and author" },
            new Author { AuthorId = 3, AuthorName = "Frank Herbert", Biography = "American science fiction writer" }
        );
    }
}

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("books");
        builder.HasKey(b => b.BookId);
        builder.Property(b => b.BookId).HasColumnName("book_id").UseIdentityColumn();
        builder.Property(b => b.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
        builder.Property(b => b.Isbn).HasColumnName("isbn").HasMaxLength(20);
        builder.HasIndex(b => b.Isbn).IsUnique().HasFilter("[isbn] IS NOT NULL");
        builder.Property(b => b.CategoryId).HasColumnName("category_id");
        builder.Property(b => b.Publisher).HasColumnName("publisher").HasMaxLength(150);
        builder.Property(b => b.PublishYear).HasColumnName("publish_year");
        builder.Property(b => b.Description).HasColumnName("description").HasColumnType("NVARCHAR(MAX)");
        builder.Property(b => b.Quantity).HasColumnName("quantity").HasDefaultValue(1);
        builder.Property(b => b.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("Available");
        builder.Property(b => b.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");

        builder.HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasData(
            new Book { BookId = 1, Title = "Clean Code: A Handbook of Agile Software Craftsmanship", Isbn = "9780132350884", CategoryId = 1, Publisher = "Prentice Hall", PublishYear = 2008, Quantity = 5, Status = "Available", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Book { BookId = 2, Title = "Refactoring: Improving the Design of Existing Code", Isbn = "9780201485677", CategoryId = 1, Publisher = "Addison-Wesley", PublishYear = 1999, Quantity = 3, Status = "Available", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Book { BookId = 3, Title = "Dune", Isbn = "9780441172719", CategoryId = 2, Publisher = "Chilton Books", PublishYear = 1965, Quantity = 10, Status = "Available", CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}

public class BookAuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
{
    public void Configure(EntityTypeBuilder<BookAuthor> builder)
    {
        builder.ToTable("book_authors");
        builder.HasKey(ba => new { ba.BookId, ba.AuthorId });
        builder.Property(ba => ba.BookId).HasColumnName("book_id");
        builder.Property(ba => ba.AuthorId).HasColumnName("author_id");

        builder.HasOne(ba => ba.Book)
            .WithMany(b => b.BookAuthors)
            .HasForeignKey(ba => ba.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ba => ba.Author)
            .WithMany(a => a.BookAuthors)
            .HasForeignKey(ba => ba.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(
            new BookAuthor { BookId = 1, AuthorId = 1 },
            new BookAuthor { BookId = 2, AuthorId = 2 },
            new BookAuthor { BookId = 3, AuthorId = 3 }
        );
    }
}

public class BorrowRecordConfiguration : IEntityTypeConfiguration<BorrowRecord>
{
    public void Configure(EntityTypeBuilder<BorrowRecord> builder)
    {
        builder.ToTable("borrow_records");
        builder.HasKey(r => r.BorrowId);
        builder.Property(r => r.BorrowId).HasColumnName("borrow_id").UseIdentityColumn();
        builder.Property(r => r.MemberId).HasColumnName("member_id").IsRequired();
        builder.Property(r => r.LibrarianId).HasColumnName("librarian_id");
        builder.Property(r => r.BorrowDate).HasColumnName("borrow_date").HasColumnType("date");
        builder.Property(r => r.DueDate).HasColumnName("due_date").HasColumnType("date");
        builder.Property(r => r.ReturnDate).HasColumnName("return_date").HasColumnType("date");
        builder.Property(r => r.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("Borrowing");
        builder.Property(r => r.TotalFine).HasColumnName("total_fine").HasColumnType("decimal(10,2)").HasDefaultValue(0);
    }
}

public class BorrowDetailConfiguration : IEntityTypeConfiguration<BorrowDetail>
{
    public void Configure(EntityTypeBuilder<BorrowDetail> builder)
    {
        builder.ToTable("borrow_details");
        builder.HasKey(d => d.BorrowDetailId);
        builder.Property(d => d.BorrowDetailId).HasColumnName("borrow_detail_id").UseIdentityColumn();
        builder.Property(d => d.BorrowId).HasColumnName("borrow_id");
        builder.Property(d => d.BookId).HasColumnName("book_id");
        builder.Property(d => d.Quantity).HasColumnName("quantity").HasDefaultValue(1);
        builder.Property(d => d.ReturnDate).HasColumnName("return_date").HasColumnType("date");
        builder.Property(d => d.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("Borrowed");

        builder.HasOne(d => d.BorrowRecord)
            .WithMany(r => r.BorrowDetails)
            .HasForeignKey(d => d.BorrowId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Book)
            .WithMany(b => b.BorrowDetails)
            .HasForeignKey(d => d.BookId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(d => d.Fine)
            .WithOne(f => f.BorrowDetail)
            .HasForeignKey<Fine>(f => f.BorrowDetailId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class FineConfiguration : IEntityTypeConfiguration<Fine>
{
    public void Configure(EntityTypeBuilder<Fine> builder)
    {
        builder.ToTable("fines");
        builder.HasKey(f => f.FineId);
        builder.Property(f => f.FineId).HasColumnName("fine_id").UseIdentityColumn();
        builder.Property(f => f.BorrowDetailId).HasColumnName("borrow_detail_id");
        builder.Property(f => f.DaysOverdue).HasColumnName("days_overdue");
        builder.Property(f => f.FineAmount).HasColumnName("fine_amount").HasColumnType("decimal(10,2)");
        builder.Property(f => f.PaidStatus).HasColumnName("paid_status").HasDefaultValue(false);
        builder.Property(f => f.PaidDate).HasColumnName("paid_date").HasColumnType("date");
    }
}
