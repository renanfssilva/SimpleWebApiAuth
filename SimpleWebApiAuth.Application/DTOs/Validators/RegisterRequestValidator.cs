using FluentValidation;
using SimpleWebApiAuth.Application.DTOs.Authentication.Requests;

namespace SimpleWebApiAuth.Application.DTOs.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(r => r.Username)
                .NotEmpty();

            RuleFor(r => r.FullName)
                .NotEmpty()
                .Matches(@"^(?=.*[A-Za-z])[A-Za-z\- ]+$")
                .WithMessage("Full name must contain at least one letter and can only contain letters, spaces, and hyphens.");

            RuleFor(r => r.Password)
                .NotEmpty()
                .MinimumLength(6)
                .Matches(@"\d")
                .WithMessage("Password must contain at least one digit");

            RuleFor(r => r.ConfirmPassword)
                .NotEmpty()
                .Equal(r => r.Password)
                .WithMessage("Passwords do not match");
        }
    }
}
