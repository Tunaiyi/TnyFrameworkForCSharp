using System;
using System.Runtime.Serialization;

namespace TnyFramework.Namespace.Exceptions
{

    public class NamespaceNodeException : NamespaceException
    {
        public NamespaceNodeException()
        {
        }

        protected NamespaceNodeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NamespaceNodeException(string message) : base(message)
        {
        }

        public NamespaceNodeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}
