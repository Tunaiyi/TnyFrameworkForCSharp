using System;
using System.Runtime.Serialization;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Result;
using TnyFramework.Net.Common;

namespace TnyFramework.Net.Exceptions
{

    public class ValidationException : ResultCodeException
    {
        public ValidationException(IResultCode code, SerializationInfo info, StreamingContext context) : base(code, info, context)
        {
        }

        public ValidationException(string message = "") : base(NetResultCode.VALIDATOR_FAIL_ERROR, message)
        {
        }

        public ValidationException(Exception innerException, string message = "") : base(NetResultCode.VALIDATOR_FAIL_ERROR, innerException, message)
        {
        }

        public ValidationException(IResultCode code, string message = "") : base(code, message)
        {
        }

        public ValidationException(IResultCode code, Exception innerException, string message) : base(code, innerException, message)
        {
        }
    }

}
