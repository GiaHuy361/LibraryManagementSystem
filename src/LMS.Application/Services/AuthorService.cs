using LMS.Application.Common;
using LMS.Application.DTOs.Authors;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;

namespace LMS.Application.Services;

public class AuthorService : IAuthorService
{
    private readonly IUnitOfWork _uow;
    public AuthorService(IUnitOfWork uow) => _uow = uow;

    public async Task<Result<IEnumerable<AuthorDto>>> GetAllAuthorsAsync()
    {
        var authors = await _uow.Authors.GetAllAsync();
        return Result<IEnumerable<AuthorDto>>.Success(authors.Select(MapToDto));
    }

    public async Task<Result<AuthorDto>> GetAuthorByIdAsync(int id)
    {
        var author = await _uow.Authors.GetByIdAsync(id);
        if (author == null) return Result<AuthorDto>.NotFound($"Author {id} not found.");
        return Result<AuthorDto>.Success(MapToDto(author));
    }

    public async Task<Result<AuthorDto>> CreateAuthorAsync(CreateAuthorDto dto)
    {
        var author = new Author { AuthorName = dto.AuthorName, Biography = dto.Biography };
        await _uow.Authors.AddAsync(author);
        await _uow.SaveChangesAsync();
        return Result<AuthorDto>.Created(MapToDto(author));
    }

    public async Task<Result<AuthorDto>> UpdateAuthorAsync(int id, UpdateAuthorDto dto)
    {
        var author = await _uow.Authors.GetByIdAsync(id);
        if (author == null) return Result<AuthorDto>.NotFound($"Author {id} not found.");
        if (dto.AuthorName != null) author.AuthorName = dto.AuthorName;
        if (dto.Biography != null) author.Biography = dto.Biography;
        _uow.Authors.Update(author);
        await _uow.SaveChangesAsync();
        return Result<AuthorDto>.Success(MapToDto(author));
    }

    public async Task<Result> DeleteAuthorAsync(int id)
    {
        var author = await _uow.Authors.GetByIdAsync(id);
        if (author == null) return Result.NotFound($"Author {id} not found.");
        _uow.Authors.Remove(author);
        await _uow.SaveChangesAsync();
        return Result.Success();
    }

    private static AuthorDto MapToDto(Author a) => new()
    {
        AuthorId = a.AuthorId,
        AuthorName = a.AuthorName,
        Biography = a.Biography
    };
}
