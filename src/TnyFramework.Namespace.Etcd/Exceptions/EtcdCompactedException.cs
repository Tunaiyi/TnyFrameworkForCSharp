using System;
using System.Runtime.Serialization;
using Grpc.Core;

namespace TnyFramework.Namespace.Etcd.Exceptions
{

    public class EtcdCompactedException : EtcdException
    {
        public long CompactedRevision { get; }

        public EtcdCompactedException(StatusCode code, long compactedRevision) : base(code)
        {
            this.CompactedRevision = compactedRevision;
        }

        protected EtcdCompactedException(StatusCode code, SerializationInfo info, StreamingContext context, long compactedRevision) : base(code, info,
            context)
        {
            this.CompactedRevision = compactedRevision;
        }

        public EtcdCompactedException(StatusCode code, string message, long compactedRevision) : base(code, message)
        {
            this.CompactedRevision = compactedRevision;
        }

        public EtcdCompactedException(StatusCode code, string message, Exception innerException, long compactedRevision) : base(code, message,
            innerException)
        {
            this.CompactedRevision = compactedRevision;
        }
    }

}
