using LMS.Application.DTOs.Categories;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _service;
    public CategoriesController(ICategoryService service) => _service = service;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllCategoriesAsync();
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetCategoryByIdAsync(id);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-10: Admin creates a category.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        var result = await _service.CreateCategoryAsync(dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.CategoryId }, result.Data)
            : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-10: Admin updates a category.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        var result = await _service.UpdateCategoryAsync(id, dto);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-10: Admin deletes a category.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteCategoryAsync(id);
        return result.IsSuccess ? NoContent() : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }
}
