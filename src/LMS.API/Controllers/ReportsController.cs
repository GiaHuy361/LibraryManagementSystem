using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;
    public ReportsController(IReportService reportService) => _reportService = reportService;

    /// <summary>UC-11: Admin views summary statistics report.</summary>
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var result = await _reportService.GetSummaryReportAsync();
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }
}
