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
