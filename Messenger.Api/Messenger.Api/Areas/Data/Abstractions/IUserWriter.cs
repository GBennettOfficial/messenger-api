using Messenger.Api.Areas.Data.Models;
using Messenger.Api.Utilities;

namespace Messenger.Api.Areas.Data.Abstractions
{
    public interface IUserWriter
    {
        Task<Result<User>> Create(User user);
        Task<Result> Update(User user);
    }
}