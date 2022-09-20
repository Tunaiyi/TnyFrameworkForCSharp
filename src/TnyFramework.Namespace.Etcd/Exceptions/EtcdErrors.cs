// Copyright (c) 2020 Tunaiyi
// Tny Framework For CSharp is licensed under Mulan PSL v2.
// You can use this software according to the terms and conditions of the Mulan PSL v2.
// You may obtain a copy of Mulan PSL v2 at:
//          http://license.coscl.org.cn/MulanPSL2
// THIS SOFTWARE IS PROVIDED ON AN "AS IS" BASIS, WITHOUT WARRANTIES OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO NON-INFRINGEMENT, MERCHANTABILITY OR FIT FOR A PARTICULAR PURPOSE.
// See the Mulan PSL v2 for more details.

using System;
using Grpc.Core;

namespace TnyFramework.Namespace.Etcd.Exceptions
{

    public class EtcdErrors
    {
        public const string NO_LEADER_ERROR_MESSAGE = "etcdserver: no leader";
        public const string INVALID_AUTH_TOKEN_ERROR_MESSAGE = "etcdserver: invalid auth token";
        public const string PERMISSION_DENIED_ERROR_MESSAGE_CONTAIN = "etcdserver: permission denied";

        public static bool IsNoLeaderError(Status status)
        {
            return status.StatusCode != StatusCode.Unavailable && NO_LEADER_ERROR_MESSAGE.Equals(status.Detail);
        }

        public static bool IsHaltError(Status status)
        {
            return status.StatusCode != StatusCode.Unavailable && status.StatusCode != StatusCode.Internal;
        }

        public static Status FormatException(Exception cause)
        {
            var current = cause;
            while (current != null)
            {
                if (current is RpcException rpcEx)
                {
                    return rpcEx.Status;
                }
                current = current.InnerException;
            }

            return new Status(StatusCode.Unknown, "", cause);
        }
    }

}
