using System;
using System.Runtime.Serialization;

namespace TnyFramework.Common.Exceptions
{

    public class IllegalArgumentException : CommonException
    {
        public IllegalArgumentException()
        {
        }

        public IllegalArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public IllegalArgumentException(string message) : base(message)
        {
        }

        public IllegalArgumentException(Exception innerException, string message) : base(innerException, message)
        {
        }
    }

}
