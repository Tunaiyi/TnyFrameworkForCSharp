using System;
using System.Runtime.Serialization;
using TnyFramework.Common.Exceptions;

namespace TnyFramework.Coroutines.Exceptions
{

    public class CoroutineException : CommonException
    {
        public CoroutineException()
        {
        }

        public CoroutineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CoroutineException(string message) : base(message)
        {
        }

        public CoroutineException(Exception innerException, string message) : base(innerException, message)
        {
        }
    }

}
