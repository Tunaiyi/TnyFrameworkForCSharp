using System;
using System.Runtime.Serialization;

namespace TnyFramework.Namespace.Exceptions
{

    public class NamespaceHashingSubscriberClosedException : NamespaceHashingException
    {
        public NamespaceHashingSubscriberClosedException()
        {
        }

        protected NamespaceHashingSubscriberClosedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public NamespaceHashingSubscriberClosedException(string message) : base(message)
        {
        }

        public NamespaceHashingSubscriberClosedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

}
