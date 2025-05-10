using Messenger.Api.Areas.Cryptography.Models;
using Messenger.Api.Areas.Data.Models;
using Messenger.Common;

namespace Messenger.Api.Areas.Cryptography.Abstractions
{
    public interface IJsonWebTokenProvider
    {
        JsonWebTokenDto CreateJsonWebToken(User user);
    }
}