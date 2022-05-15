using System;
using System.Runtime.Serialization;

namespace TnyFramework.Common.Scanner.Exceptions
{

    public class TypeScanException : Exception
    {
        public TypeScanException()
        {
        }

        public TypeScanException(string message) : base(message)
        {
        }

        public TypeScanException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TypeScanException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}
