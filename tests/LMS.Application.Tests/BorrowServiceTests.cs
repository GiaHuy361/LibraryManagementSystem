using LMS.Application.DTOs.Borrows;
using LMS.Application.Services;
using LMS.Application.Strategies;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using Moq;
using FluentAssertions;
using Xunit;

namespace LMS.Application.Tests;

public class BorrowServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IFineCalculationStrategy> _mockFineStrategy;
    private readonly BorrowService _borrowService;

    public BorrowServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockFineStrategy = new Mock<IFineCalculationStrategy>();
        _borrowService = new BorrowService(_mockUow.Object, _mockFineStrategy.Object);
    }

    [Fact]
    public async Task IssueBookAsync_ShouldReturnError_WhenBookNotFound()
    {
        // Arrange
        var request = new IssueBorrowDto 
        { 
            MemberId = 1, 
            Books = new List<BorrowBookItemDto> { new() { BookId = 999, Quantity = 1 } }, 
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)) 
        };
        _mockUow.Setup(u => u.Users.GetByIdAsync(1)).ReturnsAsync(new User { UserId = 1, RoleId = 3 }); // Valid Member
        _mockUow.Setup(u => u.Books.GetByIdAsync(999)).ReturnsAsync((Book?)null);

        // Act
        var result = await _borrowService.IssueBookAsync(2, request); // 2 = Librarian

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Book 999 not found");
    }

    [Fact]
    public async Task IssueBookAsync_ShouldReturnError_WhenQuantityIsZero()
    {
        // Arrange
        var request = new IssueBorrowDto 
        { 
            MemberId = 1, 
            Books = new List<BorrowBookItemDto> { new() { BookId = 1, Quantity = 1 } }, 
            DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)) 
        };
        _mockUow.Setup(u => u.Users.GetByIdAsync(1)).ReturnsAsync(new User { UserId = 1, RoleId = 3 }); 
        _mockUow.Setup(u => u.Books.GetByIdAsync(1)).ReturnsAsync(new Book { BookId = 1, Title = "Learn C#", Quantity = 0 }); // No stock

        // Act
        var result = await _borrowService.IssueBookAsync(2, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Book 'Learn C#' is not available");
    }

    [Fact]
    public async Task IssueBookAsync_ShouldDecreaseQuantityAndReturnSuccess_WhenValid()
    {
        // Arrange
        var dueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var request = new IssueBorrowDto 
        { 
            MemberId = 1, 
            Books = new List<BorrowBookItemDto> { new() { BookId = 1, Quantity = 1 } }, 
            DueDate = dueDate 
        };
        
        var validUser = new User { UserId = 1, RoleId = 3 };
        var validBook = new Book { BookId = 1, Quantity = 5, Title = "Clean Code" };

        _mockUow.Setup(u => u.Users.GetByIdAsync(1)).ReturnsAsync(validUser);
        _mockUow.Setup(u => u.Books.GetByIdAsync(1)).ReturnsAsync(validBook);
        _mockUow.Setup(u => u.Borrows.AddAsync(It.IsAny<BorrowRecord>())).Returns(Task.CompletedTask);

        // Required to return the newly created record with details mapping later if GetWithDetailsAsync is called
        _mockUow.Setup(u => u.Borrows.GetWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync(new BorrowRecord { BorrowId = 10, Member = validUser, BorrowDetails = [] });

        // Act
        var result = await _borrowService.IssueBookAsync(2, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        validBook.Quantity.Should().Be(4); // Decreased by 1
        _mockUow.Verify(u => u.SaveChangesAsync(), Times.AtLeastOnce);
    }
}
