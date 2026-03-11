using LMS.Application.Common;
using LMS.Application.DTOs.Categories;

namespace LMS.Application.Interfaces;

/// <summary>UC-10 Manage Category</summary>
public interface ICategoryService
{
    Task<Result<IEnumerable<CategoryDto>>> GetAllCategoriesAsync();
    Task<Result<CategoryDto>> GetCategoryByIdAsync(int id);
    Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto);
    Task<Result<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
    Task<Result> DeleteCategoryAsync(int id);
}
