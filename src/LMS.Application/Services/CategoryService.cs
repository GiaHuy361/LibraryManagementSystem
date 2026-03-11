using LMS.Application.Common;
using LMS.Application.DTOs.Categories;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;

namespace LMS.Application.Services;

/// <summary>Handles UC-10: Create, Update, Delete Category.</summary>
public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;
    public CategoryService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
    {
        var cats = await _uow.Categories.GetAllAsync();
        return Result<IEnumerable<CategoryDto>>.Success(cats.Select(MapToDto));
    }

    public async Task<Result<CategoryDto>> GetCategoryByIdAsync(int id)
    {
        var cat = await _uow.Categories.GetByIdAsync(id);
        if (cat == null) return Result<CategoryDto>.NotFound($"Category {id} not found.");
        return Result<CategoryDto>.Success(MapToDto(cat));
    }

    public async Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var cat = new Category { CategoryName = dto.CategoryName, Description = dto.Description };
        await _uow.Categories.AddAsync(cat);
        await _uow.SaveChangesAsync();
        return Result<CategoryDto>.Created(MapToDto(cat));
    }

    public async Task<Result<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
    {
        var cat = await _uow.Categories.GetByIdAsync(id);
        if (cat == null) return Result<CategoryDto>.NotFound($"Category {id} not found.");
        if (dto.CategoryName != null) cat.CategoryName = dto.CategoryName;
        if (dto.Description != null) cat.Description = dto.Description;
        _uow.Categories.Update(cat);
        await _uow.SaveChangesAsync();
        return Result<CategoryDto>.Success(MapToDto(cat));
    }

    public async Task<Result> DeleteCategoryAsync(int id)
    {
        var cat = await _uow.Categories.GetByIdAsync(id);
        if (cat == null) return Result.NotFound($"Category {id} not found.");
        _uow.Categories.Remove(cat);
        await _uow.SaveChangesAsync();
        return Result.Success();
    }

    private static CategoryDto MapToDto(Category c) => new()
    {
        CategoryId = c.CategoryId,
        CategoryName = c.CategoryName,
        Description = c.Description
    };
}
