using LMS.Application.DTOs.Authors;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _service;
    public AuthorsController(IAuthorService service) => _service = service;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAuthorsAsync();
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetAuthorByIdAsync(id);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuthorDto dto)
    {
        var result = await _service.CreateAuthorAsync(dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.AuthorId }, result.Data)
            : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAuthorDto dto)
    {
        var result = await _service.UpdateAuthorAsync(id, dto);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAuthorAsync(id);
        return result.IsSuccess ? NoContent() : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }
}
