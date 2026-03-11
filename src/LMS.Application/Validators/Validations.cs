using FluentValidation;
using LMS.Application.DTOs.Auth;
using LMS.Application.DTOs.Books;
using LMS.Application.DTOs.Users;

namespace LMS.Application.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequestDto>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}

public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Username).NotEmpty().Length(3, 50).WithMessage("Username must be between 3 and 50 characters.");
        RuleFor(x => x.Password)
            .NotEmpty().MinimumLength(6).WithMessage("Password must be at least 6 characters.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one number.");
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("A valid email is required.");
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.RoleId).GreaterThan(0).WithMessage("A valid Role ID is required.");
    }
}

public class CreateBookValidator : AbstractValidator<CreateBookDto>
{
    public CreateBookValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(255).WithMessage("Title is required and must not exceed 255 characters.");
        RuleFor(x => x.Isbn).MaximumLength(20).WithMessage("ISBN must not exceed 20 characters.");
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative.");
    }
}
