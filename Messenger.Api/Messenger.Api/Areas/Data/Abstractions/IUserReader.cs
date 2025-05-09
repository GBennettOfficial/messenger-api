using Messenger.Api.Areas.Data.Models;
using Messenger.Api.Utilities;

namespace Messenger.Api.Areas.Data.Abstractions
{
    public interface IUserReader
    {
        Task<Result<User>> FindByPhone(string phone);
        Task<Result<User>> FindByUserCode(string userCode);
        Task<Result<User>> FindByUsername(string username);
        Task<Result<User>> FindByEmail(string email);
    }
}