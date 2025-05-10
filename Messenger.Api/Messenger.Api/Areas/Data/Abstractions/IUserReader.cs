using Messenger.Api.Areas.Data.Models;
using Messenger.Api.Utilities;

namespace Messenger.Api.Areas.Data.Abstractions
{
    public interface IUserReader
    {
        Task<Result<User>> SearchByPhone(string phone);
        Task<Result<User>> SearchByUserCode(string userCode);
        Task<Result<User>> SearchByUsername(string username);
        Task<Result<User>> SearchByEmail(string email);
    }
}