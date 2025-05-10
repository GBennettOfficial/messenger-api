using Dapper;
using Messenger.Api.Areas.Data.Abstractions;
using Messenger.Api.Areas.Data.Models;
using Messenger.Api.Utilities;
using Microsoft.Data.SqlClient;

namespace Messenger.Api.Areas.Data.Services
{
    public class UserReader : IUserReader
    {
        private readonly string _connectionString;
        private readonly ILogger<UserReader> _logger;

        public UserReader(IConfiguration configuration, ILogger<UserReader> logger)
        {
            _connectionString = configuration.GetConnectionString("Messenger")!;
            _logger = logger;
        }

        public async Task<Result<User>> SearchByUsername(string username)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();
                User? user = await conn.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Username = @Username", new { Username = username }, commandType: System.Data.CommandType.Text);
                if (user is null)
                    return new(ResultCode.NotFound, user);

                return new Result<User>(ResultCode.Success, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding user by Username: {Username}", username);
                return new(ResultCode.Error, null, "Failure reading user from database");
            }
        }

        public async Task<Result<User>> SearchByUserCode(string userCode)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();
                User? user = await conn.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE UserCode = @UserCode", new { UserCode = userCode }, commandType: System.Data.CommandType.Text);
                if (user is null)
                    return new(ResultCode.NotFound, user);

                return new Result<User>(ResultCode.Success, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding user by UserCode: {UserCode}", userCode);
                return new(ResultCode.Error, null, "Failure reading user from database");
            }
        }

        public async Task<Result<User>> SearchByEmail(string email)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();
                User? user = await conn.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Email = @Email", new { Email = email }, commandType: System.Data.CommandType.Text);
                if (user is null)
                    return new(ResultCode.NotFound, user);

                return new Result<User>(ResultCode.Success, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding user by Email: {Email}", email);
                return new(ResultCode.Error, null, "Failure reading user from database");
            }
        }

        public async Task<Result<User>> SearchByPhone(string phone)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();
                User? user = await conn.QuerySingleOrDefaultAsync<User>("SELECT * FROM Users WHERE Phone = @Phone", new { Phone = phone }, commandType: System.Data.CommandType.Text);
                if (user is null)
                    return new(ResultCode.NotFound, user);

                return new Result<User>(ResultCode.Success, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding user by Phone: {Phone}", phone);
                return new(ResultCode.Error, null, "Failure reading user from database");
            }
        }
    }
}
