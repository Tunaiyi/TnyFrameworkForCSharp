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
