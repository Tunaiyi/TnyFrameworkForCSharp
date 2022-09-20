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

    public class EtcdExceptionFactory
    {
        public static EtcdException NewEtcdException(StatusCode code, string message)
        {
            return new EtcdException(code, message, null);
        }

        public static EtcdException NewEtcdException(StatusCode code, string message, Exception cause)
        {
            return new EtcdException(code, message, cause);
        }

        public static EtcdCompactedException NewCompactedException(long compactedRev)
        {
            return new EtcdCompactedException(StatusCode.OutOfRange,
                "etcdserver: mvcc: required revision has been compacted", compactedRev);
        }

        public static EtcdClosedWatcherException NewClosedWatcherException()
        {
            return new EtcdClosedWatcherException();
        }

        public static EtcdClosedClientException NewClosedWatchClientException()
        {
            return new EtcdClosedClientException("Watch Client has been closed");
        }

        public static EtcdClosedClientException NewClosedLeaseClientException()
        {
            return new EtcdClosedClientException("Lease Client has been closed");
        }

        public static EtcdException ToEtcdException(Exception cause)
        {
            if (cause is EtcdException ex)
            {
                return ex;
            }
            return ToEtcdException(EtcdErrors.FormatException(cause));
        }

        public static EtcdException ToEtcdException(Status status)
        {
            return FromStatus(status);
        }

        private static EtcdException FromStatus(Status status)
        {
            return NewEtcdException(status.StatusCode, status.Detail, status.DebugException);
        }
    }

}
