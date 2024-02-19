using FluentValidation;
using SimpleWebApiAuth.Domain.Books;

namespace SimpleWebApiAuth.Application.DTOs.Validators
{
    public class BookValidator : AbstractValidator<Book>
    {
        public BookValidator()
        {
            RuleFor(b => b.BookName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(b => b.Price)
                .GreaterThanOrEqualTo(0);

            RuleFor(b => b.Category)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(b => b.Author)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}
