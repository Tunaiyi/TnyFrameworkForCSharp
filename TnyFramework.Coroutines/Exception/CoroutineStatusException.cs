using System.Runtime.Serialization;
namespace TnyFramework.Coroutines.Exception
{
    public class CoroutineStatusException : CoroutineException
    {
        public CoroutineStatusException()
        {
        }


        public CoroutineStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }


        public CoroutineStatusException(string message) : base(message)
        {
        }


        public CoroutineStatusException(System.Exception innerException, string message) : base(innerException, message)
        {
        }
    }
}
