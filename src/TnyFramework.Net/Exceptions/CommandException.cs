using System.Runtime.Serialization;
using TnyFramework.Common.Exception;
using TnyFramework.Common.Result;

namespace TnyFramework.Net.Exceptions
{
    public class CommandException : ResultCodeException
    {
        public object Body { get; }


        public CommandException(IResultCode code, SerializationInfo info, StreamingContext context) :
            base(code, info, context)
        {
            Body = null;
        }


        public CommandException(IResultCode code, string message = "") : base(code, message)
        {
            Body = null;
        }


        public CommandException(IResultCode code, System.Exception innerException, string message = "") :
            base(code, innerException, message)
        {
            Body = null;
        }


        public CommandException(IResultCode code, SerializationInfo info, object body, StreamingContext context) :
            base(code, info, context)
        {
            Body = body;
        }


        public CommandException(IResultCode code, object body, string message = "") : base(code, message)
        {
            Body = body;
        }


        public CommandException(IResultCode code, System.Exception innerException, object body, string message = "") :
            base(code, innerException, message)
        {
            Body = body;
        }
    }
}
