using FluentValidation;
using Messenger.Api.Areas.Validation;
using Messenger.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Api.UnitTests.Validation
{
    public class UsernameLoginDtoValidatorTests
    {
        private readonly IValidator<UsernameLoginDto> _validator;

        public UsernameLoginDtoValidatorTests()
        {
            _validator = new UsernameLoginDtoValidator();
        }

        [Fact]
        public void Validate_Valid_Valid()
        {
            // Arrange
            var usernameLoginDto = new UsernameLoginDto("ValidUsername", "PAssword123!@#");

            // Act
            var result = _validator.Validate(usernameLoginDto);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_UsernameSpecialChar_Invalid()
        {
            // Arrange
            var usernameLoginDto = new UsernameLoginDto("InvalidUsername@", "PAssword123!@#");

            // Act
            var result = _validator.Validate(usernameLoginDto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(UsernameLoginDto.Username) && e.ErrorMessage == "Username must contain only letters and numbers");
        }

        [Fact]
        public void Validate_PasswordNoCaps_Invalid()
        {
            // Arrange
            UsernameLoginDto usernameLoginDto = new("ValidUsername", "password123!@#");

            // Act
            var result = _validator.Validate(usernameLoginDto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(UsernameLoginDto.Password) && e.ErrorMessage == "Password must contain at least 2 uppercase letters");
        }
    }
}
