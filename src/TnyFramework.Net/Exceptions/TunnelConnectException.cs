// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using TnyFramework.Common.Result;

namespace TnyFramework.Net.Exceptions
{

    public class TunnelConnectException : TunnelException
    {
        public TunnelConnectException(string message = "") : base(message)
        {
        }

        public TunnelConnectException(IResultCode? code = null, object? body = null, string message = "")
            : base(code, body, message)
        {
        }

        public TunnelConnectException(Exception innerException, IResultCode? code = null, object? body = null, string message = "")
            : base(innerException, code, body, message)
        {
        }
    }

}
