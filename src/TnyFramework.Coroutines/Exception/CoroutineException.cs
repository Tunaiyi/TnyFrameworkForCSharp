using System.Runtime.Serialization;
using TnyFramework.Common.Exception;
namespace TnyFramework.Coroutines.Exception
{
    public class CoroutineException : CommonException
    {
        public CoroutineException()
        {
        }


        public CoroutineException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }


        public CoroutineException(string message) : base(message)
        {
        }


        public CoroutineException(System.Exception innerException, string message) : base(innerException, message)
        {
        }
    }
}
