using Messenger.Api.Areas.Cryptography.Abstractions;
using Messenger.Api.Areas.Cryptography.Models;
using Messenger.Api.Areas.Cryptography.Services;
using Messenger.Api.Areas.Data.Models;
using Messenger.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Api.UnitTests.Cryptography
{
    public class JsonWebTokenProviderTests
    {
        private readonly IJsonWebTokenProvider _jsonWebTokenProvider;

        public JsonWebTokenProviderTests()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddUserSecrets<JsonWebTokenProviderTests>()
                .Build();

            _jsonWebTokenProvider = new JsonWebTokenProvider(configuration);
        }

        [Fact]
        public void CreateToken_ValidInput_NotEmpty()
        {
            // Arrange
            User user = new(0, "JohnDoe", "123456789012", "John", "Doe", "JohnDoe@email.com", null, "x", "x");

            // Act
            JsonWebTokenDto jsonWebToken = _jsonWebTokenProvider.CreateJsonWebToken(user);

            // Assert
            Assert.NotNull(jsonWebToken);
            Assert.NotEmpty(jsonWebToken.Value);
            Assert.True(jsonWebToken.Value.Length > 10);
        }
    }
}
