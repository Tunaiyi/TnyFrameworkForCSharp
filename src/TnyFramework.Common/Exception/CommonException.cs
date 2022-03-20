using System;
using System.Runtime.Serialization;
namespace TnyFramework.Common.Exception
{
    public class CommonException : ApplicationException
    {
        public CommonException()
        {
        }


        public CommonException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }


        public CommonException(string message) : base(message)
        {
        }


        public CommonException(System.Exception innerException, string message) : base(message, innerException)
        {
        }
    }
}
