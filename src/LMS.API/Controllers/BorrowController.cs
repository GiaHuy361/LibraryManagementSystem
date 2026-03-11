using LMS.Application.DTOs.Borrows;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BorrowController : ControllerBase
{
    private readonly IBorrowService _borrowService;

    public BorrowController(IBorrowService borrowService) => _borrowService = borrowService;

    private int GetCurrentUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>UC-12: Librarian issues books to a member.</summary>
    [HttpPost("issue")]
    [Authorize(Roles = "Librarian,Admin")]
    public async Task<IActionResult> IssueBook([FromBody] IssueBorrowDto dto)
    {
        var librarianId = GetCurrentUserId();
        var result = await _borrowService.IssueBookAsync(librarianId, dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.BorrowId }, result.Data)
            : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-13: Librarian marks a borrow record as returned. Triggers fine calculation if overdue.</summary>
    [HttpPost("return/{borrowId:int}")]
    [Authorize(Roles = "Librarian,Admin")]
    public async Task<IActionResult> ReturnBook(int borrowId)
    {
        var librarianId = GetCurrentUserId();
        var result = await _borrowService.ReturnBookAsync(borrowId, librarianId);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-15: Librarian views all borrow records. Supports filter by memberId and date range.</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? memberId,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to)
    {
        var result = await _borrowService.GetAllRecordsAsync(memberId, from, to);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>Get a specific borrow record by ID.</summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _borrowService.GetRecordByIdAsync(id);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-18: Member views their own borrow history.</summary>
    [HttpGet("my-history")]
    public async Task<IActionResult> GetMyHistory()
    {
        var memberId = GetCurrentUserId();
        var result = await _borrowService.GetMemberHistoryAsync(memberId);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }
}
