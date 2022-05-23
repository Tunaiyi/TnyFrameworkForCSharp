using System;
using System.Runtime.Serialization;

namespace TnyFramework.Namespace.Exceptions
{

    public class NamespaceHashingException : NamespaceException
    {
        public NamespaceHashingException()
        {
        }

        protected NamespaceHashingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NamespaceHashingException(string message) : base(message)
        {
        }

        public NamespaceHashingException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}
