using FluentValidation;
using SimpleWebApiAuth.Application.DTOs.Authentication.Requests;

namespace SimpleWebApiAuth.Application.DTOs.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(r => r.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(r => r.Password)
                .NotEmpty();
        }
    }
}
