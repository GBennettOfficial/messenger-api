using Dapper;
using Messenger.Api.Areas.Cryptography.Abstractions;
using Messenger.Api.Areas.Cryptography.Models;
using Messenger.Api.Areas.Data.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Api.EndpointTests.Account
{
    public class RefreshTokenTests : IClassFixture<MessengerApiFactory>
    {
        private readonly HttpClient _client;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJsonWebTokenProvider _jsonWebTokenProvider;
        private readonly string _connectionString;

        public RefreshTokenTests(MessengerApiFactory factory)
        {
            _client = factory.CreateClient();
            _passwordHasher = factory.Inject<IPasswordHasher>();
            _jsonWebTokenProvider = factory.Inject<IJsonWebTokenProvider>();
            _connectionString = factory.GetConnectionString("Messenger");
        }

        [Fact]
        public async Task RefreshToken_Authorized_OkReturnToken()
        {
            // Arrange
            string password = "PAssword123!@#";
            HashedPassword hashedPassword = _passwordHasher.HashPassword(password);
            User user = new(0, "JuliaFinnington", "1234513244", "Julia", "Finnington", "JuliaFinnington@email.com", null, hashedPassword.Hash, hashedPassword.Salt);
            using SqlConnection conn = new(_connectionString);
            conn.Open();
            int newId = await conn.ExecuteScalarAsync<int>("usp_Create_User", user, commandType: System.Data.CommandType.StoredProcedure);
            user = user with { Id = newId };
            JsonWebToken jwt = _jsonWebTokenProvider.CreateJsonWebToken(user);
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt.Value);

            try
            {
                // Act
                HttpResponseMessage response = await _client.GetAsync("Account/RefreshToken");
                JsonWebToken? responseJwt = null;
                if (response.IsSuccessStatusCode)
                {
                    responseJwt = await response.Content.ReadFromJsonAsync<JsonWebToken>();
                }

                // Assert
                Assert.True(response.IsSuccessStatusCode);
                Assert.NotNull(responseJwt);
                Assert.NotNull(responseJwt.Value);
                Assert.NotEmpty(responseJwt.Value);
                Assert.True(responseJwt.Value.Length > 10);
                Assert.NotEqual(jwt.Value, responseJwt.Value);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                // Clean up
                conn.Execute("DELETE FROM Users WHERE Username = @Username", new { user.Username });
            }
        }

        [Fact]
        public async Task RefreshToken_NotAuthorized_Unauthorized()
        {
            // Arrange
            _client.DefaultRequestHeaders.Clear();

            // Act
            HttpResponseMessage response = await _client.GetAsync("Account/RefreshToken");

            // Assert
            Assert.False(response.IsSuccessStatusCode);
            Assert.True(response.StatusCode == System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
