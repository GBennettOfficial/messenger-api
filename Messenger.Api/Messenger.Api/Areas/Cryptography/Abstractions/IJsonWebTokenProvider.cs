using Messenger.Api.Areas.Cryptography.Models;
using Messenger.Api.Areas.Data.Models;

namespace Messenger.Api.Areas.Cryptography.Abstractions
{
    public interface IJsonWebTokenProvider
    {
        JsonWebToken CreateJsonWebToken(User user);
    }
}