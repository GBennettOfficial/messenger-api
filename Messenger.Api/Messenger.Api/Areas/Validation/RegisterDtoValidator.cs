using FluentValidation;
using Messenger.Api.Areas.Data.Abstractions;
using Messenger.Api.Utilities;
using Messenger.Common;

namespace Messenger.Api.Areas.Validation
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator(IUserReader userReader)
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Username must not be empty")
                .Length(5, 120)
                .WithMessage("Username must be between 5 and 120 characters")
                .Matches(@"^[a-zA-Z0-9]+$")
                .WithMessage("Username must contain only letters and numbers")
                .MustAsync(async (x, ct) =>
                {
                    if (ct.IsCancellationRequested)
                        return false;

                    var result = await userReader.FindByUsername(x);
                    return result.Code == ResultCode.NotFound;
                })
                .WithMessage("Username already exists");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name must not be empty")
                .Length(2, 35)
                .WithMessage("First name must be between 2 and 35 characters")
                .Matches(@"^[a-zA-Z]+$")
                .WithMessage("First name must contain only letters");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name must not be empty")
                .Length(2, 35)
                .WithMessage("Last name must be between 2 and 35 characters")
                .Matches(@"^[a-zA-Z]+$")
                .WithMessage("Last name must contain only letters");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email must not be empty")
                .Length(5, 120)
                .WithMessage("Email must be between 5 and 120 characters")
                .Matches(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
                .WithMessage("Email must be a valid email address")
                .MustAsync(async (x, ct) =>
                {
                    if (ct.IsCancellationRequested)
                        return false;

                    var result = await userReader.FindByEmail(x);
                    return result.Code == ResultCode.NotFound;
                })
                .WithMessage("Email already exists");

            RuleFor(x => x.Phone)
                .Length(7, 13)
                .WithMessage("Phone number must be between 7 and 13 digits")
                .Matches(@"^\d+$")
                .WithMessage("Phone number must contain only digits")
                .MustAsync(async (x, ct) =>
                {
                    if (ct.IsCancellationRequested)
                        return false;

                    var result = await userReader.FindByPhone(x);
                    return result.Code == ResultCode.NotFound;
                })
                .WithMessage("Phone already exists");

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
