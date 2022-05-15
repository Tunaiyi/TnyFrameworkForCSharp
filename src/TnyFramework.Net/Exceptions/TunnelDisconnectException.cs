using System;
using System.Runtime.Serialization;

namespace TnyFramework.Net.Exceptions
{

    public class TunnelDisconnectException : NetException
    {
        public TunnelDisconnectException()
        {
        }

        public TunnelDisconnectException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public TunnelDisconnectException(string message) : base(message)
        {
        }

        public TunnelDisconnectException(Exception innerException, string message) : base(innerException, message)
        {
        }
    }

}
