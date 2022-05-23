using TnyFramework.Common.Result;
using TnyFramework.Net.Message;

namespace TnyFramework.Net.Command
{

    public interface IRpcResult
    {
        object Body { get; }

        IMessage Message { get; }

        IResultCode ResultCode { get; }

        string Description { get; }

        bool IsSuccess();

        bool IsFailure();
    }

    public interface IRpcResult<out TBody> : IRpcResult
    {
        new TBody Body { get; }
    }

}
