using System;
using System.Runtime.Serialization;
using TnyFramework.Common.Exception;
using TnyFramework.Common.Result;
using TnyFramework.Net.Common;

namespace TnyFramework.Net.Exceptions
{
    public class ValidatorFailException : ValidationException
    {
        public ValidatorFailException(IResultCode code, SerializationInfo info, StreamingContext context) : base(code, info, context)
        {
        }


        public ValidatorFailException(string message = "") : base(NetResultCode.VALIDATOR_FAIL_ERROR, message)
        {
        }


        public ValidatorFailException(IResultCode code, string message = "") : base(code, message)
        {
        }


        public ValidatorFailException(IResultCode code, Exception innerException, string message) : base(code, innerException, message)
        {
        }
    }
}
