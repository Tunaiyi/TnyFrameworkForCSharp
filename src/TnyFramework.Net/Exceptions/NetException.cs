using System;
using System.Runtime.Serialization;
using TnyFramework.Common.Exceptions;

namespace TnyFramework.Net.Exceptions
{

    public class NetException : CommonException
    {
        public NetException()
        {
        }

        public NetException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NetException(string message) : base(message)
        {
        }

        public NetException(Exception innerException, string message) : base(innerException, message)
        {
        }
    }

}
