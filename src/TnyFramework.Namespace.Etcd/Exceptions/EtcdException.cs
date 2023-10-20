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
using TnyFramework.Namespace.Exceptions;

namespace TnyFramework.Namespace.Etcd.Exceptions
{

    public class EtcdException : NamespaceException
    {
        public StatusCode Code { get; }

        public EtcdException(StatusCode code)
        {
            Code = code;
        }

        protected EtcdException(StatusCode code, SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Code = code;
        }

        public EtcdException(StatusCode code, string message) : base(message)
        {
            Code = code;
        }

        public EtcdException(StatusCode code, string message, Exception? innerException) : base(message, innerException)
        {
            Code = code;
        }
    }

}
