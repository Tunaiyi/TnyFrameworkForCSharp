using System;
using System.Runtime.Serialization;

namespace TnyFramework.Codec.Execptions
{

    public class ObjectCodecException : Exception
    {
        public ObjectCodecException()
        {
        }

        public ObjectCodecException(string message) : base(message)
        {
        }

        public ObjectCodecException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ObjectCodecException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

}
