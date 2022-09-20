using System;
using System.Runtime.Serialization;
using TnyFramework.Common.Exceptions;

namespace TnyFramework.Common.Lifecycle.Exceptions
{

    public class LifecycleProcessException : CommonException
    {
        public LifecycleProcessException()
        {
        }

        protected LifecycleProcessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public LifecycleProcessException(string message) : base(message)
        {
        }

        public LifecycleProcessException(Exception innerException, string message) : base(innerException, message)
        {
        }

        public LifecycleProcessException(Exception innerException) : base(innerException, innerException.Message)
        {
        }
    }

}
