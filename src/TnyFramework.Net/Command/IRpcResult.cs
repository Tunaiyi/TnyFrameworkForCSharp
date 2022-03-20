using TnyFramework.Common.Result;
namespace TnyFramework.Net.Command
{
    public interface IRpcResult
    {
        object Body { get; }

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
