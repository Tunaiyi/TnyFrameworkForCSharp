using System;
using System.Runtime.Serialization;

namespace TnyFramework.Namespace.Exceptions
{

    public class NamespaceNodeCodecException : NamespaceNodeException
    {
        public NamespaceNodeCodecException()
        {
        }

        public NamespaceNodeCodecException(string message) : base(message)
        {
        }

        public NamespaceNodeCodecException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NamespaceNodeCodecException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}
