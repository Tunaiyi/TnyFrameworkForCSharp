using System;
using System.Runtime.Serialization;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Result;

namespace TnyFramework.Net.Rpc.Exceptions
{

    public class RpcException : ResultCodeException
    {
        public RpcException(IResultCode code, SerializationInfo info, StreamingContext context) : base(code, info, context)
        {
        }

        public RpcException(IResultCode code, string message = "") : base(code, message)
        {
        }

        public RpcException(IResultCode code, Exception innerException, string message) : base(code, innerException, message)
        {
        }
    }

}
