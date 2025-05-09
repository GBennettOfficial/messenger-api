using Messenger.Api.Areas.Cryptography.Abstractions;
using Messenger.Api.Areas.Cryptography.Models;
using Messenger.Api.Areas.Cryptography.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Api.UnitTests.Cryptography
{
    public class PasswordHasherTests
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPasswordChecker _passwordChecker;
        public PasswordHasherTests()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets<PasswordHasherTests>()
                .Build();

            _passwordHasher = new PasswordHasher(configuration);
            _passwordChecker = new PasswordChecker(configuration);
        }

        [Fact]
        public void HashPassword_NotEmpty()
        {
            // Arrange
            string password = "TestPassword123!@#";

            // Act
            HashedPassword result = _passwordHasher.HashPassword(password);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Hash);
            Assert.NotEmpty(result.Salt);
            Assert.True(result.Hash.Length > 10);
            Assert.True(result.Salt.Length > 10);
        }

        [Fact]
        public void HashPasswordCheckPassword_CorrectInput_True()
        {
            // Arrange
            string password = "TestPassword123!@#";

            // Act
            HashedPassword hashedPassword = _passwordHasher.HashPassword(password);
            bool isValid = _passwordChecker.CheckPassword(password, hashedPassword);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void HashPasswordCheckPassword_IncorrectInput_False()
        {
            // Arrange
            string password = "TestPassword123!@#";
            string incorrectPassword = "TestPassword1234!@#";

            // Act
            HashedPassword hashedPassword = _passwordHasher.HashPassword(password);
            bool isValid = _passwordChecker.CheckPassword(incorrectPassword, hashedPassword);

            // Assert
            Assert.False(isValid);
        }
    }
}
