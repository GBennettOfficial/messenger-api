using FluentValidation;
using Messenger.Common;

namespace Messenger.Api.Areas.Validation
{
    public class UsernameLoginDtoValidator : AbstractValidator<UsernameLoginDto>
    {
        public UsernameLoginDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username must not be empty")
                .Length(5, 120)
                .WithMessage("Username must be between 5 and 120 characters")
                .Matches(@"^[a-zA-Z0-9]+$")
                .WithMessage("Username must contain only letters and numbers");

            RuleFor(x => x.Password)
                .Length(8, 30)
                .WithErrorCode("Password must be between 8 and 30 characters")
                .Must(x => x.Count(char.IsUpper) >= 2)
                .WithMessage("Password must contain at least 2 uppercase letters")
                .Must(x => x.Count(char.IsLower) >= 2)
                .WithMessage("Password must contain at least 2 lowercase letters")
                .Must(x => x.Count(char.IsDigit) >= 2)
                .WithMessage("Password must contain at least 2 digits")
                .Must(x => x.Count(x => "!@#$%^&*<>~".Contains(x)) >= 2)
                .WithMessage("Password must contain at least 2 special chars (!@#$%^&*<>~)");
        }
    }
}
