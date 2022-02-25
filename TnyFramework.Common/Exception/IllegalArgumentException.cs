using System.Runtime.Serialization;
namespace TnyFramework.Common.Exception
{
    public class IllegalArgumentException : CommonException
    {
        public IllegalArgumentException()
        {
        }


        public IllegalArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }


        public IllegalArgumentException(string message) : base(message)
        {
        }


        public IllegalArgumentException(System.Exception innerException, string message) : base(innerException, message)
        {
        }
    }
}
