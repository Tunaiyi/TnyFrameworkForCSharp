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

        public EtcdException(StatusCode code, string message, Exception innerException) : base(message, innerException)
        {
            Code = code;
        }
    }

}
