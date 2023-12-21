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

namespace TnyFramework.Net.Rpc.Exceptions
{

    public class RpcException : ResultCodeException
    {


        public RpcException(IResultCode code, string message = "") : base(code, message)
        {
        }

        public RpcException(IResultCode code, Exception innerException, string message) : base(code, innerException, message)
        {
        }
    }

}
