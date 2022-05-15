using System;
using System.Runtime.Serialization;

namespace TnyFramework.Coroutines.Exceptions
{

    public class CoroutineStatusException : CoroutineException
    {
        public CoroutineStatusException()
        {
        }

        public CoroutineStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CoroutineStatusException(string message) : base(message)
        {
        }

        public CoroutineStatusException(Exception innerException, string message) : base(innerException, message)
        {
        }
    }

}
