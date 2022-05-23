using System;
using System.Runtime.Serialization;
using Grpc.Core;

namespace TnyFramework.Namespace.Etcd.Exceptions
{

    public class EtcdClosedWatcherException : EtcdException
    {
        public EtcdClosedWatcherException() : base(StatusCode.Cancelled)
        {
        }

        protected EtcdClosedWatcherException(SerializationInfo info, StreamingContext context) : base(StatusCode.Cancelled, info, context)
        {
        }

        public EtcdClosedWatcherException(string message) : base(StatusCode.Cancelled, message)
        {
        }

        public EtcdClosedWatcherException(string message, Exception innerException) : base(StatusCode.Cancelled, message, innerException)
        {
        }
    }

}
