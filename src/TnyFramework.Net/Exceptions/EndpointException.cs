// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Runtime.Serialization;
using TnyFramework.Common.Result;
using TnyFramework.Net.Common;

namespace TnyFramework.Net.Exceptions
{

    public class EndpointException : NetException
    {
        private static readonly IResultCode CODE = NetResultCode.ENDPOINT_CLOSE;

        public EndpointException(string message = "") : base(CODE, message)
        {
        }

        public EndpointException(IResultCode? code = null, object? body = null, string message = "")
            : base(code ?? CODE, body, message)
        {
        }

        public EndpointException(Exception innerException, IResultCode? code = null, object? body = null, string message = "")
            : base(innerException, code ?? CODE, body, message)
        {
        }

        public EndpointException(SerializationInfo info, StreamingContext context, IResultCode? code = null, object? body = null)
            : base(info, context, code ?? CODE, body)
        {
        }

        public EndpointException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

}
