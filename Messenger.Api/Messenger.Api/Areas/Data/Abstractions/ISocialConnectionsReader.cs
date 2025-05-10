using Messenger.Api.Utilities;
using Messenger.Common;

namespace Messenger.Api.Areas.Data.Abstractions
{
    public interface ISocialConnectionsReader
    {
        Task<Result<SocialConnectionsDto>> Read(string username);
    }
}