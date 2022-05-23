using System;
using System.Runtime.Serialization;

namespace TnyFramework.Namespace.Exceptions
{

    public class NamespaceHashingPartitionException : NamespaceHashingException
    {
        public NamespaceHashingPartitionException()
        {
        }

        protected NamespaceHashingPartitionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NamespaceHashingPartitionException(string message) : base(message)
        {
        }

        public NamespaceHashingPartitionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}
