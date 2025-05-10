using Dapper;
using Messenger.Api.Areas.Cryptography.Models;
using Messenger.Api.Areas.Data.Models;
using Messenger.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Messenger.Api.EndpointTests.Account
{
    public class RegisterTests : IClassFixture<MessengerApiFactory>
    {
        private readonly HttpClient _client;
        private readonly string _connectionString;

        public RegisterTests(MessengerApiFactory factory)
        {
            _client = factory.CreateClient();
            _connectionString = factory.GetConnectionString("Messenger");
        }

        [Fact]
        public async Task Register_Valid_OkUserCreatedTokenCreated()
        {
            // Arrange
            RegisterDto registerDto = new("BobSmith", "Bob", "Smith", "BobSmith@email.com", null, null, "PAssword123!@#");
            using SqlConnection conn = new(_connectionString);
            conn.Open();
            int startCount = conn.ExecuteScalar<int>("SELECT COUNT(*) FROM Users WHERE Username = @Username", new { registerDto.Username });

            try
            {

                // Act
                HttpResponseMessage response = await _client.PostAsJsonAsync("Account/Register", registerDto);
                JsonWebTokenDto? jwt = null;
                if (response.IsSuccessStatusCode)
                {
                    var loginResponseDto = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                    jwt = loginResponseDto?.JsonWebToken;
                }
                int endCount = conn.ExecuteScalar<int>("SELECT COUNT(*) FROM Users WHERE Username = @Username", new { registerDto.Username });

                // Assert
                Assert.Equal(0, startCount);
                Assert.True(response.IsSuccessStatusCode);
                Assert.NotNull(jwt);
                Assert.NotEmpty(jwt.Value);
                Assert.True(jwt.Value.Length > 10);
                Assert.Equal(1, endCount);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                // Clean up
                conn.Execute("DELETE FROM Users WHERE Username = @Username", new { registerDto.Username });
            }
        }

        [Fact]
        public async Task Register_Invalid_BadRequestUserNotCreated()
        {
            // Arrange
            string invalidPassword = "Password123";
            RegisterDto registerDto = new("CharlesHammilton", "Charles", "Hammilton", "CharlesHammilton@email.com", null, null, invalidPassword);
            using SqlConnection conn = new(_connectionString);
            conn.Open();
            int startCount = conn.ExecuteScalar<int>("SELECT COUNT(*) FROM Users WHERE Username = @Username", new { registerDto.Username });
            try
            {
                // Act
                HttpResponseMessage response = await _client.PostAsJsonAsync("Account/Register", registerDto);
                int endCount = conn.ExecuteScalar<int>("SELECT COUNT(*) FROM Users WHERE Username = @Username", new { registerDto.Username });
                string content = await response.Content.ReadAsStringAsync();


                // Assert
                Assert.Equal(0, startCount);
                Assert.False(response.IsSuccessStatusCode);
                Assert.Equal(0, endCount);
                Assert.Contains("Password must contain at least 2 special chars", content);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.ToString());
            }
            finally
            {
                // Clean up
                conn.Execute("DELETE FROM Users WHERE Username = @Username", new { registerDto.Username });
            }
        }

    }
}
