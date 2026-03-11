using LMS.Application.Common;
using LMS.Application.DTOs.Authors;

namespace LMS.Application.Interfaces;

public interface IAuthorService
{
    Task<Result<IEnumerable<AuthorDto>>> GetAllAuthorsAsync();
    Task<Result<AuthorDto>> GetAuthorByIdAsync(int id);
    Task<Result<AuthorDto>> CreateAuthorAsync(CreateAuthorDto dto);
    Task<Result<AuthorDto>> UpdateAuthorAsync(int id, UpdateAuthorDto dto);
    Task<Result> DeleteAuthorAsync(int id);
}
