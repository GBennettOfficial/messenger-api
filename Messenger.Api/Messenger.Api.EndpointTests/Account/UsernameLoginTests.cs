using Dapper;
using Messenger.Api.Areas.Cryptography.Abstractions;
using Messenger.Api.Areas.Cryptography.Models;
using Messenger.Api.Areas.Data.Abstractions;
using Messenger.Api.Areas.Data.Models;
using Messenger.Common;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Api.EndpointTests.Account
{
    public class UsernameLoginTests : IClassFixture<MessengerApiFactory>
    {
        private readonly HttpClient _client;
        private readonly IPasswordHasher _passwordHasher;
        private readonly string _connectionString;

        public UsernameLoginTests(MessengerApiFactory factory)
        {
            _client = factory.CreateClient();
            _connectionString = factory.GetConnectionString("Messenger");
            _passwordHasher = _passwordHasher = factory.Inject<IPasswordHasher>();
        }

        [Fact]
        public async Task UsernameLogin_Valid_OkUserCreatedTokenCreated()
        {
            // Arrange
            string password = "PAssword123!@#";
            HashedPassword hashedPassword = _passwordHasher.HashPassword(password);
            User user = new(0, "SophiaMartinez", "12345678976", "Sophia", "Martinez", "SophiaMartinez@email.com", null, hashedPassword.Hash, hashedPassword.Salt);
            using SqlConnection conn = new(_connectionString);
            conn.Open();
            conn.Execute("usp_Create_User", user, commandType: System.Data.CommandType.StoredProcedure);
            UsernameLoginDto usernameLoginDto = new(user.Username, password);

            try
            {
                // Act
                HttpResponseMessage response = await _client.PostAsJsonAsync("Account/UsernameLogin", usernameLoginDto);
                JsonWebToken? jwt = null;
                if (response.IsSuccessStatusCode)
                {
                    jwt = await response.Content.ReadFromJsonAsync<JsonWebToken>();
                }

                // Assert
                Assert.True(response.IsSuccessStatusCode);
                Assert.NotNull(jwt);
                Assert.NotNull(jwt.Value);
                Assert.NotEmpty(jwt.Value);
                Assert.True(jwt.Value.Length > 10);
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
        public async Task UsernameLogin_WrongPassword_BadRequestNoToken()
        {
            // Arrange
            string password = "PAssword123!@#";
            string invalidPassword = "PAssword1234!@#";
            HashedPassword hashedPassword = _passwordHasher.HashPassword(password);
            User user = new(0, "JohnsonSmith", "12455123454", "Johnson", "Smith", "JohnsonSmith@email.com", null, hashedPassword.Hash, hashedPassword.Salt);
            using SqlConnection conn = new(_connectionString);
            conn.Open();
            conn.Execute("usp_Create_User", user, commandType: System.Data.CommandType.StoredProcedure);
            UsernameLoginDto usernameLoginDto = new(user.Username, invalidPassword);

            try
            {
                // Act
                HttpResponseMessage response = await _client.PostAsJsonAsync("Account/UsernameLogin", usernameLoginDto);
                string? message = null;
                if (response.IsSuccessStatusCode == false)
                {
                    message = await response.Content.ReadAsStringAsync();
                }

                // Assert
                Assert.False(response.IsSuccessStatusCode);
                Assert.True(response.StatusCode == System.Net.HttpStatusCode.BadRequest);
                Assert.NotNull(message);
                Assert.Contains("Invalid username or password", message);
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
    }
}
