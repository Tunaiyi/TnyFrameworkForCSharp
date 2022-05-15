using System;
using System.Runtime.Serialization;

namespace TnyFramework.Net.Exceptions
{

    public class EndpointCloseException : EndpointException
    {
        public EndpointCloseException()
        {
        }

        public EndpointCloseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public EndpointCloseException(string message) : base(message)
        {
        }

        public EndpointCloseException(Exception innerException, string message) : base(innerException, message)
        {
        }
    }

}
