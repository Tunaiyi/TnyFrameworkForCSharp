// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using System.Runtime.Serialization;
using Grpc.Core;

namespace TnyFramework.Namespace.Etcd.Exceptions
{

    public class EtcdClosedClientException : EtcdException
    {
        public EtcdClosedClientException() : base(StatusCode.Cancelled)
        {
        }

        protected EtcdClosedClientException(SerializationInfo info, StreamingContext context) : base(StatusCode.Cancelled, info, context)
        {
        }

        public EtcdClosedClientException(string message) : base(StatusCode.Cancelled, message)
        {
        }

        public EtcdClosedClientException(string message, Exception innerException) : base(StatusCode.Cancelled, message, innerException)
        {
        }
    }

}
