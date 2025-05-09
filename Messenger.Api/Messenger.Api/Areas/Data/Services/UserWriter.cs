using Dapper;
using Messenger.Api.Areas.Data.Abstractions;
using Messenger.Api.Areas.Data.Models;
using Messenger.Api.Utilities;
using Microsoft.Data.SqlClient;

namespace Messenger.Api.Areas.Data.Services
{
    public class UserWriter : IUserWriter
    {
        private readonly string _connectionString;
        private readonly ILogger<UserWriter> _logger;

        public UserWriter(IConfiguration configuration, ILogger<UserWriter> logger)
        {
            _connectionString = configuration.GetConnectionString("Messenger")!;
            _logger = logger;
        }

        public async Task<Result<User>> Create(User user)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();
                int newId = await conn.ExecuteScalarAsync<int>("usp_Create_User", user, commandType: System.Data.CommandType.StoredProcedure);
                User newUser = user with { Id = newId };
                return new Result<User>(ResultCode.Success, newUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user '{Username}'", user.Username);
                return new Result<User>(ResultCode.Error, null, "Failure creating user in database");
            }
        }

        public async Task<Result> Update(User user)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();
                int rowsEffected = await conn.ExecuteAsync("usp_Update_User", user, commandType: System.Data.CommandType.StoredProcedure);
                if (rowsEffected == 0)
                    return new Result(ResultCode.NotFound, "User not found");
                return new Result(ResultCode.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user '{Username}'", user.Username);
                return new Result(ResultCode.Error, "Failure updating user in database");
            }
        }

        public async Task<Result> Delete(int id)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();
                int rowsEffected = await conn.ExecuteAsync("DELETE FROM Users WHERE Id = @Id", new { id }, commandType: System.Data.CommandType.Text);
                if (rowsEffected == 0)
                    return new Result(ResultCode.NotFound, "User not found");
                return new Result(ResultCode.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with id '{Id}'", id);
                return new Result(ResultCode.Error, "Failure deleting user in database");
            }
        }
    }
}
