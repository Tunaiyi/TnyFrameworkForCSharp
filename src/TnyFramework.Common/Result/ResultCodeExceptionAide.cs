using System;
using TnyFramework.Common.Exceptions;

namespace TnyFramework.Common.Result
{

    public static class ResultCodeExceptionAide
    {
        public static IResultCode CodeOf(Exception cause, IResultCode defaultCode = default)
        {
            if (cause is ResultCodeException exception)
            {
                return exception.Code;
            }
            return defaultCode;
        }
    }

}
