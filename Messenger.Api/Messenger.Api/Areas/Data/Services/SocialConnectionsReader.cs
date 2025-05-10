using Dapper;
using Messenger.Api.Areas.Data.Abstractions;
using Messenger.Api.Utilities;
using Messenger.Common;
using Microsoft.Data.SqlClient;

namespace Messenger.Api.Areas.Data.Services
{
    public class SocialConnectionsReader : ISocialConnectionsReader
    {
        private readonly string _connectionString;
        private readonly ILogger<SocialConnectionsReader> _logger;

        public SocialConnectionsReader(IConfiguration configuration, ILogger<SocialConnectionsReader> logger)
        {
            _connectionString = configuration.GetConnectionString("Messenger")!;
            _logger = logger;
        }

        public async Task<Result<SocialConnectionsDto>> Read(string username)
        {
            try
            {
                using SqlConnection conn = new(_connectionString);
                await conn.OpenAsync();
                List<FriendRequestDto> friendRequests = conn.Query<FriendRequestDto>("usp_Read_FriendRequests", new { Username = username }, commandType: System.Data.CommandType.StoredProcedure)
                        .ToList();
                List<GroupRequestDto> groupRequests = conn.Query<GroupRequestDto>("usp_Read_GroupRequests", new { Username = username }, commandType: System.Data.CommandType.StoredProcedure)
                        .ToList();
                List<FriendDto> friends = conn.Query<FriendDto>("usp_Read_Friends", new { Username = username }, commandType: System.Data.CommandType.StoredProcedure)
                        .ToList();
                List<GroupDto> groups = conn.Query<GroupDto>("usp_Read_Groups", new { Username = username }, commandType: System.Data.CommandType.StoredProcedure)
                        .ToList();

                SocialConnectionsDto socialConnections = new(friends, friendRequests, groups, groupRequests);

                return new Result<SocialConnectionsDto>(ResultCode.Success, socialConnections);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reading social connections for user '{Username}'", username);
                return new(ResultCode.Error, null, "Failure reading social connections from database");
            }
        }
    }
}
