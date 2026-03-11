using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Librarian")]
public class FinesController : ControllerBase
{
    private readonly IFineService _fineService;
    public FinesController(IFineService fineService) => _fineService = fineService;

    /// <summary>UC-14: Get fine details for a specific borrow detail.</summary>
    [HttpGet("borrow-detail/{borrowDetailId:int}")]
    public async Task<IActionResult> GetByBorrowDetail(int borrowDetailId)
    {
        var result = await _fineService.GetFineByBorrowDetailAsync(borrowDetailId);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>Get all unpaid fines.</summary>
    [HttpGet("unpaid")]
    public async Task<IActionResult> GetUnpaid()
    {
        var result = await _fineService.GetUnpaidFinesAsync();
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-14: Librarian confirms fine payment.</summary>
    [HttpPost("{fineId:int}/pay")]
    public async Task<IActionResult> Pay(int fineId)
    {
        var result = await _fineService.PayFineAsync(fineId);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }
}
