// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Runtime.Serialization;
using TnyFramework.Common.Exceptions;
using TnyFramework.Common.Result;
using TnyFramework.Net.Common;

namespace TnyFramework.Net.Exceptions
{

    public class NetException : ResultCodeException
    {
        private static readonly IResultCode CODE = NetResultCode.SERVER_ERROR;

        public object Body { get; }

        public NetException(string message = "")
            : base(CODE, message)
        {
            Body = null!;
        }

        public NetException(IResultCode? code = null, object? body = null, string message = "")
            : base(code ?? CODE, message)
        {
            Body = body!;
        }

        public NetException(Exception innerException, IResultCode? code = null, object? body = null, string message = "")
            : base(code ?? CODE, innerException, message)
        {
            Body = body!;
        }

        public NetException(SerializationInfo info, StreamingContext context, IResultCode? code = null, object? body = null)
            : base(code ?? CODE, info, context)
        {
            Body = body!;
        }
    }

}
