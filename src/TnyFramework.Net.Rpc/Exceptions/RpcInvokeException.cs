using System;
using System.Runtime.Serialization;
using TnyFramework.Common.Result;

namespace TnyFramework.Net.Rpc.Exceptions
{

    public class RpcInvokeException : RpcException
    {
        public RpcInvokeException(IResultCode code, SerializationInfo info, StreamingContext context) : base(code, info, context)
        {
        }

        public RpcInvokeException(IResultCode code, string message = "") : base(code, message)
        {
        }

        public RpcInvokeException(IResultCode code, Exception innerException, string message) : base(code, innerException, message)
        {
        }
    }

}
