using Messenger.Api.Areas.Cryptography.Models;

namespace Messenger.Api.Areas.Cryptography.Abstractions
{
    public interface IPasswordChecker
    {
        public bool CheckPassword(string userPassword, HashedPassword hashedPassword);
    }
}
