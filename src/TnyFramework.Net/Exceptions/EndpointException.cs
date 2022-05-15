using System;
using System.Runtime.Serialization;

namespace TnyFramework.Net.Exceptions
{

    public class EndpointException : NetException
    {
        public EndpointException()
        {
        }

        public EndpointException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public EndpointException(string message) : base(message)
        {
        }

        public EndpointException(Exception innerException, string message) : base(innerException, message)
        {
        }
    }

}
