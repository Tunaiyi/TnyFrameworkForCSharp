using System;
using System.Runtime.Serialization;

namespace TnyFramework.Namespace.Exceptions
{

    public class NamespaceException : Exception
    {
        public NamespaceException()
        {
        }

        protected NamespaceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NamespaceException(string message) : base(message)
        {
        }

        public NamespaceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}
