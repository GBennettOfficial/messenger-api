using Messenger.Api.Areas.Cryptography.Models;

namespace Messenger.Api.Areas.Cryptography.Abstractions
{
    public interface IPasswordHasher
    {
        HashedPassword HashPassword(string userPassword);
    }
}
