using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.SignalR.Protocol;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Messenger.Api.Utilities
{
    public record Result
    {
        public Result(ResultCode Code, string Message = "")
        {
            if (Code != ResultCode.Success && Code != ResultCode.Cancelled && Message == "")
            {
                throw new ArgumentException(nameof(Message), "message cannot be empty unless result code is 'Success' or 'Canceled'");
            }
            this.Code = Code;
            this.Message = Message;
        }
        public ResultCode Code { get; init; }
        public string Message { get; init; }
    }

    public record Result<T> : Result
    {
        public Result(ResultCode Code, T? Data, string Message = "") : base(Code, Message)
        {
            if (Code == ResultCode.Success && Data is null)
            {
                throw new ArgumentNullException(nameof(Data), "Data cannot be null when code is 'Success'");
            }
            this.Value = Data;

        }
        public T? Value { get; init; }
    }

    public enum ResultCode
    {
        Success = 0,
        Error = 1,
        Invalid = 2,
        Cancelled = 3,
        NotFound = 4,
        Failure = 5,
    }

}
