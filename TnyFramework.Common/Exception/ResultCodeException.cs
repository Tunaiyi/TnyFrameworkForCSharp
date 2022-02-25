using System.Runtime.Serialization;
using TnyFramework.Common.Result;
namespace TnyFramework.Common.Exception
{
    public class ResultCodeException : CommonException
    {
        public ResultCodeException(IResultCode code, SerializationInfo info, StreamingContext context) :
            base(info, context)
        {
            Code = code;
        }


        public ResultCodeException(IResultCode code, string message = "") : this(code, null, message)
        {
        }


        public ResultCodeException(IResultCode code, System.Exception innerException, string message) :
            base(innerException, message)
        {
            Code = code;
        }


        public IResultCode Code { get; }
    }
}
