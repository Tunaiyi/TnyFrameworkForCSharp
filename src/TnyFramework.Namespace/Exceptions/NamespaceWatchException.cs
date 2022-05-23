using System;
using System.Runtime.Serialization;

namespace TnyFramework.Namespace.Exceptions
{

    public class NameNodeWatchException : NamespaceException
    {
        public NameNodeWatchException()
        {
        }

        protected NameNodeWatchException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NameNodeWatchException(string message) : base(message)
        {
        }

        public NameNodeWatchException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}
