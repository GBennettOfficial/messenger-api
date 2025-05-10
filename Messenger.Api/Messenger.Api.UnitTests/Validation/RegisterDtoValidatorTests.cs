using FluentValidation;
using Messenger.Api.Areas.Data.Abstractions;
using Messenger.Api.Areas.Data.Models;
using Messenger.Api.Areas.Validation;
using Messenger.Api.Utilities;
using Messenger.Common;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Api.UnitTests.Validation
{
    public class RegisterDtoValidatorTests
    {
        private readonly IValidator<RegisterDto> _validator;
        private readonly IUserReader _userReader;
        private readonly User _emptyUser;

        public RegisterDtoValidatorTests()
        {
            _userReader = NSubstitute.Substitute.For<IUserReader>();
            _validator = new RegisterDtoValidator(_userReader);

            _userReader.SearchByUsername(Arg.Any<string>())
                .Returns(new Result<User>(ResultCode.NotFound, null, "x"));
            _userReader.SearchByEmail(Arg.Any<string>())             
                .Returns(new Result<User>(ResultCode.NotFound, null, "x"));
            _userReader.SearchByPhone(Arg.Any<string>())             
                .Returns(new Result<User>(ResultCode.NotFound, null, "x"));

            _emptyUser = new User(0, "", "", "", "", "", null, "", "");
        }

        [Fact]
        public async Task Validate_UsernameExists_Invalid()
        {
            // Arrange
            _userReader.SearchByUsername("JohnCornwallis").Returns(new Result<User>(ResultCode.Success, _emptyUser));
            var registerDto = new RegisterDto("JohnCornwallis", "John", "CornWallis", "JohnCornwallis@email.com", null, null, "PAssword123!@#");

            // Act
            var result = await _validator.ValidateAsync(registerDto);  

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.Username) && e.ErrorMessage == "Username already exists");
        }

        [Fact]
        public async Task Validate_EmailExists_Invalid()
        {
            // Arrange
            _userReader.SearchByEmail("JohnCornwallis@email.com").Returns(new Result<User>(ResultCode.Success, _emptyUser));
            var registerDto = new RegisterDto("JohnCornwallis", "John", "CornWallis", "JohnCornwallis@email.com", null, null, "PAssword123!@#");

            // Act
            var result = await _validator.ValidateAsync(registerDto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.Email) && e.ErrorMessage == "Email already exists");
        }

        [Fact]
        public async Task Validate_PhoneExists_Invalid()
        {
            // Arrange
            _userReader.SearchByPhone("1234567").Returns(new Result<User>(ResultCode.Success, _emptyUser));
            var registerDto = new RegisterDto("JohnCornwallis", "John", "CornWallis", "JohnCornwallis@email.com", "1234567", null, "PAssword123!@#");

            // Act
            var result = await _validator.ValidateAsync(registerDto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.Phone) && e.ErrorMessage == "Phone already exists");
        }


        [Fact]
        public async Task Validate_NoPhoneNoPicture_Valid()
        {
            // Arrange  
            var registerDto = new RegisterDto("WilliamThomas", "William", "Thomas", "WilliamThomas@email.com", null, null, "PAssword123!@#");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Validate_WithPhoneNoPicture_Valid()
        {
            // Arrange  
            var registerDto = new RegisterDto("WilliamThomas", "William", "Thomas", "WilliamThomas@email.com", "1234567", null, "PAssword123!@#");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Validate_NoPhoneWithPicture_Valid()
        {
            // Arrange  
            var registerDto = new RegisterDto("WilliamThomas", "William", "Thomas", "WilliamThomas@email.com", null, new string('0', 100_000), "PAssword123!@#");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Validate_WithPhoneWithPicture_Valid()
        {
            // Arrange  
            var registerDto = new RegisterDto("WilliamThomas", "William", "Thomas", "WilliamThomas@email.com", "1234567", new string('0', 100_000), "PAssword123!@#");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Validate_EmptyUsername_Invalid()
        {
            // Arrange  
            var registerDto = new RegisterDto("", "Joe", "Smith", "JoeSmith@email.com", null, null, "PAssword123!@#");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.Username) && e.ErrorMessage == "Username must not be empty");
        }

        [Fact]
        public async Task Validate_ShortUsername_Invalid()
        {
            // Arrange  
            var registerDto = new RegisterDto("Joe", "Joe", "Smith", "JoeSmith@email.com", null, null, "PAssword123!@#");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.Username) && e.ErrorMessage == "Username must be between 5 and 120 characters");
        }

        [Fact]
        public async Task Validate_LongUsername_Invalid()
        {
            // Arrange
            var registerDto = new RegisterDto(new string('A', 121), "Joe", "Smith", "JoeSmith@email.com", null, null, "PAssword123!@#");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.Username) && e.ErrorMessage == "Username must be between 5 and 120 characters");
        }

        [Fact]
        public async Task Validate_InvalidEmail_Invalid()
        {
            // Arrange
            var registerDto = new RegisterDto("JoeSmith", "Joe", "Smith", "InvalidEmail", null, null, "PAssword123!@#");

            // Act
            var result = await _validator.ValidateAsync(registerDto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.Email) && e.ErrorMessage == "Email must be a valid email address");
        }

        [Fact]
        public async Task Validate_LongEmail_Invalid()
        {
            // Arrange
            var registerDto = new RegisterDto("JoeSmith", "Joe", "Smith", new string('A', 125) + "@email.com", null, null, "PAssword123!@#");

            // Act
            var result = await _validator.ValidateAsync(registerDto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.Email) && e.ErrorMessage == "Email must be between 5 and 120 characters");
        }

        [Fact]
        public async Task Validate_EmptyFirstName_Invalid()
        {
            // Arrange  
            var registerDto = new RegisterDto("ValidUsername", "", "Smith", "ValidEmail@email.com", null, null, "PAssword123!@#");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.FirstName) && e.ErrorMessage == "First name must not be empty");
        }

        [Fact]
        public async Task Validate_ShortFirstName_Invalid()
        {
            // Arrange  
            var registerDto = new RegisterDto("ValidUsername", "A", "Smith", "ValidEmail@email.com", null, null, "PAssword123!@#");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.FirstName) && e.ErrorMessage == "First name must be between 2 and 35 characters");
        }

        [Fact]
        public async Task Validate_LongFirstName_Invalid()
        {
            // Arrange  
            var registerDto = new RegisterDto("ValidUsername", new string('A', 36), "Smith", "ValidEmail@email.com", null, null, "PAssword123!@#");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.FirstName) && e.ErrorMessage == "First name must be between 2 and 35 characters");
        }

        [Fact]
        public async Task Validate_EmptyLastName_Invalid()
        {
            // Arrange  
            var registerDto = new RegisterDto("ValidUsername", "John", "", "ValidEmail@email.com", null, null, "PAssword123!@#");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.LastName) && e.ErrorMessage == "Last name must not be empty");
        }


        [Fact]
        public async Task Validate_ShortLastName_Invalid()
        {
            // Arrange  
            var registerDto = new RegisterDto("ValidUsername", "John", "A", "ValidEmail@email.com", null, null, "PAssword123!@#");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.LastName) && e.ErrorMessage == "Last name must be between 2 and 35 characters");
        }

        [Fact]
        public async Task Validate_LongLastName_Invalid()
        {
            // Arrange  
            var registerDto = new RegisterDto("ValidUsername", "John", new string('A', 36), "ValidEmail@email.com", null, null, "PAssword123!@#");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.LastName) && e.ErrorMessage == "Last name must be between 2 and 35 characters");
        }

        [Fact]
        public async Task Validate_ShortPhoneNumber_Invalid()
        {
            // Arrange  
            var registerDto = new RegisterDto("ValidUsername", "John", "Smith", "ValidEmail@email.com", "123", null, "PAssword123!@#");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.Phone) && e.ErrorMessage == "Phone number must be between 7 and 13 digits");
        }

        [Fact]
        public async Task Validate_LongPhoneNumber_Invalid()
        {
            // Arrange  
            var registerDto = new RegisterDto("ValidUsername", "John", "Smith", "ValidEmail@email.com", "12345678910111213", null, "PAssword123!@#");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.Phone) && e.ErrorMessage == "Phone number must be between 7 and 13 digits");
        }

        [Fact]
        public async Task Validate_NonDigitPhoneNumber_Invalid()
        {
            // Arrange
            var registerDto = new RegisterDto("ChristopherAnthony", "Christopher", "Anthony", "ChristopherAnthony@email.com", "1234567ABC", null, "PAssword123!@#");

            // Act
            var result = await _validator.ValidateAsync(registerDto);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.Phone) && e.ErrorMessage == "Phone number must contain only digits");
        }

        [Fact]
        public async Task Validate_WeakPassword_Invalid()
        {
            // Arrange  
            var registerDto = new RegisterDto("ValidUsername", "John", "Smith", "ValidEmail@email.com", null, null, "password");

            // Act  
            var result = await _validator.ValidateAsync(registerDto);

            // Assert  
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.Password) && e.ErrorMessage == "Password must contain at least 2 uppercase letters");
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.Password) && e.ErrorMessage == "Password must contain at least 2 digits");
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(RegisterDto.Password) && e.ErrorMessage == "Password must contain at least 2 special chars (!@#$%^&*<>~)");
        }
    }
}
