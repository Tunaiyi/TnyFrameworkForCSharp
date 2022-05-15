using TnyFramework.Common.Result;

namespace TnyFramework.Net.Command
{

    //TODO Protocol 设置
    internal class DefaultRpcResult<TBody> : IRpcResult<TBody>
    {
        public TBody Body { get; }

        public IResultCode ResultCode { get; }

        public string Description { get; }

        public DefaultRpcResult(IResultCode resultCode, TBody body)
        {
            Body = body;
            ResultCode = resultCode;
            Description = resultCode.Message;
        }

        public DefaultRpcResult(IResultCode resultCode)
        {
            ResultCode = resultCode;
            Description = resultCode.Message;
        }

        object IRpcResult.Body => Body;

        public bool IsSuccess() => ResultCode.IsSuccess();

        public bool IsFailure() => ResultCode.IsFailure();
    }

    public static class RpcResults
    {
        private static readonly IRpcResult SUCCESS = new DefaultRpcResult<object>(ResultCode.SUCCESS, null);

        /// <summary>
        /// 创建成功响应结果
        /// </summary>
        /// <returns>结果</returns>
        public static IRpcResult Success()
        {
            return SUCCESS;
        }

        /// <summary>
        /// 创建成功响应结果
        /// </summary>
        /// <param name="body">信息体</param>
        /// <typeparam name="TBody"></typeparam>
        /// <returns>返回响应结果</returns>
        public static IRpcResult<TBody> Success<TBody>(TBody body)
        {
            return new DefaultRpcResult<TBody>(ResultCode.SUCCESS, body);
        }

        /// <summary>
        /// 请求结果
        /// </summary>
        /// <returns>结果</returns>
        public static IRpcResult Result(IResultCode code)
        {
            return new DefaultRpcResult<object>(code);
        }

        /// <summary>
        /// 请求结果
        /// </summary>
        /// <returns>结果</returns>
        public static IRpcResult<TBody> Result<TBody>(IResultCode code)
        {
            return new DefaultRpcResult<TBody>(code);
        }

        /// <summary>
        /// 请求结果
        /// </summary>
        /// <returns>结果</returns>
        public static IRpcResult<TBody> Result<TBody>(IResultCode code, TBody body)
        {
            return new DefaultRpcResult<TBody>(code, body);
        }
        
    }

}
